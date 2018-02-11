using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SaluteOnline.Domain.DTO;
using SaluteOnline.IdentityServer.Constants;
using SaluteOnline.IdentityServer.Domain;
using SaluteOnline.IdentityServer.Service.Implementation;
using SaluteOnline.IdentityServer.ViewModels;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SaluteOnline.Domain.DTO.Activity;
using SaluteOnline.Domain.Events;
using SaluteOnline.IdentityServer.Service.Declaration;

namespace SaluteOnline.IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SoApplicationUser> _userManager;
        private readonly SignInManager<SoApplicationUser> _signInManager;
        private readonly AccountService _accountService;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IEventService _eventService;
        private readonly WebApplicationClientSettings _webApplicationClientSettings;
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IBusService _busService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IIdentityServerInteractionService interactionService, IHttpContextAccessor httpContextAccessor, IAuthenticationSchemeProvider schemeProvider, IClientStore clientStore,
            UserManager<SoApplicationUser> userManager, SignInManager<SoApplicationUser> signInManager, IEventService eventService, ILogger<AccountController> logger,
            IOptions<WebApplicationClientSettings> webApplicationClientSettings, IPersistedGrantService persistedGrantService, IBusService busService)
        {
            _persistedGrantService = persistedGrantService;
            _userManager = userManager;
            _signInManager = signInManager;
            _eventService = eventService;
            _interactionService = interactionService;
            _webApplicationClientSettings = webApplicationClientSettings.Value;
            _accountService = new AccountService(interactionService, httpContextAccessor, schemeProvider, clientStore);
            _busService = busService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            try
            {
                var vm = await _accountService.GenerateLoginViewModelAsync(returnUrl);
                if (vm.IsExternalLoginOnly)
                    return await ExternalLogin(vm.ExternalLoginScheme, returnUrl);
                return View(vm);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new
                {
                    message = "Internal error. Please try a bit later",
                    type = ErrorType.LoginFail
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Username);
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                        AuthenticationProperties props = null;
                        if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                        {
                            props = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                            };
                        }
                        await HttpContext.SignInAsync(user.Id, user.UserName, props);
                        if (!_interactionService.IsValidReturnUrl(model.ReturnUrl) && !Url.IsLocalUrl(model.ReturnUrl) && !model.ReturnUrl.Contains("localhost"))
                            return RedirectToAction("Error", "Account", new { message = "Redirect URL not valid. Please try a bit later", type = ErrorType.LoginFail });

                        _busService.Publish(new ActivitySet
                        {
                            SubjectId = user.Id,
                            Type = ActivityType.Login,
                            Importance = ActivityImportance.Low,
                            Status = ActivityStatus.Success
                        });
                        return Redirect(model.ReturnUrl);
                    }
                    await _eventService.RaiseAsync(new UserLoginFailureEvent(model.Username, "Invalid credentials"));
                    ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
                    _busService.Publish(new ActivitySet
                    {
                        SubjectId = user?.Id,
                        Type = ActivityType.Login,
                        Importance = ActivityImportance.Low,
                        Status = ActivityStatus.Fail,
                        Data = JsonConvert.SerializeObject(model)
                    });
                }
                var vm = await _accountService.GenerateLoginViewModelAsync(model);
                return View(vm);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Internal login error. Please try a bit later", type = ErrorType.LoginFail });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl)
        {
            try
            {
                returnUrl = Url.Action("ExternalLoginCallback", new { returnUrl });
                if (AccountOptions.WindowsAuthenticationSchemeName == provider)
                {
                    var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
                    if (!(result?.Principal is WindowsPrincipal wp))
                        return new ChallengeResult(AccountOptions.WindowsAuthenticationSchemeName);

                    var props = new AuthenticationProperties();
                    props.Items.Add("scheme", AccountOptions.WindowsAuthenticationSchemeName);
                    var id = new ClaimsIdentity(provider);
                    id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                    id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));
                    if (AccountOptions.IncludeWindowsGroups)
                    {
                        var wi = wp.Identity as WindowsIdentity;
                        var groups = wi?.Groups?.Translate(typeof(NTAccount));
                        var roles = groups?.Select(x => new Claim(JwtClaimTypes.Role, x.Value));
                        id.AddClaims(roles);
                    }
                    await HttpContext.SignInAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme, new ClaimsPrincipal(id), props);
                    return Redirect(returnUrl);
                }
                var authProps = new AuthenticationProperties
                {
                    RedirectUri = returnUrl,
                    Items = { { "scheme", provider } }
                };
                return new ChallengeResult(provider, authProps);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Login error. Please try a bit later", type = ErrorType.ExternalLoginFail });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
                if (result?.Succeeded != true)
                    return RedirectToAction("Error", "Account", new { message = "Cannot authenticate. Please try a bit later", type = ErrorType.ExternalLoginFail });

                var externalUser = result.Principal;
                var claims = externalUser.Claims.ToList();
                var userIdClaim = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.Subject) ??
                                  claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return RedirectToAction("Error", "Account", new { message = "Wrong user claims. Please try a bit later", type = ErrorType.ExternalLoginFail } );

                claims.Remove(userIdClaim);
                var provider = result.Properties.Items["scheme"];
                var userId = userIdClaim.Value;

                var user = await _userManager.FindByLoginAsync(provider, userId);
                if (user == null)
                {
                    user = new SoApplicationUser
                    {
                        UserName = Guid.NewGuid().ToString(),
                        Role = Roles.Guest
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userId, provider));
                }
                var additionalClaim = new List<Claim>();
                var sid = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.SessionId);
                if (sid != null)
                {
                    additionalClaim.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
                }

                var props = new AuthenticationProperties();
                var idToken = result.Properties.GetTokenValue("id_token");
                if (!string.IsNullOrEmpty(idToken))
                {
                    props.StoreTokens(new List<AuthenticationToken> { new AuthenticationToken { Name = "id_token", Value = idToken} });
                }

                await _eventService.RaiseAsync(new UserLoginSuccessEvent(provider, userId, user.Id, user.UserName));
                await HttpContext.SignInAsync(user.Id, user.UserName, provider, props, additionalClaim.ToArray());
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                var returnUrl = result.Properties.Items["returnUrl"];
                if (!_interactionService.IsValidReturnUrl(returnUrl) && !Url.IsLocalUrl(returnUrl))
                    return RedirectToAction("Error", "Account", new {message = "Login error. Please try a bit later", type = ErrorType.ExternalLoginFail});

                _busService.Publish(new ActivitySet
                {
                    SubjectId = userId,
                    Type = ActivityType.ExternalLogin,
                    Importance = ActivityImportance.Low,
                    Status = ActivityStatus.Success,
                    Data = JsonConvert.SerializeObject(new { provider, returnUrl })
                });
                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Login error. Please try a bit later", type = ErrorType.ExternalLoginFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            try
            {
                if (User.Identity.IsAuthenticated == false)
                    return await Logout(new LogoutViewModel {LogoutId = logoutId});

                var context = await _interactionService.GetLogoutContextAsync(logoutId);
                if (context?.ShowSignoutPrompt == false)
                    return await Logout(new LogoutViewModel {LogoutId = logoutId});

                var vm = new LogoutViewModel
                {
                    LogoutId = logoutId
                };
                return View(vm);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Logout error. Please try a bit later", type = ErrorType.LogoutFail });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            try
            {
                var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                var subjectId = HttpContext.User.Identity.GetSubjectId();

                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider && !string.IsNullOrEmpty(model.LogoutId))
                {
                    model.LogoutId = await _interactionService.CreateLogoutContextAsync();
                }

                await _signInManager.SignOutAsync();
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
                var logout = await _interactionService.GetLogoutContextAsync(model.LogoutId);
                var vm = new LoggedOutViewModel
                {
                    PostLogoutRedirectUri = logout?.PostLogoutRedirectUri ?? _webApplicationClientSettings.PostLogoutRedirectUris.FirstOrDefault(),
                    ClientName = logout?.ClientName,
                    SignoutIframeUrl = logout?.SignOutIFrameUrl
                };
                await _persistedGrantService.RemoveAllGrantsAsync(subjectId, logout?.ClientId);
                _busService.Publish(new ActivitySet
                {
                    SubjectId = subjectId,
                    Type = ActivityType.Logout,
                    Importance = ActivityImportance.Low,
                    Status = ActivityStatus.Success,
                    Data = JsonConvert.SerializeObject(model)
                });
                return View("LoggedOut", vm);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Logout error. Please try a bit later", type = ErrorType.LogoutFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterUserViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = new SoApplicationUser
                {
                    UserName = model.Nickname,
                    Email = model.Email,
                    Role = Roles.User
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return View(model);
                _busService.Publish(new ActivitySet
                {
                    SubjectId = user.Id,
                    Type = ActivityType.Register,
                    Importance = ActivityImportance.High,
                    Status = ActivityStatus.Success,
                    Data = JsonConvert.SerializeObject(model)
                });
                var registerCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    code = registerCode,
                    returnUrl
                });
                _busService.Publish(new SendEmailEvent
                {
                    To = new List<string> { model.Email },
                    HtmlBody = $"<div>click to {callbackUrl}</div>",
                    Subject = "registration"
                });
                return View("RegisterConfirmation");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Registration error. Please try a bit later", type = ErrorType.RegisterFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                    return RedirectToAction("Error", "Account", new { message = "Wrong arguments. Please contact support", type = ErrorType.ConfirmEmailFail });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return RedirectToAction("Error", "Account", new { message = "User not found", type = ErrorType.ConfirmEmailFail });

                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (!result.Succeeded)
                    return RedirectToAction("Error", "Account", new { message = "Confirmation error. Please try a bit later", type = ErrorType.ConfirmEmailFail });

                await _eventService.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                await HttpContext.SignInAsync(user.Id, user.UserName, (AuthenticationProperties) null);

                _busService.Publish(new ActivitySet
                {
                    SubjectId = user.Id,
                    Type = ActivityType.EmailConfirmed,
                    Importance = ActivityImportance.Medium,
                    Status = ActivityStatus.Success,
                    Data = JsonConvert.SerializeObject(new { userId, code, returnUrl })
                });
                _busService.Publish(new UserRegisteredEvent
                {
                    SubjectId = user.Id,
                    Email = user.Email,
                    Username = user.UserName
                });
                if (_interactionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Confirmation error. Please try a bit later", type = ErrorType.ConfirmEmailFail });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email) ?? await _userManager.FindByNameAsync(email);
                if (user == null)
                    return RedirectToAction("Error", "Account", new { message = "User not found", type = ErrorType.ChangePasswordFail });

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new
                    {
                        userId = user.Id,
                        code,
                        redirectUrl = _webApplicationClientSettings.RedirectUris.FirstOrDefault()
                    },
                    HttpContext.Request.Scheme);
                return Redirect(callbackUrl);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Internal error. Please try a bit later", type = ErrorType.ChangePasswordFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId = null, string code = null, string redirectUrl = null)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(redirectUrl))
                return RedirectToAction("Error", "Account", new { message = "Invalid reset password request. Please contact support", type = ErrorType.ResetPasswordFail });
            return View(new ResetPasswordViewModel
            {
                RedirectUrl = redirectUrl,
                Code = code,
                UserId = userId
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("Error", "Account", new { message = "Internal error. Please try a bit later", type = ErrorType.ResetPasswordFail });

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return RedirectToAction("Error", "Account", new { message = "User not found", type = ErrorType.ResetPasswordFail });

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

                _busService.Publish(new ActivitySet
                {
                    SubjectId = user.Id,
                    Type = ActivityType.PasswordChanged,
                    Importance = ActivityImportance.Medium,
                    Status = result.Succeeded ? ActivityStatus.Success : ActivityStatus.Fail
                });

                return result.Succeeded ? 
                    RedirectToAction(nameof(ResetPasswordConfirmation), "Account", new {redirectUrl = model.RedirectUrl}) : 
                    RedirectToAction("Error", "Account", new { message = $"Error while changing password: \r\n{string.Join("\r\n", result.Errors)}", type = ErrorType.ResetPasswordFail });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Reset password error. Please try a bit later", type = ErrorType.ResetPasswordFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation(string redirectUrl = null)
        {
            try
            {
                var domain = _webApplicationClientSettings.ClientUri.Replace("https://", string.Empty)
                    .Replace("http://", string.Empty);
                if (string.IsNullOrEmpty(redirectUrl) || !redirectUrl.Contains(domain))
                    return RedirectToAction("Error", "Account", new { message = "Reset password error. Please try a bit later", type = ErrorType.ResetPasswordFail });

                var request = new RequestUrl(_webApplicationClientSettings.ClientUri);
                var url = request.CreateAuthorizeUrl(_webApplicationClientSettings.ClientId,
                    OidcConstants.ResponseTypes.CodeIdToken, OidcConstants.ResponseModes.FormPost, redirectUrl,
                    CryptoRandom.CreateUniqueId(), CryptoRandom.CreateUniqueId());
                return Redirect(url);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Reset password confirmation error. Please try a bit later", type = ErrorType.ResetPasswordFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string redirectUrl = null)
        {
            return View(new ForgotPasswordViewModel { ReturnUrl = redirectUrl});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return RedirectToAction("Error", "Account", new { message = "Internal error. Please try a bit later", type = ErrorType.ForgotPasswordFail });

                var user = await _userManager.FindByEmailAsync(model.Email) ?? await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                    return RedirectToAction("Error", "Account", new { message = "User not found", type = ErrorType.ForgotPasswordFail });

                if (!await _userManager.IsEmailConfirmedAsync(user))
                    return RedirectToAction("Error", "Account", new { message = "Your registration was not completed. Please verify your email before changing password", type = ErrorType.ForgotPasswordFail });

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new {userId = user.Id, code, redirectUrl = model.ReturnUrl}, HttpContext.Request.Scheme);
                _busService.Publish(new SendEmailEvent
                {
                    To = new List<string> { model.Email },
                    Subject = "Reset password",
                    HtmlBody = $"<div>{callbackUrl}</div>"
                });
                return View("ForgotPasswordConfirmation");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Internal error. Please try a bit later", type = ErrorType.ForgotPasswordFail });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost("/account/changeRole")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest("User not found");

                user.Role = model.NewRole;
                var result = _userManager.UpdateAsync(user).Result;

                _busService.Publish(new ActivitySet
                {
                    SubjectId = user.Id,
                    Type = ActivityType.RoleChanged,
                    Importance = ActivityImportance.High,
                    Status = result.Succeeded ? ActivityStatus.Success : ActivityStatus.Fail,
                    Data = JsonConvert.SerializeObject(model)
                });

                if (result.Succeeded)
                    return Ok();
                return BadRequest(result.Errors);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpPost("ro/register")]
        [AllowAnonymous]
        public async Task<IActionResult> RoRegister([FromBody] RegisterUser model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Wrong model");

                var user = new SoApplicationUser
                {
                    UserName = model.Nickname,
                    Email = model.Email,
                    Role = Roles.User
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest("Error while creating user. Please try a bit later");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code},
                    HttpContext.Request.Scheme);
                _busService.Publish(new SendEmailEvent
                {
                    To = new List<string> { model.Email },
                    Subject = "Register",
                    HtmlBody = $"<div>{link}</div>"
                });

                _busService.Publish(new ActivitySet
                {
                    SubjectId = user.Id,
                    Type = ActivityType.Register,
                    Importance = ActivityImportance.High,
                    Status = result.Succeeded ? ActivityStatus.Success : ActivityStatus.Fail,
                    Data = JsonConvert.SerializeObject(model)
                });

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpPost("ro/forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> RoForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return BadRequest("User not found");
                if (!await _userManager.IsEmailConfirmedAsync(user))
                    return BadRequest("Email not confirmed");

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account",
                    new {userId = user.Id, code, redirectUrl = model.ReturnUrl}, HttpContext.Request.Scheme);
                _busService.Publish(new SendEmailEvent
                {
                    To = new List<string> { model.Email },
                    Subject = "Reset Password",
                    HtmlBody = $"<div>{link}</div>"
                });
                return Ok();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Error(string message, ErrorType type = ErrorType.Unknown)
        {
            return View(new ErrorViewModel
            {
                Message = message,
                Type = type
            });
        }

        [HttpGet]
        public IActionResult ContactSupport(ErrorType type = ErrorType.Unknown)
        {
            return View(new ContactSupportViewModel
            {
                Type = type
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactSupport(ContactSupportViewModel model)
        {
            try
            {
                var subjectId = HttpContext.User.Identity.GetSubjectId();
                _busService.Publish(new ContactSupportEvent
                {
                      SubjectId = subjectId,
                      Created = DateTimeOffset.UtcNow,
                      From = model.Email,
                      Message = model.Content
                });
                return RedirectToAction("ContactSupportConfirmation", "Account");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Internal error. Please try a bit later", type = ErrorType.ContactSupportFail });
            }
        }

        [HttpGet]
        public IActionResult ContactSupportConfirmation()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("Error", "Account", new { message = "Internal error. Please try a bit later", type = ErrorType.ContactSupportFail });
            }
        }
    }
}
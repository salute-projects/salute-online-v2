using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaluteOnline.MailService.Model;
using SaluteOnline.MailService.Services.Declaration;
using SaluteOnline.Shared.Events;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SaluteOnline.MailService.Services.Implementation
{
    public class SesHandler : ISesHandler
    {
        private readonly SendGridClient _client;
        private readonly ILogger<SesHandler> _logger;
        private readonly MainSettings _settings;

        public SesHandler(IOptions<MainSettings> awsSettings, ILogger<SesHandler> logger, IConfiguration configuration)
        {
            _settings = awsSettings.Value;
            _client = new SendGridClient(configuration.GetValue<string>("SendGridKey"));
            _logger = logger;
        }

        public async Task<bool> HandleEmail(SendEmailEvent email)
        {
            try
            {
                var from = new EmailAddress(string.IsNullOrEmpty(email.From) ? _settings.FromAddress : email.From, "");
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from,
                    email.To.Select(t => new EmailAddress(t)).ToList(), email.Subject, email.TextBody, email.HtmlBody,
                    false);
                if (email.Bcc.Any())
                {
                    msg.AddBccs(email.Bcc.Select(t => new EmailAddress(t)).ToList());
                }
                if (email.Cc.Any())
                {
                    msg.AddCcs(email.Cc.Select(t => new EmailAddress(t)).ToList());
                }
                var response = await _client.SendEmailAsync(msg);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, JsonConvert.SerializeObject(email));
                return false;
            }
        }

        public async Task<bool> HandleSupportEvent(ContactSupportEvent supportEvent)
        {
            try
            {
                if (string.IsNullOrEmpty(supportEvent.From) || string.IsNullOrEmpty(supportEvent.Message))
                    throw new Exception("Wrong input model");
                return await HandleEmail(new SendEmailEvent
                {
                    To = new List<string> {_settings.SupportAddress},
                    From = supportEvent.From,
                    TextBody = supportEvent.Message,
                    Subject =
                        !string.IsNullOrEmpty(supportEvent.SubjectId)
                            ? $"Support request: [SubId {supportEvent.SubjectId}]"
                            : supportEvent.UserId != default(long)
                                ? $"Support request: [UsId {supportEvent.UserId}]"
                                : "External support request"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, JsonConvert.SerializeObject(supportEvent));
                return false;
            }
        }
    }
}

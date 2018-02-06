using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaluteOnline.Domain.DTO.Activity;
using SaluteOnline.Domain.Events;
using SaluteOnline.MailService.Model;
using SaluteOnline.MailService.Services.Declaration;

namespace SaluteOnline.MailService.Services.Implementation
{
    public class SesHandler : ISesHandler
    {
        private readonly AmazonSimpleEmailServiceClient _client;
        private readonly ILogger<SesHandler> _logger;
        private readonly AwsSettings _settings;

        public SesHandler(IOptions<AwsSettings> awsSettings, ILogger<SesHandler> logger)
        {
            _settings = awsSettings.Value;
            _client = new AmazonSimpleEmailServiceClient(new BasicAWSCredentials(_settings.AccessKey, _settings.SecretKey), RegionEndpoint.EUWest2);
            _logger = logger;
        }

        public async Task<bool> HandleEmail(SendEmailEvent email)
        {
            try
            {
                if (email == null || !IsEmailValid(email))
                    throw new Exception("Wrong input model");
                var to = new Destination
                {
                    ToAddresses = email.To,
                    //BccAddresses = email.Bcc,
                    //CcAddresses = email.Cc
                };
                var subject = new Content(email.Subject);
                var body = new Body
                {
                    //Text = new Content(email.TextBody),
                    Html = new Content(email.HtmlBody)
                };
                var message = new Message(subject, body);
                var request = new SendEmailRequest(string.IsNullOrEmpty(email.From) ? _settings.FromAddress : email.From, to, message);
                var result = await _client.SendEmailAsync(request);
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

        private static bool IsEmailValid(SendEmailEvent email)
        {
            if (string.IsNullOrEmpty(email.Subject) ||
                string.IsNullOrEmpty(email.HtmlBody) && string.IsNullOrEmpty(email.TextBody))
                return false;
            return email.To != null && !email.To.All(string.IsNullOrEmpty);
        }
    }
}

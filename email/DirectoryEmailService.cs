using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace email
{
    /// <summary>
    /// Delivers email as .EML files to a specified directory. 
    /// Useful for inspecting email formatting prior to a product launch.
    /// </summary>
    public class DirectoryEmailService : EmailServiceBase
    {
        private readonly Func<SmtpClient> _client;

        public DirectoryEmailService(string directory)
        {
            _client = () => new SmtpClient
            {
                Host = "localhost",
                Credentials = CredentialCache.DefaultNetworkCredentials,
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = directory
            };
        }

        public override bool Send(EmailMessage message)
        {
            AlternateView textView;
            AlternateView htmlView;
            var smtpMessage = SmtpEmailService.BuildMessageAndViews(message, out textView, out htmlView);
            try
            {
                _client().Send(smtpMessage);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (htmlView != null)
                {
                    htmlView.Dispose();
                }
                if (textView != null)
                {
                    textView.Dispose();
                }  
            }
        }

        public override bool Send(IEnumerable<EmailMessage> messages)
        {
            return messages.Aggregate(true, (current, message) => current & Send(message));
        }
    }
}
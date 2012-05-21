using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace email
{
    /// <summary>
    /// Delivers email through the default .NET SMTP client provider
    /// </summary>
    internal class SmtpEmailService : EmailServiceBase
    {
        private readonly Func<SmtpClient> _client;

        public SmtpEmailService()
        {
            _client = () => new SmtpClient { Host = "localhost", Credentials = CredentialCache.DefaultNetworkCredentials };
        }

        public SmtpEmailService(Func<SmtpClient> client)
        {
            _client = client;
        }

        public override bool Send(EmailMessage message)
        {
            AlternateView textView;
            AlternateView htmlView;
            var smtpMessage = BuildMessageAndViews(message, out textView, out htmlView);

            try
            {
                _client().Send(smtpMessage);
                return true;
            }
            catch (SmtpException)
            {
                return false;
            }
            finally
            {
                if(htmlView != null)
                {
                    htmlView.Dispose();
                }
                if(textView != null)
                {
                    textView.Dispose();
                }
            }
        }

        public override bool Send(IEnumerable<EmailMessage> messages)
        {
            return messages.Aggregate(true, (current, message) => current & Send(message));
        }

        public static MailMessage BuildMessageAndViews(EmailMessage message, out AlternateView textView, out AlternateView htmlView)
        {
            var smtpMessage = new MailMessage { BodyEncoding = Encoding.UTF8, From = new MailAddress(message.From) };
            if(message.To.Count > 0) smtpMessage.To.Add(string.Join(",", message.To));
            if(message.ReplyTo.Count > 0) smtpMessage.ReplyToList.Add(string.Join(",", message.ReplyTo));
            if(message.Cc.Count > 0) smtpMessage.CC.Add(string.Join(",", message.Cc));
            if(message.Bcc.Count > 0) smtpMessage.Bcc.Add(string.Join(",", message.Bcc));
            
            htmlView = null;
            textView = null;

            if (!string.IsNullOrWhiteSpace(message.HtmlBody))
            {
                var mimeType = new ContentType("text/html");
                htmlView = AlternateView.CreateAlternateViewFromString(message.HtmlBody, mimeType);
                smtpMessage.AlternateViews.Add(htmlView);
            }

            if (!string.IsNullOrWhiteSpace(message.TextBody))
            {
                const string mediaType = "text/plain";
                textView = AlternateView.CreateAlternateViewFromString(message.TextBody, smtpMessage.BodyEncoding, mediaType);
                textView.TransferEncoding = TransferEncoding.SevenBit;
                smtpMessage.AlternateViews.Add(textView);
            }
            return smtpMessage;
        }
    }
}
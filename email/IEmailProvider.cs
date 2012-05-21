using System.Collections.Generic;

namespace email
{
    public interface IEmailProvider
    {
        bool Send(EmailMessage message);
        bool Send(IEnumerable<EmailMessage> messages);
    }
}
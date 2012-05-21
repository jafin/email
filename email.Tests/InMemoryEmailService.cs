using System.Collections.Generic;
using System.Linq;

namespace email.Tests
{
    public class InMemoryEmailService : EmailServiceBase
    {
        public ICollection<EmailMessage> Messages { get; private set; }

        public InMemoryEmailService()
        {
            Messages = new List<EmailMessage>();
        }

        public override bool Send(EmailMessage message)
        {
            lock(Messages)
            {
                Messages.Add(message);
                return true;
            }
        }

        public override bool Send(IEnumerable<EmailMessage> messages)
        {
            return messages.Aggregate(true, (current, message) => current & Send(message));
        }
    }
}

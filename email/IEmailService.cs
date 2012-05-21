namespace email
{
    public interface IEmailService : IEmailProvider
    {
        EmailMessage CreateTextEmail(string textTemplate, dynamic model);
        EmailMessage CreateCombinedEmail(string htmlTemplate, string textTemplate, dynamic model);
        EmailMessage CreateHtmlEmail(string htmlTemplate, dynamic model);
    }
}
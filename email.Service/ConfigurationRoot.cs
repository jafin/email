using System.Configuration;
using Munq;
using email.Postmark;

namespace email.Service
{
    public class ConfigurationRoot
    {
        private static readonly IocContainer Container;

        static ConfigurationRoot()
        {
            Container = new IocContainer();
            Container.Register<IEmailService>(r => new PostmarkEmailService(ConfigurationManager.AppSettings["PostmarkServerToken"]));
        }

        public static T GetInstance<T>() where T : class
        {
            return Container.Resolve<T>();
        }
    }
}
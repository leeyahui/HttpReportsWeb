using System.Configuration;

namespace HttpReports.Web.Models
{
    public class HttpReportsConfig
    {
        public HttpReportsConfig()
        {
            this.UserName = ConfigurationManager.AppSettings["UserName"];
            this.Password = ConfigurationManager.AppSettings["Password"];
        }

        public string UserName { get; set; }

        public string Password { get; set; }

    }
}

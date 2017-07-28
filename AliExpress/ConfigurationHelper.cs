using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Configuration;
using System.Configuration;

namespace AliExpress
{
    public class ConfigurationHelper:IConfiguration
    {
        public string GetUserName()
        {
            return ConfigurationManager.AppSettings["userName"];
        }

        public string GetPassword()
        {
            return ConfigurationManager.AppSettings["password"];
        }

        public string GetConnectName()
        {
            return ConfigurationManager.AppSettings["connectName"];
        }

        private void Test()
        {
            string userName = GetUserName();
            string password = GetPassword();
            string connectName = GetConnectName();
        }
    }
}

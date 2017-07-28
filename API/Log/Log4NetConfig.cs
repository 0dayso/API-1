using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace API.Log
{
    public class Log4NetConfig
    {
        public static ILog InitLog4Net(Type type)
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            var logConfig = new FileInfo(directory);
            XmlConfigurator.ConfigureAndWatch(logConfig);
            ILog logger = LogManager.GetLogger(type);
            return logger;
        }
    }
}

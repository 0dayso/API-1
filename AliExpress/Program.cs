using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using API;
using API.ADSL;
using API.Configuration;
using API.Log;
using API.Process;

namespace AliExpress
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Process();

            //Console.ReadKey();
        }


        private void Process()
        {
            IProcess processHandler = new ProcessHandler();
            IConfiguration configuration = new ConfigurationHelper();
            while (true)
            {
                processHandler.Process(exception =>
                {
                    WriteLog writeLog = new WriteLog();
                    writeLog.WriteDirectly(this, new ExceptionEventArgs(exception?.Message, exception));
                    //Environment.Exit(1);
                }, () =>
                {
                    //Adsl.Connect(configuration.GetConnectName(), configuration.GetUserName(), configuration.GetPassword());
                    Adsl.Connect(configuration.GetConnectName());
                });


                Console.WriteLine("暂停2分钟后取任务");
                Thread.Sleep(TimeSpan.FromMinutes(2));
            }

            
        }



        private void Test()
        {
            try
            {
                int a = 10;
                int b = 0;
                int c = a/b;
            }
            catch (Exception e)
            {
                WriteLog writeLog = new WriteLog();
                writeLog.WriteDirectly(this,new ExceptionEventArgs(e.Message,e));
            }
        }


    }
}

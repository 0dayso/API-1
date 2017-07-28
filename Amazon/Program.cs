using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using API;
using API.ADSL;
using API.Configuration;
using API.Handler;
using API.Helper;
using API.Log;
using API.Process;

namespace Amazon
{
    class Program
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

        void Test()
        {
            MySqlHelper taskHelper = new MySqlHelper();
            MySqlHelper resultHelper = new MySqlHelper();
            TaskDbHandler taskDbHandler = new AmazonTaskDbHandler(taskHelper);
            ResultDbHandler resultDbHandler = new AmazonResultDbHandler(resultHelper);
            OrderListHandler orderListHandler = new OrderListHandler("加拿大", new List<string> { "ATVPDKIKX0DER" }, "AKIAJTPVS26NBX7CKCGA", "ALM27M18VEIRS", "0f5708dECCx680pEVPFq40bNg4SLz3L0cb9lYpqm");
            IDictionary<string, string> dic = orderListHandler.GetParameterDic("2017-02-28T16:00:00Z");
            string html = orderListHandler.GetOrderListHtml(dic);
            //orderListHandler.GetMainInfoDicList(html, null);
        }



    }
}

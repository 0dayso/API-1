using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace API.Log
{
    public class WriteLog:IWriteLog
    {

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <param name="type"></param>
        public void Write(object sender, EventArgs eventArgs,Type type)
        {
            ILog logger = Log4NetConfig.InitLog4Net(type);
            ExceptionEventArgs exceptionEventArgs = eventArgs as ExceptionEventArgs;
            if (exceptionEventArgs == null) return;
            if (exceptionEventArgs.Exception == null)
            {
                logger.Info(exceptionEventArgs.Message);
            }
            else
            {
                logger.Error(exceptionEventArgs.Message, exceptionEventArgs.Exception);
            }

        }

        /// <summary>
        /// 只有单个的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void WriteDirectly(object sender, EventArgs eventArgs)
        {
            WriteLogManager writeLogManager = new WriteLogManager();
            writeLogManager.Add((s,e)=> Write(sender, eventArgs, typeof(WriteLog)));
            writeLogManager.WriteLog(sender, eventArgs);
        }




        /// <summary>
        /// 有多个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public void Write(object sender, EventArgs eventArgs)
        {
            Write(sender, eventArgs, typeof(WriteLog));
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Log
{
    internal class WriteLogTest
    {
        internal void Test()
        {
            WriteLog writeLog = new WriteLog();
            WriteLogManager writeLogManager = new WriteLogManager();
            writeLogManager.Add(writeLog.Write);
            writeLogManager.WriteLog(this,new ExceptionEventArgs("Test",new Exception("Exception")));
        }

        internal void Test1()
        {
            WriteLog writeLog = new WriteLog();
            writeLog.WriteDirectly(this,new ExceptionEventArgs("Test1",new Exception("Exception1")));
        }
    }
}

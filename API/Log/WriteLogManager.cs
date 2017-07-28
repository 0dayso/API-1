using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Log
{
    class WriteLogManager:IWriteLogManager
    {
        private event EventHandler<ExceptionEventArgs> EventHandler;


        public void Add(EventHandler<ExceptionEventArgs> eventHandler)
        {
            EventHandler += eventHandler;
        }

        public void Remove(EventHandler<ExceptionEventArgs> eventHandler)
        {
            EventHandler -= eventHandler;
        }

        public void WriteLog(object sender, EventArgs eventArgs)
        {
            EventHandler<ExceptionEventArgs> temp = EventHandler;
            if (temp != null)
                temp(sender, eventArgs as ExceptionEventArgs);
        }

       
    }
}

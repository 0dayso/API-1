using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Log
{
    public class ExceptionEventArgs:EventArgs
    {
        
        public string Message { get; }

        public Exception Exception { get; }

        public ExceptionEventArgs(string message, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }

    }
}

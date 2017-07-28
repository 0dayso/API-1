using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace API.Log
{
    public interface IWriteLogManager
    {
        void Add(EventHandler<ExceptionEventArgs> eventHandler);
        void Remove(EventHandler<ExceptionEventArgs> eventHandler);
        void WriteLog(object sender, EventArgs eventArgs);
    }
}

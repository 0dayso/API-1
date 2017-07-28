using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Log
{
    public interface IWriteLog
    {
        void Write(object sender,EventArgs eventArgs);
    }
}

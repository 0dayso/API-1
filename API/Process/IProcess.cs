using System;

namespace API.Process
{
    public interface IProcess
    {
        void Process(Action<Exception> writeLog,Action adsl);
    }
}

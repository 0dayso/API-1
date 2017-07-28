using System;

namespace API.Process
{
    public interface ITaskProcess
    {
        string TableName { get; set; }

        void Start(string accountId,string message);

        void Succeed(string accountId,string message);

        void Failed(string accountId,Exception exception);

        void UpdateTable(string sql);

    }
}

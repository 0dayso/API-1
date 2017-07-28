using System;
using System.Collections.Generic;
using API.Helper;
using API.Process;

namespace API.Handler
{
    public class TaskDbHandler:ITaskProcess
    {
        protected readonly MySqlHelper Task;
        public virtual string TableName { get; set; }

        protected TaskDbHandler(MySqlHelper task)
        {
            Task = task;
        }

        protected TaskDbHandler(string taskDbServer, string taskDbName, string taskDbUserName, string taskDbPassWord)
        {
            Task = new MySqlHelper(taskDbServer, taskDbName, taskDbUserName, taskDbPassWord);
        }

        /// <summary>
        /// GetSelectDicBySql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IDictionary<string, List<object>> GetSelectDicBySql(string sql)
        {
            return Task.GetSelectDicBySql(sql);
        }


        /// <summary>
        /// GetSelectDicBySqlWithLock
        /// </summary>
        /// <param name="sqlSelect"></param>
        /// <param name="lockName"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public IDictionary<string, List<object>> GetSelectDicBySqlWithLock(string sqlSelect,string lockName,Action<string> start)
        {
            return Task.GetSelectBySqlWithLock(sqlSelect, lockName,start);
        }


        public void Start(string accountId,string message="查询中")
        {
            string sql = $"UPDATE {TableName} SET taskstatue = 2,taskmessage = '{message}',taskupdatetime = NOW() WHERE account_id = {accountId}";
            UpdateTable(sql);
        }

        public void Succeed(string accountId, string message= "查询成功")
        {
            string sql = $"UPDATE {TableName} SET taskstatue = 5,taskmessage = '{message}',taskupdatetime = NOW() WHERE account_id = {accountId}";
            UpdateTable(sql);
        }

        public void Failed(string accountId, Exception exception)
        {
            string exceptionMessage = exception == null ? "查询失败" : $"{exception.Message}{Environment.NewLine}{exception.StackTrace}";
            string sql = $"UPDATE {TableName} SET taskstatue = 6,taskmessage = '{exceptionMessage}',taskupdatetime = NOW() WHERE account_id = {accountId}";
            UpdateTable(sql);
        }

        public void UpdateTable(string sql)
        {
            Task.UpdateTable(sql);
        }
    }
}

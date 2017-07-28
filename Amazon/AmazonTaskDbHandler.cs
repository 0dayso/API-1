using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Handler;
using API.Helper;

namespace Amazon
{
    internal class AmazonTaskDbHandler:TaskDbHandler
    {
        public override string TableName { get; set; } = "amazon_mws_key";

        public AmazonTaskDbHandler(MySqlHelper task) : base(task)
        {
        }

        public AmazonTaskDbHandler(string taskDbServer, string taskDbName, string taskDbUserName, string taskDbPassWord) : base(taskDbServer, taskDbName, taskDbUserName, taskDbPassWord)
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;
using API.Handler;
using API.Helper;

namespace AliExpress
{
    internal class AliExpressTaskDbHandler:TaskDbHandler
    {
        //public override string TableName { get; set; }

        public AliExpressTaskDbHandler(MySqlHelper task) : base(task)
        {
            
        }

        public AliExpressTaskDbHandler(string taskDbServer, string taskDbName, string taskDbUserName, string taskDbPassWord) : base(taskDbServer, taskDbName, taskDbUserName, taskDbPassWord)
        {
            
        }

    }
}

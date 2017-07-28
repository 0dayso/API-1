using System.Collections.Generic;
using API.Helper;

namespace API.Handler
{
    public abstract class ResultDbHandler
    {
        
        protected readonly MySqlHelper ResultHelper;

        protected ResultDbHandler(MySqlHelper resultHelper)
        {
            ResultHelper = resultHelper;
        }

        
        protected ResultDbHandler(string resultDbServer, string resultDbName, string resultDbUserName, string resultDbPassWord)
        {
            ResultHelper = new MySqlHelper(resultDbServer, resultDbName, resultDbUserName, resultDbPassWord);
        }



        /// <summary>
        /// CreateTable
        /// </summary>
        /// <param name="tableName"></param>
        public abstract void CreateTable(string tableName);



        /// <summary>
        /// TableIfExists
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool TableIfExists(string tableName)
        {
            string sql = $"SELECT COUNT(*) as number FROM information_schema.TABLES WHERE table_Name = '{tableName}'";
            var value = ResultHelper.GetSelectOneValue(sql, "number");
            return value == "1";
        }


        /// <summary>
        /// GetEndDate
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetEndDate(string tableName,string fieldName)
        {
            var sql = $"SELECT MAX({fieldName}) as max{fieldName} FROM {tableName}";
            return ResultHelper.GetSelectOneValue(sql, $"max{fieldName}");
        }


        /// <summary>
        /// InsertTableWithDicExistsUpdate
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="tableName"></param>
        public void InsertTableWithDicExistsUpdate(IDictionary<string, object> dic, string tableName)
        {
            ResultHelper.InsertTableWithDicExistsUpdate(dic, tableName);
        }


        /// <summary>
        /// OrderImportSuccessfullyExistsUpdate
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="orderLastDate"></param>
        /// <param name="status"></param>
        /// <param name="updateTime"></param>
        /// <param name="message"></param>
        public void OrderImportSuccessfullyExistsUpdate(string shopId, string orderLastDate, string status, string updateTime, string message)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>
            {
                {"shopId",shopId },
                {"orderLastDate",orderLastDate },
                {"status",status },
                {"updateTime",updateTime },
                {"message",message }
            };

            //data_customer_order_log
            ResultHelper.InsertTableWithDicExistsUpdate(dic, "data_customer_order_log");
        }
    }
}

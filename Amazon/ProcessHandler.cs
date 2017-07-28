using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API.Extensions;
using API.Handler;
using API.Helper;
using API.Process;
using Newtonsoft.Json.Linq;

namespace Amazon
{
    class ProcessHandler:IProcess
    {
        public void Process(Action<Exception> writeLog,Action adsl)
        {
            Func<DateTime,string> dateTimeToIso8601 = dateTime => dateTime.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);

            const string taskTableName = "amazon_mws_key";
            MySqlHelper taskHelper = new MySqlHelper("127.0.0.1", "zhanxian", "root", "root");
            MySqlHelper resultHelper = new MySqlHelper("127.0.0.1", "zhanxian", "root", "root");
            TaskDbHandler taskDbHandler = new AmazonTaskDbHandler(taskHelper);
            //设置任务表名
            taskDbHandler.TableName = taskTableName;

            ResultDbHandler resultDbHandler = new AmazonResultDbHandler(resultHelper);

            string accountId = null;

            try
            {
                //var dic =
                //    taskDbHandler.GetSelectDicBySql(
                //        $"SELECT account_id,sellerId,awsAccessKeyId,secertKey,marketPlace,queryDateSpan FROM {taskTableName} WHERE taskstatue = 1");

                //取任务为1的
                var dic =
                    taskDbHandler.GetSelectDicBySqlWithLock(
                        $"SELECT account_id,sellerId,awsAccessKeyId,secertKey,marketPlace,queryDateSpan FROM {taskTableName} WHERE taskstatue = 1 ORDER BY id LIMIT 1",
                        "key_db.amazon_mws_key.taskStatueEqual1",
                        id => taskDbHandler.Start(id));
                dic.Print();
                ICollection<string> keys = dic.Keys;
                int rowCount = dic.GetRowCount();

                //如果没有任务为1的 取任务为6的
                if (rowCount == 0)
                {
                    dic = taskDbHandler.GetSelectDicBySqlWithLock(
                        $"SELECT account_id,sellerId,awsAccessKeyId,secertKey,marketPlace,queryDateSpan FROM {taskTableName} WHERE taskstatue = 6 ORDER BY id LIMIT 1",
                        "key_db.amazon_mws_key.taskStatueEqual6", 
                        id => taskDbHandler.Start(id));
                    dic.Print();
                    keys = dic.Keys;
                    rowCount = dic.GetRowCount();
                }

                for (int i = 0; i < rowCount; i++)
                {
                    try
                    {
                        IDictionary<string, object> curDic = new Dictionary<string, object>();
                        foreach (var key in keys)
                        {
                            curDic.Add(key, dic[key][i]);
                        }
                        curDic.Print();

                        accountId = curDic["account_id"].ToString();

                        string sellerId = curDic["sellerId"].ToString();
                        string accessKey = curDic["awsAccessKeyId"].ToString();
                        string secretKey = curDic["secertKey"].ToString();
                        string marketPlace = curDic["marketPlace"].ToString();
                        int queryDateSpan = int.Parse(curDic["queryDateSpan"].ToString());
                        string tableName = $"order_{accountId}";
                        ////设置任务表名
                        //taskDbHandler.TableName = taskTableName;
                        ////任务表设置开始状态
                        //taskDbHandler.Start(accountId);
                        OrderListHandler orderListHandler = new OrderListHandler(marketPlace, accessKey, sellerId,
                            secretKey);
                        string createAfter;
                        if (resultDbHandler.TableIfExists(tableName))
                        {
                            string purchaseDate = resultDbHandler.GetEndDate(tableName, "purchaseDate");
                            if (string.IsNullOrEmpty(purchaseDate))
                                createAfter = dateTimeToIso8601(DateTime.Now.AddDays(-queryDateSpan));
                            else
                                createAfter = dateTimeToIso8601(DateTime.Parse(purchaseDate));
                        }
                        else
                        {
                            resultDbHandler.CreateTable(tableName);
                            createAfter = dateTimeToIso8601(DateTime.Now.AddDays(-queryDateSpan));
                        }


                        bool isFirstPage = true;
                        //第一次设置不为空的任意值
                        string nextToken = "We Will See";
                        while (!string.IsNullOrEmpty(nextToken))
                        {
                            //var parameterDic = isFirstPage
                            //    ? orderListHandler.GetParameterDic(createAfter)
                            //    : orderListHandler.GetNextTokenParameterDic(nextToken);

                            //string html = orderListHandler.GetOrderListHtml(parameterDic);

                            string html = GetHtmlByPostTryThreeTimes(isFirstPage, createAfter, nextToken, orderListHandler,writeLog);

                            nextToken = isFirstPage
                                ? JObject.Parse(html)["ListOrdersResponse"]?["ListOrdersResult"]?["NextToken"]?.ToString
                                    ()
                                : JObject.Parse(html)["ListOrdersByNextTokenResponse"]?["ListOrdersByNextTokenResult"]?[
                                    "NextToken"]?.ToString();

                            orderListHandler.GetMainInfoDicList(html, infoDic =>
                            {
                                resultHelper.InsertTableWithDicExistsUpdate(infoDic, tableName);
                            }, isFirstPage);
                            //翻页休息1分钟
                            Thread.Sleep(60*1000 + new Random().Next(5000));
                            if (isFirstPage)
                                isFirstPage = false;
                        }

                        //订单数据导入成功操作
                        resultDbHandler.OrderImportSuccessfullyExistsUpdate(accountId,
                            Convert.ToDateTime(createAfter).ToString("yyyy/MM/dd hh:mm:ss"), "0",
                            DateTime.Now.ToString(CultureInfo.CurrentCulture), "成功");
                        //任务表设置成功状态
                        taskDbHandler.Succeed(accountId);
                    }
                    catch (Exception e)
                    {
                        //任务表设置失败状态
                        taskDbHandler.Failed(accountId, e);
                        //写错误日志
                        writeLog?.Invoke(e);
                    }
                    //adsl换ip
                    adsl?.Invoke();
                }

            }
            catch (Exception exception)
            {
                //写错误日志
                writeLog?.Invoke(exception);
            }

        }



        private string GetHtmlByPostTryThreeTimes(bool isFirstPage,string createAfter,string nextToken,OrderListHandler orderListHandler, Action<Exception> writeLog)
        {

            int tryTimes = 0;
            string html = null;
            bool isException = true;
            //设置为4次
            int maxTryTimes = 4;

            while (tryTimes <= maxTryTimes && isException)
            {
                tryTimes++;
                try
                {
                    var parameterDic = isFirstPage
                        ? orderListHandler.GetParameterDic(createAfter)
                        : orderListHandler.GetNextTokenParameterDic(nextToken);

                    html = orderListHandler.GetOrderListHtml(parameterDic);

                    isException = false;
                }
                catch (Exception e)
                {
                    
                    isException = true;
                    if (tryTimes > maxTryTimes)
                        throw;
                    Console.WriteLine($"GetHtmlByPostTryThreeTimes : {tryTimes}");
                    Exception exception = new Exception($"tryTimes:{tryTimes * 5} mins",e);
                    writeLog?.Invoke(exception);
                    Thread.Sleep(tryTimes * 5 * 1000 * 60);
                }
            }
            return html;
        }


        private void Test()
        {
            Func<DateTime, string> dateTimeToIso8601 = dateTime => dateTime.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);

            var createAfter = dateTimeToIso8601(DateTime.Now.AddDays(-600));
        }
    }
}

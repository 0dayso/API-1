using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API;
using API.ADSL;
using API.Extensions;
using API.Handler;
using API.Helper;
using API.Process;

namespace AliExpress
{
    internal class ProcessHandler:IProcess
    {

        /// <summary>
        /// Process
        /// </summary>
        public void Process(Action<Exception> writeLog,Action adsl)
        {

            Process(writeLog, adsl ,"aliexpress_oauth_token");
            Process(writeLog, adsl ,"aliexpress_oauth_token_365");

            //Task[] task = new Task[2];

            //task[0] = Task.Factory.StartNew(() => Process(writeLog, adsl ,"aliexpress_oauth_token"));
            //task[1] = Task.Factory.StartNew(() => Process(writeLog, adsl ,"aliexpress_oauth_token_365"));

            //Task.WaitAll(task);
        }


        private void Process(Action<Exception> writeLog,Action adsl,string taskTableName)
        {
           
            MySqlHelper taskHelper = new MySqlHelper("127.0.0.1", "zhanxian", "root", "root");
            MySqlHelper resultHelper = new MySqlHelper("127.0.0.1", "zhanxian", "root", "root");
            TaskDbHandler taskDbHandler = new AliExpressTaskDbHandler(taskHelper);
            //设置任务表名
            taskDbHandler.TableName = taskTableName;
            
            ResultDbHandler resultDbHandler = new AliExpressResultDbHandler(resultHelper);
            string accountId = null;
            try
            {
                //var dic =
                //    taskDbHandler.GetSelectDicBySql(
                //        $"SELECT account_id,client_id,client_secret,refresh_token,queryDateSpan FROM {taskTableName} WHERE taskstatue = 1");


                var dic =
                    taskDbHandler.GetSelectDicBySqlWithLock(
                        $"SELECT account_id,client_id,client_secret,refresh_token,queryDateSpan FROM {taskTableName} WHERE taskstatue = 1 ORDER BY id LIMIT 1",
                        $"key_db.{taskTableName}.taskStatueEqual1",
                        id=> taskDbHandler.Start(id)
                        );
                //dic输出
                dic.Print();
                ICollection<string> keys = dic.Keys;
                int rowCount = dic.GetRowCount();

                if (rowCount == 0)
                {
                    dic =
                    taskDbHandler.GetSelectDicBySqlWithLock(
                        $"SELECT account_id,client_id,client_secret,refresh_token,queryDateSpan FROM {taskTableName} WHERE taskstatue = 6 ORDER BY id LIMIT 1",
                        $"key_db.{taskTableName}.taskStatueEqual6",
                        id => taskDbHandler.Start(id)
                        );
                    //dic输出
                    dic.Print();
                    keys = dic.Keys;
                    rowCount = dic.GetRowCount();
                }


                //数据库表一行一行处理
                for (var i = 0; i < rowCount; i++)
                {
                    try
                    {
                        var curDic = new Dictionary<string, object>();
                        foreach (var key in keys)
                        {
                            curDic.Add(key, dic[key][i]);
                        }
                        curDic.Print();
                        accountId = curDic["account_id"].ToString();

                        string clientId = curDic["client_id"].ToString();
                        string clientSecret = curDic["client_secret"].ToString();
                        string refreshToken = curDic["refresh_token"].ToString();
                        string queryDateSpan = curDic["queryDateSpan"].ToString();
                        string tableName = $"order_{accountId}";
                        ////设置任务表名
                        //taskDbHandler.TableName = taskTableName;
                        ////任务表设置开始状态
                        //taskDbHandler.Start(accountId);
                        string startDate;
                        string endDate;
                        //表存在处理
                        if (resultDbHandler.TableIfExists(tableName))
                        {
                            //读出lastDate，不存在则设置为当前时间
                            string lastDateInDb = resultDbHandler.GetEndDate(tableName, "gmtCreate");
                            endDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
                            if (string.IsNullOrEmpty(lastDateInDb))
                            {
                                startDate = GetDateTimeByDateSpan(endDate, queryDateSpan);
                            }
                            else
                            {
                                //var endDate = "20120801154220368+0800";
                                string lastDateFormat = Regex.Match(lastDateInDb, @"\d{14}").Value;
                                DateTime dateTime = DateTime.ParseExact(lastDateFormat, "yyyyMMddHHmmss",
                                    CultureInfo.CurrentCulture);
                                startDate = dateTime.ToString("MM/dd/yyyy hh:mm:ss");
                            }
                        }
                        //表不存在处理
                        else
                        {
                            //创建表
                            resultDbHandler.CreateTable(tableName);
                            endDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
                            startDate = GetDateTimeByDateSpan(endDate, queryDateSpan);
                        }


                        OrderListHandler orderListHandler = new OrderListHandler(clientId, clientSecret, refreshToken);
                        var urlList = orderListHandler.GetFindOrderListQueryUrlIList(startDate, endDate);
                        foreach (var url in urlList)
                        {
                            HttpHelper httpHelper = new HttpHelper();
                            string html = httpHelper.GetHtmlByGet(url);

                            IList<IDictionary<string, object>> list = orderListHandler.GetMainInfoDicList(html,
                                infoDic =>
                                {
                                    resultDbHandler.InsertTableWithDicExistsUpdate(infoDic, tableName);
                                });
                        }

                        //订单数据导入成功操作
                        resultDbHandler.OrderImportSuccessfullyExistsUpdate(accountId,
                            Convert.ToDateTime(startDate).ToString("yyyy/MM/dd hh:mm:ss"), "0",
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



        /// <summary>
        /// GetDateTimeByDateSpan
        /// </summary>
        /// <param name="curDataTime"></param>
        /// <param name="querDataSpan"></param>
        /// <returns></returns>
        private static string GetDateTimeByDateSpan(string curDataTime, string querDataSpan)
        {
            int queryDataSpanInt;
            if (!int.TryParse(querDataSpan, out queryDataSpanInt))
                throw new Exception("querDataSpan为非日期格式!");
            return DateTime.Parse(curDataTime).AddDays(-int.Parse(querDataSpan)).ToString("MM/dd/yyyy hh:mm:ss");
        }

        private void Test()
        {
            //yyyyMMddHHmmssSSSZ
            //20120801154220368+0800
            var endDate = "20120801154220368+0800";
            endDate = Regex.Match(endDate, @"\d{14}").Value;
            DateTime dateTime = DateTime.ParseExact(endDate, "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
            endDate = dateTime.ToString("MM/dd/yyyy hh:mm:ss");

            string now = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
            var beforeTime = GetDateTimeByDateSpan(now, "180");
        }
    }
}

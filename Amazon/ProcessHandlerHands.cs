//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using API;
//using API.Extensions;
//using API.Handler;
//using API.Helper;
//using API.Process;
//using Newtonsoft.Json.Linq;

//namespace Amazon
//{
//    class ProcessHandlerHands:IProcess
//    {







//        public void Process(Action<Exception> action)
//        {
//            string sellerId = "ALM27M18VEIRS";
//            string tableName = $"order_{sellerId}";
//            MySqlHelper taskHelper = new MySqlHelper();
//            MySqlHelper resultHelper = new MySqlHelper();
//            ResultDbHandler resultDbHandler = new AmazonResultDbHandler(resultHelper);
//            TaskDbHandler taskDbHandler = new AmazonTaskDbHandler(taskHelper);
//            OrderListHandler orderListHandler = new OrderListHandler("加拿大", new List<string> { "ATVPDKIKX0DER" }, "AKIAJTPVS26NBX7CKCGA", "ALM27M18VEIRS", "0f5708dECCx680pEVPFq40bNg4SLz3L0cb9lYpqm");
//            IDictionary<string, string> dic = orderListHandler.GetParameterDic("2017-02-28T16:00:00Z");
//            string html = orderListHandler.GetOrderListHtml(dic);
//            if(!resultDbHandler.TableIfExists(tableName))
//                resultDbHandler.CreateTable(tableName);
//            orderListHandler.GetMainInfoDicList(html, infoDic =>
//            {
//                resultDbHandler.InsertTableWithDicExistsUpdate(infoDic,tableName);
//            },true);
//        }


//        public void Process(string areaName,IList<string> marketPlaceIds,string accessKey,string sellerId,string secretKey)
//        {
//            string tableName = $"order_{sellerId}";
//            MySqlHelper taskHelper = new MySqlHelper();
//            MySqlHelper resultHelper = new MySqlHelper("211.149.229.28", "data_amazon", "amazon_g", "B8493nxs");
//            ResultDbHandler amazonDbHandler = new AmazonResultDbHandler(resultHelper);
//            OrderListHandler orderListHandler = new OrderListHandler(areaName, marketPlaceIds, accessKey, sellerId, secretKey);
//            string createAfter;
//            if (!amazonDbHandler.TableIfExists(tableName))
//            {
//                amazonDbHandler.CreateTable(tableName);
//                createAfter = DateTime.Now.AddDays(-600).ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
//            }
//            else
//            {
//                string purchaseDate = amazonDbHandler.GetEndDate($"order_{sellerId}", "purchaseDate");
//                if(string.IsNullOrEmpty(purchaseDate))
//                    createAfter = DateTime.Now.AddDays(-600).ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
//                else
//                    createAfter = DateTime.Parse(purchaseDate).ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
//            }
            
//            bool hasNextToken = true;
//            bool isFirstPage = true;
//            string nextToken = null;
//            while (hasNextToken)
//            {
//                IDictionary<string, string> dic = null;
//                if (isFirstPage)
//                {
//                    dic = orderListHandler.GetParameterDic(createAfter);
//                }
//                else
//                {
//                    dic = orderListHandler.GetNextTokenParameterDic(nextToken);
//                }
                
//                string html = orderListHandler.GetOrderListHtml(dic);

//                if (isFirstPage)
//                {
//                    nextToken = JObject.Parse(html)["ListOrdersResponse"]?["ListOrdersResult"]?["NextToken"]?.ToString();
//                }
//                else
//                {
//                    nextToken = JObject.Parse(html)["ListOrdersByNextTokenResponse"]?["ListOrdersByNextTokenResult"]?["NextToken"]?.ToString();
//                }
//                if (string.IsNullOrEmpty(nextToken))
//                    hasNextToken = false;
                
//                orderListHandler.GetMainInfoDicList(html, infoDic =>
//                {
//                    amazonDbHandler.InsertTableWithDicExistsUpdate(infoDic, tableName);
//                }, isFirstPage);
//                //翻页休息1分钟
//                Thread.Sleep(60*1000);
//                if (isFirstPage)
//                    isFirstPage = false;
//            }
           
//        }


//        private void Test()
//        {
//            //string createAfter = DateTime.Now.AddDays(-180).ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo);
//            //string createAfter2 = DateTime.Now.AddDays(-180).ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.CurrentInfo);
//            Process("加拿大",new List<string>() { "A2EUQ1WTGCTBG2", "ATVPDKIKX0DER"}, "AKIAJJ3IGOXQ5HO4TKUQ", "A3JT9N5IAS99RQ", "bOq0cOiUzKl8Sx4FriWVVJmNKBo0ptR2pZyBkODA");
//        }

//        private void TestMultiple()
//        {
//            IList<string> areaNameList = new List<string>()
//            {
//                "加拿大","英国","日本","加拿大","英国","加拿大","英国","日本","加拿大","英国","日本","加拿大","英国","日本","加拿大","英国","日本"
//            };

//            IList<List<string>> marketPlaceIdsList = new List<List<string>>()
//            {
//                new List<string>() {"A2EUQ1WTGCTBG2","ATVPDKIKX0DER","A1AM78C64UM0Y8"},
//                new List<string>() {"A1F83G8C2ARO7P","A1PA6795UKMFR9","A1RKKUPIHCS9HS","A13V1IB3VIYZZH","APJ6JRA9NG5V4"},
//                new List<string>() {"A1VC38T7YXB528"},
//                new List<string>() {"A2EUQ1WTGCTBG2","ATVPDKIKX0DER"},
//                new List<string>() {"A1F83G8C2ARO7P", "A1PA6795UKMFR9", "A1RKKUPIHCS9HS", "A13V1IB3VIYZZH", "APJ6JRA9NG5V4"},
//                new List<string>() {"A2EUQ1WTGCTBG2","ATVPDKIKX0DER"},
//                new List<string>() { "A1F83G8C2ARO7P", "A1PA6795UKMFR9", "A1RKKUPIHCS9HS", "A13V1IB3VIYZZH", "APJ6JRA9NG5V4"},
//                new List<string>() { "A1VC38T7YXB528"},
//                new List<string>() { "A2EUQ1WTGCTBG2", "ATVPDKIKX0DER" },
//                new List<string>() { "A1F83G8C2ARO7P", "A1PA6795UKMFR9", "A1RKKUPIHCS9HS", "A13V1IB3VIYZZH", "APJ6JRA9NG5V4" },
//                new List<string>() { "A1VC38T7YXB528"},
//                new List<string>() { "A2EUQ1WTGCTBG2","ATVPDKIKX0DER" },
//                new List<string>() { "A1F83G8C2ARO7P", "A1PA6795UKMFR9", "A1RKKUPIHCS9HS", "A13V1IB3VIYZZH", "APJ6JRA9NG5V4" },
//                new List<string>() { "A1VC38T7YXB528" },
//                new List<string>() { "A2EUQ1WTGCTBG2", "ATVPDKIKX0DER", "A1AM78C64UM0Y8"},
//                new List<string>() { "A1F83G8C2ARO7P", "A1PA6795UKMFR9", "A1RKKUPIHCS9HS", "A13V1IB3VIYZZH", "APJ6JRA9NG5V4" },
//                new List<string>() { "A1VC38T7YXB528" }
//            };

//            IList<string> accessKeyList = new List<string>()
//            {
//                "AKIAJTPVS26NBX7CKCGA","AKIAICZK6OCVLYSVUZ3A","AKIAJQ57LVA3Z2OTW3SQ","AKIAIPL7UVWH2MH46YBQ",
//                "AKIAIL7XY7MU5NNBWCYQ","AKIAJZIBS7SSIWCF3KKA","AKIAIDPZPF6DIQO5U6FQ","AKIAJNB34QCRHLAVHN7A","AKIAJJ3IGOXQ5HO4TKUQ",
//                "AKIAJVCFYZEVO2MKR2LQ","AKIAIP56HXN6NDX5SOYA","AKIAITLN2T77AA5VQZJA","AKIAJUE4HDY5CCVECS7Q","AKIAJEP7KUUHZE7U2J4Q",
//                "AKIAIB5IIHB3LJ5ZFGEA","AKIAJKPGT4T2YSXHWR6Q","AKIAJD7PONGC5ETD47PA"
//            };


//            IList<string> sellerIdList = new List<string>()
//            {
//                "ALM27M18VEIRS","A126FTQNKHPAFV","A257CCE8HK83O1","AHZBVQMXH3MLJ","A3S5Q0X9ZJGCUV",
//                "A3D9FSH17UXSCR","A30WED6ALFUO8D","A21GFNZLDXVLXY","A3JT9N5IAS99RQ","AWQQIWLKT25A",
//                "A5TK09G4AZV5E","A36CN0T0KJUWP7","AEW21JAY7L5E","A1ESNZVKMJ0EOG","A3L1UZN596J714",
//                "A24YL1H66HR6UM","A2H848K6PGGK87"
//            };

//            IList<string> secretKeyList = new List<string>()
//            {
//                "0f5708dECCx680pEVPFq40bNg4SLz3L0cb9lYpqm","BnO7I3SEASNuMFSwRYSohP4kwfyPEcAlISRBKQX0","VpfarM313vEbAEE+8vNikl7g4nZSR2hAIXV0wRa9","QW7tJGa889PdAbM1zn443iFeu4SjWfjfJurZwrTN","lq2UBuLd8Z7z+BvpTZYGQYzeBHayPAu8kPBTIFg/",
//                "0Fcq/U5fhmUkqj38MVtWrxI7aPxWFL5AJG1lmzZw","KzMl3uwDlGNZGw3NIBh1R256WUVNYnh9wm7W9Guc","QL5SRI0KTB4domYGgJuRY7sQ6vvae7yUXHJOnOoU","bOq0cOiUzKl8Sx4FriWVVJmNKBo0ptR2pZyBkODA","qU/RT/gnMUjJz3+TfXJ4OdAF71AW8oIZCanxuECv",
//                "YAc7uaHxaWsy0daScGlSgZbW6B0zKdFfTkFgzgYu","rek5h89N+6Z6zIZalLnDMthXRk2+nvAOMYDqRU6Z","lkrcXRRIyw9k/jWcAc0rwgZm+olRSqte9bInH7pB","Jyw9oAv4OHrpMTqcYAJX3hwzeUNlwkpqFC/NwXk9","2Js9A7TIIhzsH5WkaCuIBPO+GrXdkO6CrREzUxs4",
//                "dAt6fPC/sFsXqhT1f1S/dThB2Z8BuYu+RcfEN8SI","jRTKZziV0Az1/B0VAVm9UL67F5cDDDe64hMey72k"
//            };

//            //Process("加拿大",new List<string>() { "A2EUQ1WTGCTBG2", "ATVPDKIKX0DER", "A1AM78C64UM0Y8" }, "AKIAJTPVS26NBX7CKCGA", "ALM27M18VEIRS", "0f5708dECCx680pEVPFq40bNg4SLz3L0cb9lYpqm");
//            //Process("英国", new List<string>() { "A1F83G8C2ARO7P", "A1PA6795UKMFR9", "A1RKKUPIHCS9HS", "A13V1IB3VIYZZH", "APJ6JRA9NG5V4" }, "AKIAICZK6OCVLYSVUZ3A", "A126FTQNKHPAFV", "BnO7I3SEASNuMFSwRYSohP4kwfyPEcAlISRBKQX0");


//            var count = areaNameList.Count;

//            //tasks multiple
//            Task[] tasks = new Task[count];
//            for (int i = 0; i < count; i++)
//            {
//                int curIndex = i;
//                tasks[i] = new Task(() =>
//                {
//                    try
//                    {
//                        Process(areaNameList[curIndex], marketPlaceIdsList[curIndex], accessKeyList[curIndex],
//                            sellerIdList[curIndex], secretKeyList[curIndex]);
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine($"exception:sellerId:{sellerIdList[curIndex]}。{Environment.NewLine}Message:{e.Message}。{Environment.NewLine}StackTrace:{e.StackTrace}");
//                    }
//                });
//                tasks[i].Start();
//            }

//            for (int i = 0; i < count; i++)
//            {
//                tasks[i].Wait();
//            }


//            //threads multiple
//            //Thread[] threads = new Thread[count];
//            //for (int i = 0; i < count; i++)
//            //{
//            //    int curIndex = i;
//            //    threads[i] = new Thread(() =>
//            //    {
//            //        try
//            //        {
//            //            Process(areaNameList[curIndex], marketPlaceIdsList[curIndex], accessKeyList[curIndex],
//            //                sellerIdList[curIndex], secretKeyList[curIndex]);
//            //        }
//            //        catch (Exception e)
//            //        {
//            //            throw new Exception($"sellerId:{sellerIdList[curIndex]}。{Environment.NewLine}Message:{e.Message}。{Environment.NewLine}StackTrace:{e.StackTrace}");
//            //        }
//            //    });
//            //    threads[i].Start();
//            //}

//            //for (int i = 0; i < count; i++)
//            //{
//            //    threads[i].Join();
//            //}

//        }




//    }
//}

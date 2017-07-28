using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using API;
using API.Extensions;
using API.Helper;
using Newtonsoft.Json.Linq;


namespace AliExpress
{
    internal class OrderListHandler
    {
        
        private readonly string _appKey;
        private readonly string _appSecret;
        private readonly string _refreshToken;
        



        public OrderListHandler(string appKey, string appSecret, string refreshToken)
        {
            _appKey = appKey;
            _appSecret = appSecret;
            _refreshToken = refreshToken;
        }

        






        private void Test()
        {
            //GetAuthUrl("39652332", "HBMfdNiBUj", "http://localhost:80");

            var accessToken = GetAccessToken("30155762", "Gm7PeLH6ZBRR", "5366be7b-97ff-4285-85ca-7c37d6a49ae5");

            var urlList = GetFindOrderListQueryUrlList("30155762", "Gm7PeLH6ZBRR", accessToken);
            foreach (var url in urlList)
            {
                HttpHelper httpHelper = new HttpHelper();
                var html = httpHelper.GetHtmlByGet(url);
                Console.Write(html);
                GetMainInfoDicList(html);
            }
            //var url = GetFindOrderListQueryUrl("30155762", "Gm7PeLH6ZBRR", 1, 50, accessToken);
        }


        private void Test1()
        {
            OrderListHandler orderListHandler = new OrderListHandler("30155762", "Gm7PeLH6ZBRR", "5366be7b-97ff-4285-85ca-7c37d6a49ae5");
            var accessToken = GetAccessToken(orderListHandler._appKey, orderListHandler._appSecret, orderListHandler._refreshToken);
            var urlList = orderListHandler.GetFindOrderListQueryUrlList(orderListHandler._appKey, orderListHandler._appSecret, accessToken);
            foreach (var url in urlList)
            {
                HttpHelper httpHelper = new HttpHelper();
                var html = httpHelper.GetHtmlByGet(url);
                orderListHandler.GetMainInfoDicList(html);
            }
        }


        


        /// <summary>
        /// GetMainInfoDicList
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IList<Dictionary<string, string>> GetMainInfoDicList(string html)
        {
            IList<Dictionary<string,string>> dicList = new List<Dictionary<string, string>>();
            JObject jObject = JObject.Parse(html);
            JArray jArray = JArray.Parse(jObject["orderList"].ToString());
            foreach (var jToken in jArray)
            {
                var amount = jToken["payAmount"]["amount"]?.ToString();
                var cent = jToken["payAmount"]["cent"]?.ToString();
                var currencyCode = jToken["payAmount"]["currencyCode"]?.ToString();
                var sellerSignerFullname = jToken["sellerSignerFullname"]?.ToString();
                var buyerLoginId = jToken["buyerLoginId"]?.ToString();
                var paymentType = jToken["paymentType"]?.ToString();
                var orderStatus = jToken["orderStatus"]?.ToString();
                var orderId = jToken["orderId"]?.ToString();
                var issueStatus = jToken["issueStatus"]?.ToString();
                var sendGoodsTime = jToken["gmtSendGoodsTime"]?.ToString();
                var gmtPayTime = jToken["gmtPayTime"]?.ToString();
                var gmtCreate = jToken["gmtCreate"]?.ToString();
                var fundStatus = jToken["fundStatus"]?.ToString();
                var frozenStatus = jToken["frozenStatus"]?.ToString();
                var loanAmount = jToken["loanAmount"]?.ToString();
                var escrowFee = jToken["escrowFee"]?.ToString();
                var buyerSignerFullname = jToken["buyerSignerFullname"]?.ToString();
                var bizType = jToken["bizType"]?.ToString();
                //var freightCommitDay = jToken["logisticsServiceName"]["freightCommitDay"]?.ToString();
                var productList = jToken["productList"].ToString();

                Dictionary<string,string> dic = new Dictionary<string, string>
                {
                    {"amount",amount },
                    {"cent",cent },
                    {"currencyCode",currencyCode },
                    {"sellerSignerFullname",sellerSignerFullname },
                    {"buyerLoginId",buyerLoginId },
                    {"paymentType",paymentType },
                    {"orderStatus",orderStatus },
                    {"orderId",orderId },
                    {"issueStatus",issueStatus },
                    {"sendGoodsTime",sendGoodsTime },
                    {"gmtPayTime",gmtPayTime },
                    {"gmtCreate",gmtCreate },
                    {"fundStatus",fundStatus },
                    {"frozenStatus",frozenStatus },
                    {"loanAmount",loanAmount },
                    {"escrowFee",escrowFee },
                    {"buyerSignerFullname",buyerSignerFullname },
                    {"bizType",bizType },
                    {"productList",productList }
                };


                dic.Print();

                MySqlHelper mySqlHelper = new MySqlHelper();
                mySqlHelper.InsertTableWithDic(dic, "aliexpressOrderList1");

                dicList.Add(dic);
            }

            return dicList;
        }



        /// <summary>
        /// insetTable
        /// </summary>
        /// <param name="html"></param>
        /// <param name="insetTable"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> GetMainInfoDicList(string html,Action<IDictionary<string,object>> insetTable)
        {
            IList<IDictionary<string, object>> dicList = new List<IDictionary<string, object>>();
            JObject jObject = JObject.Parse(html);
            JArray jArray = JArray.Parse(jObject["orderList"].ToString());
            foreach (var jToken in jArray)
            {
                var amount = jToken["payAmount"]["amount"]?.ToString();
                var cent = jToken["payAmount"]["cent"]?.ToString();
                var currencyCode = jToken["payAmount"]["currencyCode"]?.ToString();
                var sellerSignerFullname = jToken["sellerSignerFullname"]?.ToString();
                var buyerLoginId = jToken["buyerLoginId"]?.ToString();
                var paymentType = jToken["paymentType"]?.ToString();
                var orderStatus = jToken["orderStatus"]?.ToString();
                var orderId = jToken["orderId"]?.ToString();
                var issueStatus = jToken["issueStatus"]?.ToString();
                var sendGoodsTime = jToken["gmtSendGoodsTime"]?.ToString();
                var gmtPayTime = jToken["gmtPayTime"]?.ToString();
                var gmtCreate = jToken["gmtCreate"]?.ToString();
                var fundStatus = jToken["fundStatus"]?.ToString();
                var frozenStatus = jToken["frozenStatus"]?.ToString();
                var loanAmount = jToken["loanAmount"]?.ToString();
                var escrowFee = jToken["escrowFee"]?.ToString();
                var buyerSignerFullname = jToken["buyerSignerFullname"]?.ToString();
                var bizType = jToken["bizType"]?.ToString();
                var productList = jToken["productList"].ToString();

                IDictionary<string, object> dic = new Dictionary<string, object>
                {
                    {"amount",amount },
                    {"cent",cent },
                    {"currencyCode",currencyCode },
                    {"sellerSignerFullname",sellerSignerFullname },
                    {"buyerLoginId",buyerLoginId },
                    {"paymentType",paymentType },
                    {"orderStatus",orderStatus },
                    {"orderId",orderId },
                    {"issueStatus",issueStatus },
                    {"sendGoodsTime",sendGoodsTime },
                    {"gmtPayTime",gmtPayTime },
                    {"gmtCreate",gmtCreate },
                    {"fundStatus",fundStatus },
                    {"frozenStatus",frozenStatus },
                    {"loanAmount",loanAmount },
                    {"escrowFee",escrowFee },
                    {"buyerSignerFullname",buyerSignerFullname },
                    {"bizType",bizType },
                    {"productList",productList }
                };


                dic.Print();
                //调用委托
                insetTable?.Invoke(dic);

                dicList.Add(dic);
            }

            return dicList;
        }



        /// <summary>
        /// GetAuthUrl
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private string GetAuthUrl(string appKey,string appSecret,string redirectUri,string state = "test")
        {
            Dictionary<string,string> dic = new Dictionary<string, string>
            {
                {"client_id",appKey},
                {"site","aliexpress"},
                {"redirect_uri",redirectUri},
                {"state",state},
            };
            var sign = SignatureHandler.Sign(dic, appSecret);
            dic.Add("_aop_signature",sign);
            var authUrl = GetUrlByDic("http://authhz.alibaba.com/auth/authorize.htm",dic);
            return authUrl;
        }


        /// <summary>
        /// GetUrlByDic
        /// </summary>
        /// <param name="partUrl"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private string GetUrlByDic(string partUrl,IDictionary<string,string> dic)
        {
            StringBuilder anotherParturl = new StringBuilder(string.Empty);
            var isFirst = true;
            foreach (var keyValue in dic)
            {
                if (isFirst)
                {

                    anotherParturl.Append($"?{keyValue.Key}={keyValue.Value}");
                    isFirst = false;
                }
                else
                    anotherParturl.Append($"&{keyValue.Key}={keyValue.Value}");
            }
            return $"{partUrl}{anotherParturl}";
        }


        


        /// <summary>
        /// GetAccessToken
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private string GetAccessToken(string appKey,string appSecret,string refreshToken)
        {
            var url = $"https://gw.api.alibaba.com/openapi/param2/1/system.oauth2/getToken/{appKey}";
            var dic = new Dictionary<string,string>
            {
                {"grant_type","refresh_token"},
                {"client_id", appKey},
                {"client_secret",appSecret},
                {"refresh_token",refreshToken}
            };
            var postData = dic.GetPostData();
            HttpHelper httpHelper = new HttpHelper();
            var html = httpHelper.GetHtmlByPost(url, postData);
            var accessToken = Regex.Match(html, "(?<=\"access_token\":\").*(?=\")").Value;
            return accessToken;
        }


        /// <summary>
        /// GetAccessToken
        /// </summary>
        /// <returns></returns>
        private string GetAccessToken()
        {
            return GetAccessToken(_appKey, _appSecret, _refreshToken);
        }

        /// <summary>
        /// GetFindOrderListQueryUrl
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="accessToken"></param>
        /// <param name="createDateStart"></param>
        /// <param name="createDateEnd"></param>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        private string GetFindOrderListQueryUrl(string appKey,string appSecret,int page,int pageSize,string accessToken,string createDateStart =null,string createDateEnd = null,string orderStatus = null)
        {
            var partUrl = $"http://gw.api.alibaba.com:80/openapi/param2/1/aliexpress.open/api.findOrderListQuery/{appKey}";
            var dic = new Dictionary<string,string>
            {
                {"page",page.ToString()},
                {"pageSize",pageSize.ToString()},
                {"access_token",accessToken}
            };

            if(!string.IsNullOrEmpty(createDateStart))
                dic.Add("createDateStart",createDateStart);
            if(!string.IsNullOrEmpty(createDateEnd))
                dic.Add("createDateEnd",createDateEnd);
            if(!string.IsNullOrEmpty(orderStatus))
                dic.Add("orderStatus", orderStatus);

            var sign = SignatureHandler.APISign(Regex.Match(partUrl, "(?<=openapi/).*$").Value, dic, appSecret);
            dic.Add("_aop_signature", sign);
            var url = GetUrlByDic(partUrl, dic);
            return url;
        }



        /// <summary>
        /// GetFindOrderListQueryUrlList
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="accessToken"></param>
        /// <param name="createDateStart"></param>
        /// <param name="createDateEnd"></param>
        /// <param name="orderStatus"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private IList<string> GetFindOrderListQueryUrlList(string appKey, string appSecret, string accessToken,string createDateStart = null, string createDateEnd = null, string orderStatus = null, int pageSize = 50)
        {
            IList<string> list = new List<string>();
            var url = GetFindOrderListQueryUrl(appKey,appSecret,1,pageSize,accessToken,createDateStart,createDateEnd,orderStatus);
            var httpHelper = new HttpHelper();
            var html = httpHelper.GetHtmlByGet(url);
            var totalItemString = Regex.Match(html, @"(?<=""totalItem"":)\d+").Value;
            int totalPage = 0;
            if (!string.IsNullOrEmpty(totalItemString))
            {
                var totalItem = int.Parse(totalItemString);
                totalPage = totalItem%pageSize == 0 ? totalItem/pageSize : totalItem/pageSize + 1;
            }

            for (var i = 0; i < totalPage; i++)
            {
                list.Add(GetFindOrderListQueryUrl(appKey,appSecret,i+1,pageSize,accessToken,createDateStart,createDateEnd,orderStatus));
            }

            return list;
        }


        /// <summary>
        /// GetFindOrderListQueryUrlList
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="createDateStart"></param>
        /// <param name="createDateEnd"></param>
        /// <returns></returns>
        private IList<string> GetFindOrderListQueryUrlIList(string accessToken,string createDateStart,string createDateEnd)
        {
            return GetFindOrderListQueryUrlList(_appKey, _appSecret, accessToken, createDateStart, createDateEnd);
        }



        /// <summary>
        /// GetFindOrderListQueryUrlIList
        /// </summary>
        /// <param name="createDateStart"></param>
        /// <param name="createDateEnd"></param>
        /// <returns></returns>
        public IList<string> GetFindOrderListQueryUrlIList(string createDateStart, string createDateEnd)
        {
            var accessToken = GetAccessToken();
            var urlList = GetFindOrderListQueryUrlIList(accessToken, createDateStart, createDateEnd);
            return urlList;
        }




    }
}

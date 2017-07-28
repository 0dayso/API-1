using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API;
using API.Extensions;
using API.Helper;
using Newtonsoft.Json.Linq;

namespace Amazon
{
    public class OrderListHandler
    {
        private const string PartUrl = "Orders/2013-09-01";
        private const string Algorithm = "HmacSHA256";
        private readonly string _serviceUrl;
        private readonly string _area;
        private readonly string _accessKey;
        private readonly string _sellerId;
        private readonly string _secretKey;
        private readonly IList<string> _marketPlaceIds;


        /// <summary>
        /// OrderListHandler
        /// </summary>
        /// <param name="area"></param>
        /// <param name="maketPlaceIds"></param>
        /// <param name="accessKey"></param>
        /// <param name="sellerId"></param>
        /// <param name="secretKey"></param>
        public OrderListHandler(string area,IList<string> maketPlaceIds, string accessKey, string sellerId, string secretKey)
        {
            _area = area;
            _serviceUrl = AreaTransform.GetUrlByAreaCn(_area);
            _marketPlaceIds = maketPlaceIds;
            _accessKey = accessKey;
            _sellerId = sellerId;
            _secretKey = secretKey;
        }

        /// <summary>
        /// OrderListHandler
        /// </summary>
        /// <param name="maketPlace"></param>
        /// <param name="accessKey"></param>
        /// <param name="sellerId"></param>
        /// <param name="secretKey"></param>
        public OrderListHandler(string maketPlace, string accessKey, string sellerId, string secretKey)
        {
            _area = maketPlace;
            _serviceUrl = AreaTransform.GetUrlByArea(_area);
            _marketPlaceIds = AreaTransform.GetCodeList();
            _accessKey = accessKey;
            _sellerId = sellerId;
            _secretKey = secretKey;
        }

        /// <summary>
        /// GetOrderListHtml
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string GetOrderListHtml(IDictionary<string,string> dic)
        {
            SignatureHandler signatureHandler = new SignatureHandler();
            string postData = signatureHandler.GetPostDataByDic(dic, _secretKey, _serviceUrl, PartUrl);
            string url = $"{_serviceUrl}/{PartUrl}";
            HttpHelper httpHelper = new HttpHelper();
            string html = httpHelper.GetHtmlByPost(url, postData);
            //string html = GetHtmlByPostTryThreeTimes(url, postData);
            return html.XmlToJson();
        }

        private string GetHtmlByPostTryThreeTimes(string url,string postData)
        {
            HttpHelper httpHelper = new HttpHelper();
            int tryTimes = 0;
            string html = null;
            bool isException = true;
            while (tryTimes<=3&&isException)
            {

                tryTimes++;

                try
                {
                    html = httpHelper.GetHtmlByPost(url, postData);
                    isException = false;
                }
                catch (Exception)
                {
                    isException = true;
                    if (tryTimes > 3)
                        throw;
                    Console.WriteLine($"GetHtmlByPost tryTimes {tryTimes}");
                    Thread.Sleep(tryTimes * 5 * 1000 * 60);
                }
            }

            return html;
        }

        public IList<IDictionary<string, object>> GetMainInfoDicList(string html, Action<IDictionary<string, object>> insetTable,bool isFirstPage)
        {
            IList<IDictionary<string, object>> dicList = new List<IDictionary<string, object>>();
            JObject jObject = JObject.Parse(html);
            JArray jArray;
            if (isFirstPage)
                jArray = JArray.Parse(jObject["ListOrdersResponse"]["ListOrdersResult"]["Orders"]["Order"].ToString());
            else
                jArray = JArray.Parse(jObject["ListOrdersByNextTokenResponse"]["ListOrdersByNextTokenResult"]["Orders"]["Order"].ToString());
            foreach (var jToken in jArray)
            {
                var latestShipDate = jToken["LatestShipDate"]?.ToString();
                var orderType = jToken["OrderType"]?.ToString();
                var purchaseDate = jToken["PurchaseDate"]?.ToString();
                var buyerEmail = jToken["BuyerEmail"]?.ToString();
                var amazonOrderId = jToken["AmazonOrderId"]?.ToString();
                var lastUpdateDate = jToken["LastUpdateDate"]?.ToString();
                var isReplacementOrder = jToken["IsReplacementOrder"]?.ToString();
                var shipServiceLevel = jToken["ShipServiceLevel"]?.ToString();
                var numberOfItemsShipped = jToken["NumberOfItemsShipped"]?.ToString();
                var orderStatus = jToken["OrderStatus"]?.ToString();
                var salesChannel = jToken["SalesChannel"]?.ToString();
                var isBusinessOrder = jToken["IsBusinessOrder"]?.ToString();
                var numberOfItemsUnshipped = jToken["NumberOfItemsUnshipped"]?.ToString();
                var paymentMethodDetails = jToken["PaymentMethodDetails"]?.ToString();
                var buyerName = jToken["BuyerName"]?.ToString();
                var orderTotal = jToken["OrderTotal"]?.ToString();
                var isPremiumOrder = jToken["IsPremiumOrder"]?.ToString();
                var earliestShipDate = jToken["EarliestShipDate"]?.ToString();
                var marketplaceId = jToken["MarketplaceId"]?.ToString();
                var fulfillmentChannel = jToken["FulfillmentChannel"]?.ToString();
                var paymentMethod = jToken["PaymentMethod"]?.ToString();
                var shippingAddress = jToken["ShippingAddress"]?.ToString();
                var isPrime = jToken["IsPrime"]?.ToString();
                var shipmentServiceLevelCategory = jToken["ShipmentServiceLevelCategory"]?.ToString();
                var sellerOrderId = jToken["SellerOrderId"]?.ToString();

                IDictionary<string, object> dic = new Dictionary<string, object>
                {
                    {"latestShipDate",latestShipDate },
                    {"orderType",orderType },
                    {"purchaseDate",purchaseDate },
                    {"buyerEmail",buyerEmail },
                    {"amazonOrderId",amazonOrderId },
                    {"lastUpdateDate",lastUpdateDate },
                    {"isReplacementOrder",isReplacementOrder },
                    {"shipServiceLevel",shipServiceLevel },
                    {"numberOfItemsShipped",numberOfItemsShipped },
                    {"orderStatus",orderStatus },
                    {"salesChannel",salesChannel },
                    {"isBusinessOrder",isBusinessOrder },
                    {"numberOfItemsUnshipped",numberOfItemsUnshipped },
                    {"paymentMethodDetails",paymentMethodDetails },
                    {"buyerName",buyerName },
                    {"orderTotal",orderTotal },
                    {"isPremiumOrder",isPremiumOrder },
                    {"earliestShipDate",earliestShipDate },
                    {"marketplaceId",marketplaceId },
                    {"fulfillmentChannel",fulfillmentChannel },
                    {"paymentMethod",paymentMethod },
                    {"shippingAddress",shippingAddress },
                    {"isPrime",isPrime },
                    {"shipmentServiceLevelCategory",shipmentServiceLevelCategory },
                    {"sellerOrderId",sellerOrderId },
                };


                dic.Print();
                //调用委托
                insetTable?.Invoke(dic);

                dicList.Add(dic);
            }

            return dicList;
        }


        public IDictionary<string, string> GetParameterDic(string createAfter,string createdBefore = null)
        {
            IDictionary<string,string> dic = new Dictionary<string, string>();
            dic.Add("AWSAccessKeyId",_accessKey.UrlEncode());
            dic.Add("Action", "ListOrders".UrlEncode());
            dic.Add("SellerId",_sellerId.UrlEncode());
            dic.Add("SignatureMethod",Algorithm.UrlEncode());
            dic.Add("SignatureVersion","2".UrlEncode());
            dic.Add("Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo).UrlEncode());
            dic.Add("Version", "2013-09-01".UrlEncode());
            dic.Add("CreatedAfter", createAfter.UrlEncode());
            if(!string.IsNullOrEmpty(createdBefore))
                dic.Add("CreatedBefore",createdBefore.UrlEncode());
            int i = 0;
            foreach (string marketplaceId in _marketPlaceIds)
            {
                dic.Add($"MarketplaceId.Id.{++i}",marketplaceId);
            }

            return dic;
        }



        public IDictionary<string, string> GetNextTokenParameterDic(string nextToken)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("AWSAccessKeyId", _accessKey.UrlEncode());
            dic.Add("Action", "ListOrdersByNextToken".UrlEncode());
            dic.Add("SellerId", _sellerId.UrlEncode());
            dic.Add("SignatureMethod", Algorithm.UrlEncode());
            dic.Add("SignatureVersion", "2".UrlEncode());
            dic.Add("Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo).UrlEncode());
            dic.Add("Version", "2013-09-01".UrlEncode());
            dic.Add("NextToken", nextToken.UrlEncode());
            return dic;
        }





        //void Test2()
        //{

        //    IDictionary<string, string> dic = new Dictionary<string, string>();
        //    dic.Add("AWSAccessKeyId", UrlEncode("AKIAJTPVS26NBX7CKCGA"));
        //    dic.Add("Action", UrlEncode("ListOrders"));
        //    dic.Add("SellerId", UrlEncode("ALM27M18VEIRS"));
        //    dic.Add("SignatureMethod", UrlEncode("HmacSHA256"));
        //    dic.Add("SignatureVersion", UrlEncode("2"));
        //    //DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo)
        //    dic.Add("Timestamp", UrlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo)));
        //    dic.Add("Version", UrlEncode("2013-09-01"));
        //    dic.Add("CreatedAfter", UrlEncode("2017-02-28T16:00:00Z"));
        //    dic.Add("MarketplaceId.Id.1", UrlEncode("ATVPDKIKX0DER"));
        //    string formarredParameters = CaculateStringToSignV2(dic, "https://mws.amazonservices.ca/", "Orders/2013-09-01");
        //    string signature = Sign(formarredParameters, "0f5708dECCx680pEVPFq40bNg4SLz3L0cb9lYpqm");
        //    dic.Add("Signature", UrlEncode(signature));
        //    Console.WriteLine(signature);
        //    Console.WriteLine(CaculateStringToSignV2(dic, "https://mws.amazonservices.ca/", "Orders/2013-09-01"));

        //    HttpHelper httpHelper = new HttpHelper();
        //    var html = httpHelper.GetHtmlByPost("https://mws.amazonservices.ca/Orders/2013-09-01", dic.GetPostData());
        //    var json = html.XmlToJson();
        //    Console.WriteLine(json);
        //}


    }
}

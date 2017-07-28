using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Runtime.Remoting.Metadata;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using API;
using API.Extensions;
using API.Helper;


namespace Amazon
{
    public class SignatureHandler
    {
        public static readonly Encoding CharacterEncoding = Encoding.UTF8;
        private const string Algorithm = "HmacSHA256";


        /// <summary>
        /// CaculateStringToSignV2
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="partUrl"></param>
        /// <returns></returns>
        private static string CaculateStringToSignV2(IDictionary<string, string> dic, string serviceUrl,string partUrl = null)
        {
            var sortedDic = dic.SortOldVersion<string>();
            //var sortedDic2 = dic.Sort<string,string>();
            //var sortedDic3 = dic.SortOldVersion<string,string>();
            
            Uri endPoint = new Uri(serviceUrl.ToLower());

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("POST\n");
            stringBuilder.Append(endPoint.Host);
            stringBuilder.Append("\n/");
            if (!string.IsNullOrEmpty(partUrl))
                stringBuilder.Append(partUrl);
            stringBuilder.Append("\n");

            IEnumerator<KeyValuePair<string, string>> pairs = sortedDic.GetEnumerator();

            bool isFirst = true;
            while (pairs.MoveNext())
            {
                KeyValuePair<string,string> keyValue = pairs.Current;
                if (isFirst)
                {
                    isFirst = false;
                    stringBuilder.Append($"{keyValue.Key}={keyValue.Value}");
                }
                else
                    stringBuilder.Append($"&{keyValue.Key}={keyValue.Value}");
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// Sign
        /// </summary>
        /// <param name="data"></param>
        /// <param name="secreteKey"></param>
        /// <returns></returns>
        private static string Sign(string data,string secreteKey)
        {
            if(data==null||secreteKey==null)
                throw new Exception("ArgumentNullException");
            //data = data.ToUpper();
            byte[] secretKeyBytes = CharacterEncoding.GetBytes(secreteKey);
            HMACSHA256 hmac = new HMACSHA256(secretKeyBytes);
            hmac.Initialize();
            byte[] bytes = CharacterEncoding.GetBytes(data);
            byte[] rawHmac = hmac.ComputeHash(bytes);
            //foreach (var raw in rawHmac)
            //{
            //    Console.Write($"{raw:X}"); Console.Write(" ");
            //}
            //Console.WriteLine(Convert.ToBase64String(rawHmac));
            return Convert.ToBase64String(rawHmac);
        }


        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        private static string UrlEncode(string rawValue)
        {
            string value = string.IsNullOrEmpty(rawValue) ? string.Empty : rawValue;
            string encoded = null;
            var encode = value.UrlEncodeToUpper(CharacterEncoding);
            if (encode != null)
                encoded = encode.Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
            return encoded;
        }



        /// <summary>
        /// GetPostDataByDic
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="secretKey"></param>
        /// <param name="serviceUrl"></param>
        /// <param name="partUrl"></param>
        /// <returns></returns>
        public string GetPostDataByDic(IDictionary<string,string> dic,string secretKey,string serviceUrl,string partUrl)
        {
            string formarredParameters = CaculateStringToSignV2(dic, serviceUrl, partUrl);
            string signature = Sign(formarredParameters, secretKey);
            dic.Add("Signature", UrlEncode(signature));
            Console.WriteLine(signature);
            return dic.GetPostData();
        }


        private void Test()
        {


            UrlEncode("https://mws.amazonservices.com/a+b*c%7e/");

            IDictionary<string,string> dic = new Dictionary<string, string>
            {
                {"call", "3" },
                {"bitch","2" },
                {"away","1" }
            };


            CaculateStringToSignV2(dic, "https://mws.amazonservices.com/");

            var sortedDic = dic.Sort<string, string>();
            sortedDic.Print();
            Console.WriteLine("next");
            var sortedDic2 = dic.SortOldVersion<string, string>();
            sortedDic2.Print();



        }


        private void Test1()
        {

            string secretKey = "0t7k2NLJzGiTSReiv3ZxY7Pelnv";
            string serviceUrl = "https://mws.amazonservices.com/";
            IDictionary<string,string> dic = new Dictionary<string, string>();
            dic.Add("AWSAccessKeyId", UrlEncode("AKIAJTPVS26NBX7CKCGA"));
            dic.Add("Action", UrlEncode("GetFeedSubmissionList"));
            dic.Add("Merchant", "ALM27M18VEIRS");
            dic.Add("MWSAuthToken", UrlEncode(""));
            dic.Add("SellerId", UrlEncode("ACC53521AK6HM"));
            dic.Add("SignatureMethod", UrlEncode(Algorithm));
            dic.Add("SignatureVersion", UrlEncode("2"));
            dic.Add("SubmittedFromDate", UrlEncode("2017-07-12T03:19:20Z"));
            dic.Add("Timestamp", UrlEncode("2013-05-02T16:00:00Z"));
            dic.Add("Version", UrlEncode("2009-01-01"));

            string formarredParameters = CaculateStringToSignV2(dic, serviceUrl);
            string signature = Sign(formarredParameters, secretKey);
            dic.Add("Signature", UrlEncode(signature));
            Console.WriteLine(signature);
            Console.WriteLine(CaculateStringToSignV2(dic, serviceUrl));

        }


        void Test2()
        {
            
            IDictionary<string,string> dic = new Dictionary<string, string>();
            dic.Add("AWSAccessKeyId", UrlEncode("AKIAJTPVS26NBX7CKCGA"));
            dic.Add("Action", UrlEncode("ListOrders"));
            dic.Add("SellerId", UrlEncode("ALM27M18VEIRS"));
            dic.Add("SignatureMethod", UrlEncode("HmacSHA256"));
            dic.Add("SignatureVersion", UrlEncode("2"));
            //DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo)
            dic.Add("Timestamp", UrlEncode(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzzz", DateTimeFormatInfo.InvariantInfo)));
            dic.Add("Version", UrlEncode("2013-09-01"));
            dic.Add("CreatedAfter", UrlEncode("2017-02-28T16:00:00Z"));
            dic.Add("MarketplaceId.Id.1", UrlEncode("ATVPDKIKX0DER"));
            string formarredParameters = CaculateStringToSignV2(dic, "https://mws.amazonservices.ca/","Orders/2013-09-01");
            string signature = Sign(formarredParameters, "0f5708dECCx680pEVPFq40bNg4SLz3L0cb9lYpqm");
            dic.Add("Signature", UrlEncode(signature));
            Console.WriteLine(signature);
            Console.WriteLine(CaculateStringToSignV2(dic, "https://mws.amazonservices.ca/","Orders/2013-09-01"));

            HttpHelper httpHelper = new HttpHelper();
            var html = httpHelper.GetHtmlByPost("https://mws.amazonservices.ca/Orders/2013-09-01",dic.GetPostData());
            var json = html.XmlToJson();
            Console.WriteLine(json);
        }







    }
}

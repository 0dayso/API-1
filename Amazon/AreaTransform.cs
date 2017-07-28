using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazon
{
    public class AreaTransform
    {

        private static readonly IDictionary<string, string> AreaToUrlCn;
        private static readonly IDictionary<string, string> AreaToCodeCn;
        private static readonly IDictionary<string, string> AreaToUrl;
        private static readonly IDictionary<string, string> AreaToCode;

        static AreaTransform()
        {
            AreaToUrlCn = new Dictionary<string, string>()
            {
                {"加拿大","https://mws.amazonservices.ca"},
                {"美国","https://mws.amazonservices.com"},
                {"德国","https://mws-eu.amazonservices.com"},
                {"西班牙","https://mws-eu.amazonservices.com"},
                {"法国","https://mws-eu.amazonservices.com"},
                {"印度","https://mws.amazonservices.in"},
                {"意大利","	https://mws-eu.amazonservices.com"},
                {"英国","https://mws-eu.amazonservices.com"},
                {"日本","https://mws.amazonservices.jp"},
                {"中国","https://mws.amazonservices.com.cn"}
            };

            AreaToCodeCn = new Dictionary<string, string>()
            {
                {"加拿大","A2EUQ1WTGCTBG2"},
                {"美国","ATVPDKIKX0DER"},
                {"德国","A1PA6795UKMFR9"},
                {"西班牙", "A1RKKUPIHCS9HS"},
                {"法国","A13V1IB3VIYZZH" },
                {"印度","A21TJRUUN4KGV" },
                {"意大利","APJ6JRA9NG5V4" },
                {"英国","A1F83G8C2ARO7P" },
                {"日本","A1VC38T7YXB528" },
                {"中国","AAHKV2X7AFYLW" }
            };


            AreaToUrl = new Dictionary<string, string>()
            {
                {"CA","https://mws.amazonservices.ca"},
                {"US","https://mws.amazonservices.com"},
                {"DE","https://mws-eu.amazonservices.com"},
                {"ES","https://mws-eu.amazonservices.com"},
                {"FR","https://mws-eu.amazonservices.com"},
                {"IN","https://mws.amazonservices.in"},
                {"IT","	https://mws-eu.amazonservices.com"},
                {"UK","https://mws-eu.amazonservices.com"},
                {"JP","https://mws.amazonservices.jp"},
                {"CN","https://mws.amazonservices.com.cn"}
            };


            AreaToCode = new Dictionary<string, string>()
            {
                {"CA","A2EUQ1WTGCTBG2"},
                {"US","ATVPDKIKX0DER"},
                {"DE","A1PA6795UKMFR9"},
                {"ES", "A1RKKUPIHCS9HS"},
                {"FR","A13V1IB3VIYZZH" },
                {"IN","A21TJRUUN4KGV" },
                {"IT","APJ6JRA9NG5V4" },
                {"UK","A1F83G8C2ARO7P" },
                {"JP","A1VC38T7YXB528" },
                {"CN","AAHKV2X7AFYLW" }
            };

        }

        /// <summary>
        /// GetUrlByArea
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public static string GetUrlByArea(string area)
        {
            string value;
            AreaToUrl.TryGetValue(area, out value);
            return value;
        }


        public static string GetCodeByArea(string area)
        {
            string value;
            AreaToCode.TryGetValue(area, out value);
            return value;
        }

        /// <summary>
        /// GetUrlByAreaCn
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public static string GetUrlByAreaCn(string area)
        {
            string value;
            AreaToUrlCn.TryGetValue(area, out value);
            return value;
        }


        /// <summary>
        /// GetCodeByAreaCn
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public static string GetCodeByAreaCn(string area)
        {
            string value;
            AreaToCodeCn.TryGetValue(area, out value);
            return value;
        }

        /// <summary>
        /// GetCodeList
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetCodeList()
        {
            IList<string> list = new List<string>();
            foreach (KeyValuePair<string,string> pair in AreaToCode)
            {
                list.Add(pair.Value);
            }
            return list;
        }

    }
}

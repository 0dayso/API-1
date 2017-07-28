using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace API.Extensions
{
    public static class StringExtension
    {
        public static string XmlToJson(this string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
            return json;
        }

        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        public static string UrlEncode(this string rawValue)
        {
            Encoding characterEncoding = Encoding.UTF8;
            string value = string.IsNullOrEmpty(rawValue) ? string.Empty : rawValue;
            string encoded = null;
            var encode = value.UrlEncodeToUpper(characterEncoding);
            if (encode != null)
                encoded = encode.Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
            return encoded;
        }

    }
}

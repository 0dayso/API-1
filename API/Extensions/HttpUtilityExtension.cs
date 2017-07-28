using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API.Extensions
{
    public static class HttpUtilityExtension
    {
        /// <summary>
        /// 转义为%..的后面部分转换成大写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlEncodeToUpper(this string str, Encoding encoding)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in str)
            {
                string raw = c.ToString();
                string encode = HttpUtility.UrlEncode(raw, encoding);
                if (raw == encode)
                {
                    stringBuilder.Append(raw);
                }
                else
                {
                    if (encode != null) stringBuilder.Append(encode.ToUpper());
                }
            }
            return stringBuilder.ToString();
        }
    }
}

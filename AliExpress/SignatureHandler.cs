using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AliExpress
{
    internal class SignatureHandler
    {
        //urlPath: 基础url部分，格式为protocol/apiVersion/namespace/apiName/appKey，如 json/1/system/currentTime/1；如果为客户端授权时此参数置为空串""
        //paramDic: 请求参数，即queryString + request body 中的所有参数
        public static string APISign(string urlPath, Dictionary<string, string> paramDic, string appSecret)
        {
            byte[] signatureKey = Encoding.UTF8.GetBytes(appSecret);//此处用自己的签名密钥
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> kv in paramDic)
            {
                list.Add(kv.Key + kv.Value);
            }
            list.Sort();
            string tmp = urlPath;
            foreach (string kvstr in list)
            {
                tmp = tmp + kvstr;
            }

            //HMAC-SHA1
            HMACSHA1 hmacsha1 = new HMACSHA1(signatureKey);
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            /*
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(urlPath));
            foreach (string kvstr in list)
            {
                hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(kvstr));
            }
             */
            byte[] hash = hmacsha1.Hash;
            //TO HEX
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
        }


        //paramDic: 请求参数，即queryString + request body 中的所有参数
        //appSecret ：app密钥，与请求参数中client_id表示的appKey对应
        public static string Sign(Dictionary<string, string> paramDic, string appSecret)
        {
            byte[] signatureKey = Encoding.UTF8.GetBytes(appSecret);
            //第一步：拼装key+value
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> kv in paramDic)
            {
                list.Add(kv.Key + kv.Value);
            }
            //第二步：排序
            list.Sort();
            //第三步：拼装排序后的各个字符串
            string tmp = "";
            foreach (string kvstr in list)
            {
                tmp = tmp + kvstr;
            }
            //第四步：将拼装后的字符串和app密钥一起计算签名
            //HMAC-SHA1
            HMACSHA1 hmacsha1 = new HMACSHA1(signatureKey);
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            byte[] hash = hmacsha1.Hash;
            //TO HEX
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
        }
    }
}

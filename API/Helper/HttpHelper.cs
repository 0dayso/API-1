using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Helper
{
    public class HttpHelper
    {
        ///// <summary>
        ///// GET OR POST 默认为GET
        ///// </summary>
        //public string Method { get; set; } = "GET";


        /// <summary>
        /// 用户代理
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";

        /// <summary>
        /// 最大自动重定向次数 默认为10
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; } = 10;


        /// <summary>
        /// 超时时间 默认为60s
        /// </summary>
        public int Timeout { get; set; } = 60000;

        /// <summary>
        ///  Cookies
        /// </summary>
        public string Cookies { get; set; } = string.Empty;


        /// <summary>
        /// 编码
        /// </summary>
        public Encoding HttpEncoding { get; set; } = Encoding.UTF8;


        /// <summary>
        /// 允许自动跳转
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = true;


        /// <summary>
        /// Referer
        /// </summary>
        public string Referer = string.Empty;

        /// <summary>
        /// Test
        /// </summary>
        internal void Test()
        {

            var htmlTest = GetHtmlByPost("http://music.163.com/weapi/v1/resource/comments/R_SO_4_418603077?csrf_token=", "params=P86tal00glp6vCsICC2ReHcObZCGDDxucxkNTuCDtZ%2BhUKxYznpaEq0hLzapHrwuvMELsl4X68crdlke%2FqVZZDEZD%2FfUdrbluk%2FQhIQZzW07zl7p72er4wv37d%2BPtOpo%2F69bmEpK2UAdOMUjcOgK8PWua2dytf6hJAaziscIHjnyVxPeCclwJBTy%2F9qmvnCw&encSecKey=6e07a383efd3c23a32ae837c90bfffc8d1ec1b27b4579d5e11782eaa383be39956f343ff548995634700c5ad7ad5f1cb8d227d3a2a7b07e0e74ffd630db22daa14b21f6310df63eae5e94079daa443d3f4fd0eb722d66814df2fa71c28b6ef444f58a38f2b10f58855c3ad190eae7bfb23e1d5b5895bfe3401e94c31b2fe8e63");
            //var hashSet = new HashSet<string>();
            var html = GetHtmlByGet("http://op.hanhande.com/");
            var result =
                GetFormatCookies(
                    @"		ali_apache_id=125.120.197.209.1481792318477.368048.7; path=/; domain=.aliexpress.com; expires=Wed, 30-Nov-2084 01:01:01 GMT,JSESSIONID=1BC1B4C3CB26C2AE8CDF660E27B6AA13; Path=/; HttpOnly,ali_apache_track=; Domain=.aliexpress.com; Expires=Tue, 02-Jan-2085 12:12:45 GMT; Path=/,ali_apache_tracktmp=; Domain=.aliexpress.com; Path=/,xman_us_f=x_l=1; Domain=.aliexpress.com; Expires=Tue, 02-Jan-2085 12:12:45 GMT; Path=/,acs_usuc_t=acs_rt=856f31265c374fb1a225e6b94d4d728e; Domain=.aliexpress.com; Path=/,xman_t=REgao+cL+oCEXVlTWVjFd/VCGePq6nxy6PFra7a14f+nMGryAz/LCvedfGesUc9H1vOJYnh1DPJM5jHqR+8cqthrv3rREIk/; Domain=.aliexpress.com; Path=/; HttpOnly,xman_f=CanHlyU4nkXwwC0iqvXU1XK8Jxf61uLA9OECt7GRdNLTy0/TIcfbsmYdPOX7atXU4zsuBKSFAIyIKAx7IqjxT57eU36uy4gz7II73hzHef5T93i42bnWsg==; Domain=.aliexpress.com; Expires=Tue, 02-Jan-2085 12:12:45 GMT; Path=/; HttpOnly");
            Console.WriteLine(result);
        }


        internal void Test2()
        {
            var html = GetHtmlByGet("http://wenshu.court.gov.cn/content/content?DocID=f42dfa1f-b5ca-4a22-a416-a74300f61906");
        }


        /// <summary>
        /// GetHtmlByGet
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetHtmlByGet(string url)
        {

            var stringEmpty = string.Empty;
            var html = stringEmpty;

            var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            if (httpWebRequest == null) return stringEmpty;
            httpWebRequest.Method = "GET";
            httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.MaximumAutomaticRedirections = MaximumAutomaticRedirections;
            httpWebRequest.Timeout = Timeout;
            httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;
            httpWebRequest.Referer = Referer;
            if (!string.IsNullOrEmpty(Cookies))
                httpWebRequest.Headers.Add("Cookie", Cookies);

            var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;

            using (var stream = httpWebResponse?.GetResponseStream())
                if (stream != null)
                    //使用缺省的编码
                    using (var streamReader = new StreamReader(stream, HttpEncoding))
                    {
                        html = streamReader.ReadToEnd();
                    }

            //更新cookies
            Cookies = GetFormatCookies(httpWebResponse?.Headers["set-cookie"] ?? Cookies);


            return html;
        }


        /// <summary>
        /// GetHtmlByPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataString"></param>
        /// <returns></returns>
        public string GetHtmlByPost(string url, string postDataString)
        {
            var stringEmpty = string.Empty;
            var html = stringEmpty;
            //postDataString 转化为 postDataByte
            var postDataByte = HttpEncoding.GetBytes(postDataString);

            var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            if (httpWebRequest == null) return stringEmpty;
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.MaximumAutomaticRedirections = MaximumAutomaticRedirections;
            httpWebRequest.Timeout = Timeout;
            httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Referer = Referer;
            httpWebRequest.ContentLength = postDataByte.Length;
            if (!string.IsNullOrEmpty(Cookies))
                httpWebRequest.Headers.Add("Cookie", Cookies);
            //httpWebRequest写入post数据
            using (var inputStream = httpWebRequest.GetRequestStream())
            {
                inputStream.Write(postDataByte, 0, postDataByte.Length);
            }

            var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
            using (var outputStream = httpWebResponse?.GetResponseStream())
            {
                if (outputStream != null)
                    using (var streamReader = new StreamReader(outputStream, HttpEncoding))
                    {
                        html = streamReader.ReadToEnd();
                    }
            }

            //更新cookies
            Cookies = GetFormatCookies(httpWebResponse?.Headers["set-cookie"] ?? Cookies);

            return html;

        }

        /// <summary>
        /// GetImage
        /// </summary>
        /// <param name="url"></param>
        public byte[] GetImage(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            if (httpWebRequest == null) return null;
            httpWebRequest.Method = "GET";
            httpWebRequest.UserAgent = UserAgent;

            var httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;

            using (var stream = httpWebResponse?.GetResponseStream())
                if (stream != null)
                {
                    var bitmap = new Bitmap(stream);
                    //bitmap.Save(@"C:\Users\Administrator\Desktop\验证码库\myTest.gif");
                    var memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Gif);
                    var bytes = memoryStream.GetBuffer();
                    return bytes;
                }
                else
                {
                    return null;
                }

        }

        /// <summary>
        /// Test1
        /// </summary>
        public void Test1()
        {
            GetImage("http://image.58.com/showphone.aspx?t=v55&v=E6B48CA0A765ED5BD6296D7D75B9B3593");
        }

        /// <summary>
        /// GetFormatCookies
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private string GetFormatCookies(string cookies)
        {
            //cookies字符串的分隔符为;有时候为&
            //Expires日期会含有,
            var cookieCollection = Regex.Matches(cookies, "[^;&,]*=[^;&,]*");
            //需要的key=value所包含的字符串 全部小写便于后面比较
            var removeString = new string[] { "domain", "path", "expires", "httponly", "max-age" };
            //保存移除字符串数组长度
            var length = removeString.Length;
            //用于保存不重复的key=value键值对
            //.net 2.0不包含HashSet
            //HashSet相当于Dictionary只有key而value为null
            var set = new Dictionary<string, object>();

            foreach (Match cookie in cookieCollection)
            {
                //取出首尾的空格
                var value = cookie.Value.Trim();
                var isContains = false;
                for (var i = 0; i < length; i++)
                {
                    if (!value.ToLower().Contains(removeString[i].ToLower())) continue;
                    isContains = true;
                    break;
                }

                //不包含移除字符串并且集合中不存在key=value值
                if (!isContains && !set.ContainsKey(value))
                {
                    set.Add(value, null);
                }

            }

            //拼接成结果 最后一个key=value没有;
            length = set.Count;
            var result = string.Empty;
            var curIndex = 0;
            foreach (var value in set)
            {
                if (++curIndex == length)
                    result += value.Key;
                else
                {
                    result += $"{value.Key};";
                }
            }

            return result;
        }

        /// <summary>
        /// 得到网页的默认编码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Encoding GetHtmlEncoding(string html)
        {
            // 目前发现的两种格式
            // <meta http-equiv="Content-Type" content="text/html; charset=GBK" />
            // <meta charset="utf-8">
            var encoding =
                Regex.Match(Regex.Match(html, "<meta.*?>").Value,
                    @"(?<=[cC][hH][aA][rR][sS][eE][tT][\s]*=[\s]*[""]?[\s]*)[^""]+?(?=[\s""])").Value;
            return Encoding.GetEncoding(encoding);
        }
    }
}

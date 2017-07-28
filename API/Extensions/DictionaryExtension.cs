using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Extensions
{

    public static class DictionaryExtension
    {


        /// <summary>
        /// GetPostData
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetPostData(this IDictionary<string, string> dic)
        {
            var stringBuilder = new StringBuilder(string.Empty);
            var isFirst = true;
            foreach (var keyValue in dic)
            {
                if (isFirst)
                {
                    stringBuilder.Append($"{keyValue.Key}={keyValue.Value}");
                    isFirst = false;
                }
                else
                    stringBuilder.Append($"&{keyValue.Key}={keyValue.Value}");
            }
            return stringBuilder.ToString();
        }



        /// <summary>
        /// Sort .net 3.5 以上版本
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> Sort<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            var dicSort = from keyValue in dic orderby keyValue.Key select keyValue;
            return dicSort;
        }




        /// <summary>
        /// SortOldVersion .net 2.0 版本
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> SortOldVersion<TKey, TValue>(this IDictionary<TKey, TValue> dic) where TKey : IComparable<TKey>
        {
            List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();

            foreach (var keyValue in dic)
            {
                list.Add(new KeyValuePair<TKey, TValue>(keyValue.Key, keyValue.Value));
            }

            list.Sort((keyValuePair, keyValuePair2) => keyValuePair.Key.CompareTo(keyValuePair2.Key));

            IDictionary<TKey, TValue> newDic = new Dictionary<TKey, TValue>();

            foreach (var keyValue in list)
            {
                newDic.Add(keyValue.Key, keyValue.Value);
            }

            return newDic;
        }



        /// <summary>
        /// 按照每个char排序 不然只能按照首字母排序 SortOldVersion .net 2.0 版本
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IDictionary<string, TValue> SortOldVersion<TValue>(this IDictionary<string, TValue> dic)
        {
            List<KeyValuePair<string, TValue>> list = new List<KeyValuePair<string, TValue>>();

            foreach (var keyValue in dic)
            {
                list.Add(new KeyValuePair<string, TValue>(keyValue.Key, keyValue.Value));
            }

            list.Sort((keyValuePair, keyValuePair2) => string.Compare(keyValuePair.Key, keyValuePair2.Key, StringComparison.Ordinal));

            IDictionary<string, TValue> newDic = new Dictionary<string, TValue>();

            foreach (var keyValue in list)
            {
                newDic.Add(keyValue.Key, keyValue.Value);
            }

            return newDic;
        }


        /// <summary>
        /// Print
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        public static void Print<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dic)
        {
            foreach (var keyValue in dic)
            {
                Console.WriteLine($"{keyValue.Key}:{keyValue.Value}");
            }
        }


        /// <summary>
        /// Print
        /// </summary>
        /// <param name="dic"></param>
        public static void Print(this IDictionary<string, List<object>> dic)
        {
            ICollection<string> keys = dic.Keys;
            int rowCount = dic.GetRowCount();
            for (var i = 0; i < rowCount; i++)
            {
                foreach (var key in keys)
                {
                    Console.Write($"{key}:{dic[key][i]} ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// GetRowCount
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static int GetRowCount(this IDictionary<string, List<object>> dic)
        {
            ICollection<List<object>> values = dic.Values;
            IEnumerator<List<object>> enumerator = values.GetEnumerator();
            int rowCount = 0;
            if (enumerator.MoveNext())
                rowCount = enumerator.Current.Count;
            return rowCount;
        }


        /// <summary>
        /// Combine
        /// </summary>
        /// <param name="dic1"></param>
        /// <param name="dic2"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Combine(this IDictionary<string, string> dic1,
            IDictionary<string, string> dic2)
        {
            foreach (KeyValuePair<string,string> pair in dic2)
            {
                dic1.Add(pair.Key,pair.Value);
            }
            return dic1;
        }


        private static void Test()
        {
            IDictionary<string, string> dic = new Dictionary<string, string>
            {
                {"call", "3" },
                {"bitch","2" },
                {"away","1" }
            };

            var sortedDic = dic.Sort<string, string>();
            sortedDic.Print();
            Console.WriteLine("OldVersion");
            var sortedDic2 = dic.SortOldVersion<string, string>();
            sortedDic2.Print();
        }
    }
}

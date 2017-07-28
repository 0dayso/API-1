using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using API.Extensions;
using MySql.Data.MySqlClient;

namespace API.Helper
{
    public class MySqlHelper
    {
        private string _dbServer = "127.0.0.1";
        private string _dbName = "zhanxian";
        private string _dbUserName = "root";
        private string _dbPassWord = "root";

        /// <summary>
        /// 无参数的构造函数
        /// </summary>
        public MySqlHelper()
        {
        }

        /// <summary>
        /// 设置参数的构造函数
        /// </summary>
        /// <param name="dbServer"></param>
        /// <param name="dbName"></param>
        /// <param name="dbUserName"></param>
        /// <param name="dbPassWord"></param>
        public MySqlHelper(string dbServer, string dbName, string dbUserName, string dbPassWord)
        {
            _dbServer = dbServer;
            _dbName = dbName;
            _dbUserName = dbUserName;
            _dbPassWord = dbPassWord;
        }

        /// <summary>
        /// InitParameter
        /// </summary>
        /// <param name="dbServer"></param>
        /// <param name="dbName"></param>
        /// <param name="dbUserName"></param>
        /// <param name="dbPassWord"></param>
        public void InitParameter(string dbServer, string dbName, string dbUserName, string dbPassWord)
        {
            _dbServer = dbServer;
            _dbName = dbName;
            _dbUserName = dbUserName;
            _dbPassWord = dbPassWord;
        }

        /// <summary>
        /// GetMySqlConnection
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetMySqlConnection()
        {
            string conn = $"server={_dbServer};database={_dbName};Uid={_dbUserName};Pwd={_dbPassWord}";
            var mySqlConnection = new MySqlConnection(conn);
            return mySqlConnection;
        }

        /// <summary>
        /// GetMySqlCommand
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="mySqlConnection"></param>
        /// <returns></returns>
        public MySqlCommand GetMySqlCommand(string sql, MySqlConnection mySqlConnection)
        {
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            return mySqlCommand;
        }




        /// <summary>
        /// GetSelectResult
        /// </summary>
        /// <param name="mySqlCommand"></param>
        public MySqlDataReader GetSelectReader(MySqlCommand mySqlCommand)
        {
            var reader = mySqlCommand.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// GetSelectDataTable
        /// </summary>
        /// <param name="mySqlCommand"></param>
        /// <returns></returns>
        public DataTable GetSelectDataTable(MySqlCommand mySqlCommand)
        {
            //得到reader对象
            var reader = GetSelectReader(mySqlCommand);
            var dataTable = new DataTable();
            LoadDataReader(dataTable, reader);
            //字段的字符数量在几百个以内就能够正常的转换，但是一旦上千个字符就会出现以上异常了
            //dataTable.Load(reader);
            return dataTable;
        }


        /// <summary>
        /// LoadDataReader
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dataReader"></param>
        private void LoadDataReader(DataTable dataTable, IDataReader dataReader)
        {
            int fieldCount = dataReader.FieldCount;
            for (int i = 0; i < fieldCount; ++i)
            {
                dataTable.Columns.Add(dataReader.GetName(i), dataReader.GetFieldType(i));
            }
            dataTable.BeginLoadData();
            object[] objValues = new object[fieldCount];
            while (dataReader.Read())
            {
                dataReader.GetValues(objValues);
                dataTable.LoadDataRow(objValues, true);
            }
            dataReader.Close();
            dataTable.EndLoadData();
        }




        /// <summary>
        /// GetSelectDic
        /// </summary>
        /// <param name="mySqlCommand"></param>
        /// <returns></returns>
        public IDictionary<string, List<object>> GetSelectDic(MySqlCommand mySqlCommand)
        {
            var dataTable = GetSelectDataTable(mySqlCommand);
            IDictionary<string, List<object>> dic = new Dictionary<string, List<object>>();
            var columns = dataTable.Columns;
            var countColumns = columns.Count;
            var rows = dataTable.Rows;
            var countRows = rows.Count;

            //列
            for (var j = 0; j < countColumns; j++)
            {
                var list = new List<object>();
                //行
                for (var i = 0; i < countRows; i++)
                {

                    list.Add(rows[i][j]);

                }
                dic.Add(columns[j].ToString(), list);
            }



            return dic;
        }





        /// <summary>
        /// GetSelectDicBySql
        /// </summary>
        /// <param name="sqlSelect"></param>
        /// <returns></returns>
        public IDictionary<string, List<object>> GetSelectDicBySql(string sqlSelect)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sqlSelect, mySqlConnection))
                {
                    mySqlConnection.Open();
                    var dic = GetSelectDic(mySqlCommand);
                    return dic;
                }
            }
        }




        /// <summary>
        /// GetSelectBySqlWithLock
        /// </summary>
        /// <param name="sqlSelect"></param>
        /// <param name="lockName"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public IDictionary<string, List<object>> GetSelectBySqlWithLock(string sqlSelect,string lockName,Action<string> start)
        {
            
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sqlSelect, mySqlConnection))
                {
                    mySqlConnection.Open();
                    //加锁
                    GetLock(mySqlConnection, lockName);
                    
                    var dic = GetSelectDic(mySqlCommand);


                    //设置为开始状态
                    ICollection<string> keys = dic.Keys;
                    int rowCount = dic.GetRowCount();

                    for (int i = 0; i < rowCount; i++)
                    {
                        IDictionary<string, object> curDic = new Dictionary<string, object>();
                        foreach (var key in keys)
                        {
                            curDic.Add(key, dic[key][i]);
                        }
                        string accountId = curDic["account_id"].ToString();
                        //任务表设置开始状态
                        start(accountId);
                    }

                    //解锁
                    GetReleaseLock(mySqlConnection,lockName);

                    return dic;
                }
            }
        }

        /// <summary>
        /// UpdateTable
        /// </summary>
        /// <param name="mySqlCommand"></param>
        private void UpdateTable(MySqlCommand mySqlCommand)
        {
            mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// UpdateTable
        /// </summary>
        /// <param name="sqlUpdate"></param>
        public void UpdateTable(string sqlUpdate)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sqlUpdate, mySqlConnection))
                {
                    mySqlConnection.Open();
                    UpdateTable(mySqlCommand);
                }
            }
        }


        /// <summary>
        /// GetSelectOneValue
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GetSelectOneValue(string sql, string fieldName)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sql, mySqlConnection))
                {
                    mySqlConnection.Open();
                    MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                    if (mySqlDataReader.Read())
                    {
                        return mySqlDataReader[fieldName].ToString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// DeleteTable
        /// </summary>
        /// <param name="mySqlCommand"></param>
        public void DeleteTable(MySqlCommand mySqlCommand)
        {
            mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// DeleteTable
        /// </summary>
        /// <param name="sqlDelete"></param>
        public void DeleteTable(string sqlDelete)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sqlDelete, mySqlConnection))
                {
                    mySqlConnection.Open();
                    DeleteTable(mySqlCommand);
                }
            }
        }


        /// <summary>
        /// CreateTable
        /// </summary>
        /// <param name="sqlCreate"></param>
        public void CreateTable(string sqlCreate)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sqlCreate, mySqlConnection))
                {
                    mySqlConnection.Open();
                    CreateTable(mySqlCommand);
                }
            }
        }


        /// <summary>
        /// CreateTable
        /// </summary>
        /// <param name="mySqlCommand"></param>
        private void CreateTable(MySqlCommand mySqlCommand)
        {
            mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// InsertTable
        /// </summary>
        /// <param name="mySqlCommand"></param>
        private void InsertTable(MySqlCommand mySqlCommand)
        {
            mySqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// InsertTable
        /// </summary>
        /// <param name="sqlInsert"></param>
        public void InsertTable(string sqlInsert)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                using (var mySqlCommand = GetMySqlCommand(sqlInsert, mySqlConnection))
                {
                    mySqlConnection.Open();
                    InsertTable(mySqlCommand);
                }
            }
        }


        /// <summary>
        /// InsertTableWithDicExistsUpdate
        /// </summary>
        /// <param name="infoDic"></param>
        /// <param name="tableName"></param>
        public void InsertTableWithDicExistsUpdate(IDictionary<string, object> infoDic, string tableName)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                //打开连接
                mySqlConnection.Open();
                var strPart1 = $"INSERT INTO {tableName}";
                var strPart2 = " VALUES";
                var strPart3 = " ON DUPLICATE KEY UPDATE ";
                var curIndex = 0;
                var length = infoDic.Count;
                foreach (var info in infoDic)
                {
                    if (curIndex == 0)
                    {
                        strPart1 = $"{strPart1}({info.Key}";
                        strPart2 = $"{strPart2}(@{info.Key}";
                        strPart3 = $"{strPart3}{info.Key}=@{info.Key}";
                    }
                    else if (curIndex == length - 1)
                    {
                        strPart1 = $"{strPart1},{info.Key})";
                        strPart2 = $"{strPart2},@{info.Key})";
                        strPart3 = $"{strPart3},{info.Key}=@{info.Key}";
                    }
                    else
                    {
                        strPart1 = $"{strPart1},{info.Key}";
                        strPart2 = $"{strPart2},@{info.Key}";
                        strPart3 = $"{strPart3},{info.Key}=@{info.Key}";
                    }

                    curIndex++;

                }

                var str = $"{strPart1}{strPart2}{strPart3}";


                using (var mySqlCommand = GetMySqlCommand(str, mySqlConnection))
                {


                    foreach (var info in infoDic)
                    {
                        //转义 SQL 语句中使用的字符串中的特殊字符
                        //mysql_real_escape_string
                        mySqlCommand.Parameters.AddWithValue($"@{info.Key}", info.Value);
                    }

                    mySqlCommand.ExecuteNonQuery();

                }
            }
        }




        /// <summary>
        /// InsertTableWithDicExistsUpdate
        /// </summary>
        /// <param name="infoDic"></param>
        /// <param name="tableName"></param>
        public void InsertTableWithDicExistsUpdate(IDictionary<string, string> infoDic, string tableName)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                //打开连接
                mySqlConnection.Open();
                var strPart1 = $"INSERT INTO {tableName}";
                var strPart2 = " VALUES";
                var strPart3 = " ON DUPLICATE KEY UPDATE ";
                var curIndex = 0;
                var length = infoDic.Count;
                foreach (var info in infoDic)
                {
                    if (curIndex == 0)
                    {
                        strPart1 = $"{strPart1}({info.Key}";
                        strPart2 = $"{strPart2}(@{info.Key}";
                        strPart3 = $"{strPart3}{info.Key}=@{info.Key}";
                    }
                    else if (curIndex == length - 1)
                    {
                        strPart1 = $"{strPart1},{info.Key})";
                        strPart2 = $"{strPart2},@{info.Key})";
                        strPart3 = $"{strPart3},{info.Key}=@{info.Key}";
                    }
                    else
                    {
                        strPart1 = $"{strPart1},{info.Key}";
                        strPart2 = $"{strPart2},@{info.Key}";
                        strPart3 = $"{strPart3},{info.Key}=@{info.Key}";
                    }

                    curIndex++;

                }

                var str = $"{strPart1}{strPart2}{strPart3}";


                using (var mySqlCommand = GetMySqlCommand(str, mySqlConnection))
                {


                    foreach (var info in infoDic)
                    {
                        //转义 SQL 语句中使用的字符串中的特殊字符
                        //mysql_real_escape_string
                        mySqlCommand.Parameters.AddWithValue($"@{info.Key}", info.Value);
                    }

                    mySqlCommand.ExecuteNonQuery();

                }
            }
        }


        /// <summary>
        /// InsertTableWithDic
        /// </summary>
        /// <param name="infoDic"></param>
        /// <param name="tableName"></param>
        public void InsertTableWithDic(IDictionary<string, string> infoDic, string tableName)
        {
            using (var mySqlConnection = GetMySqlConnection())
            {
                //打开连接
                mySqlConnection.Open();
                var strPart1 = $"INSERT INTO {tableName}";
                var strPart2 = " VALUES";
                var curIndex = 0;
                var length = infoDic.Count;
                foreach (var info in infoDic)
                {
                    if (curIndex == 0)
                    {
                        strPart1 = $"{strPart1}({info.Key}";
                        strPart2 = $"{strPart2}(@{info.Key}";
                    }
                    else if (curIndex == length - 1)
                    {
                        strPart1 = $"{strPart1},{info.Key})";
                        strPart2 = $"{strPart2},@{info.Key})";
                    }
                    else
                    {
                        strPart1 = $"{strPart1},{info.Key}";
                        strPart2 = $"{strPart2},@{info.Key}";
                    }

                    curIndex++;

                }

                var str = $"{strPart1}{strPart2}";

                using (var mySqlCommand = GetMySqlCommand(str, mySqlConnection))
                {


                    foreach (var info in infoDic)
                    {
                        //转义 SQL 语句中使用的字符串中的特殊字符
                        //mysql_real_escape_string
                        mySqlCommand.Parameters.AddWithValue($"@{info.Key}", info.Value);
                    }

                    mySqlCommand.ExecuteNonQuery();

                }
            }
        }

        /// <summary>
        /// GetLock
        /// </summary>
        /// <param name="mySqlConnection"></param>
        /// <param name="lockName"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public void GetLock(MySqlConnection mySqlConnection, string lockName, int timeout = 50)
        {
            var sqlLock = $"SELECT GET_LOCK('{lockName}',{timeout});";
            var mySqlCommandLock = GetMySqlCommand(sqlLock, mySqlConnection);
            var result = mySqlCommandLock.ExecuteScalar();
            //若成功得到锁，则返回 1，若操作超时则返回0 (例如,由于另一个客户端已提前封锁了这个名字 ),若发生错误则返回NULL (诸如缺乏记忆或线程mysqladmin kill 被断开 )
            //0操作超时 1成功加锁 null失败
            if (result == null)
                throw new Exception("加锁失败");
            if(result.ToString()=="0")
                throw new Exception("加锁操作超时");
        }


        /// <summary>
        /// GetReleaseLock
        /// </summary>
        /// <param name="mySqlConnection"></param>
        /// <param name="lockName"></param>
        /// <returns></returns>
        public void GetReleaseLock(MySqlConnection mySqlConnection, string lockName)
        {
            var sqlUnLock = $"SELECT RELEASE_LOCK('{lockName}');";
            var mySqlCommandReleaseLock = GetMySqlCommand(sqlUnLock, mySqlConnection);
            mySqlCommandReleaseLock.ExecuteScalar();
            //如果锁被成功释放，返回1；如果这个进程没有占有该锁，则返回0；如果这个名为str的锁不存在，则返回NULL。
        }

    }
}

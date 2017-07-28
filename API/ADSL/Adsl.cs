using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotRas;

namespace API.ADSL
{
    public class Adsl
    {
        /// <summary>
        /// Connect
        /// </summary>
        /// <param name="connectName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void Connect(string connectName, string userName, string password)
        {
            //关闭现有连接
            IReadOnlyCollection<RasConnection> conns = RasConnection.GetActiveConnections();
            if (conns.Count!=0)
            {
                foreach (var conn in conns)
                {
                    conn.HangUp();
                }
            }
            
            RasPhoneBook rasPhoneBook = new RasPhoneBook();
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            rasPhoneBook.Open(path);
            

            //创建一个新的PPPOE连接
            IReadOnlyCollection<RasDevice> rasDevices = RasDevice.GetDevices();
            RasDevice device = rasDevices.First(d => d.DeviceType == RasDeviceType.PPPoE);
            var entry = RasEntry.CreateBroadbandEntry(connectName, device);
            entry.PhoneNumber = userName;
            //如果不存在该名字的用户名和密码的连接，则Add 存在则更新
            if (!rasPhoneBook.Entries.Contains(connectName))
                rasPhoneBook.Entries.Add(entry);
            else
                rasPhoneBook.Entries[connectName].Update();

            RasDialer dialer = new RasDialer
            {
                EntryName = connectName,
                PhoneNumber = userName,
                AllowUseStoredCredentials = true,
                PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers),
                Credentials = new NetworkCredential(userName, password),
                Timeout = 1000*10
            };

            RasHandle rasHandle = dialer.Dial();
        }


        public static void Connect(string connectName)
        {
            //关闭现有连接
            IReadOnlyCollection<RasConnection> conns = RasConnection.GetActiveConnections();
            if (conns.Count != 0)
            {
                foreach (var conn in conns)
                {
                    conn.HangUp();
                }
            }

            RasPhoneBook rasPhoneBook = new RasPhoneBook();
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            rasPhoneBook.Open(path);

            foreach (RasEntry rasEntry in rasPhoneBook.Entries)
            {
                if (rasEntry.Name == connectName)
                {
                    RasDialer dialer = new RasDialer
                    {
                        EntryName = rasEntry.Name,
                        PhoneNumber = rasEntry.PhoneNumber,
                        AllowUseStoredCredentials = true,
                        PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers),
                        Credentials = rasEntry.GetCredentials(),
                        Timeout = 1000 * 10

                    };

                    RasHandle rasHandle = dialer.Dial();

                }
            }

        }


        private void Test()
        {
            Connect("123", "057120337993", "552727");
        }

        private void Test1()
        {
            Connect("123");
        }

    }
}

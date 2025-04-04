using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Server.Model;
using Newtonsoft.Json;

namespace Server.Controller
{
    public class MainServer
    {
        public async Task StartMainServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 10000);
            Console.WriteLine(" 메인서버 시작 ");
            listener.Start();
            while (true)
            {
                TcpClient clnt =
                    await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                Task.Run(() => ServerMainAsync(clnt));
            }
        }

        private async Task ServerMainAsync(TcpClient clnt)
        {
            DBC dbc = new();
            Console.WriteLine("DB 연결");
            byte[] buf = new byte[1024];
            Receive_msg msg;
            NetworkStream stream = clnt.GetStream();
            while (true)
            {
                int len = await stream.ReadAsync(buf).ConfigureAwait(false);
                string json = Encoding.UTF8.GetString(buf);
                msg = JsonConvert.DeserializeObject<Receive_msg>(json);
                handler(msg);
                
            }
        }

        private void handler(Receive_msg msg)
        {

        }

        public int dif_date(DateTime entryDate, DateTime exitDate)
        {
            TimeSpan timeDiff = entryDate - exitDate;
            int min = (int)timeDiff.TotalMinutes;
            return min;
        }

        public string Cal_totalFee(int parkingTime)
        {
            string totalFee = (Fee.fee * parkingTime / 10).ToString() + "원";
            return totalFee;
        }

        //    string query = $"SELECT ENTRY_DATE FROM Entry_Exit_Record WHERE VEHICLE_NUM = '{User.Vehicle_num}' AND EXIT_DATE IS NULL;";
        //    MySqlDataReader? dr = null;
        //    MySqlCommand cmd = new MySqlCommand(query, DBC.Conn);

        //    dr = cmd.ExecuteReader();
        //        if (dr.Read())
        //        {
        //            Vehicle.EntryDate = dr.GetDateTime(0);
        //        }
        //dr.Close();
    }
}

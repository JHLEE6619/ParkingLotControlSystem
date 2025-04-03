using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controller
{
    public static MySqlConnection Conn { get; set; } = Connect_DB();

    private static MySqlConnection Connect_DB()
    {
        string ip = "127.0.0.1";
        int port = 3306;
        string uid = "root";
        string pwd = "1234";
        string dbname = "PARKING_LOT";
        MySqlConnection conn;
        string connectString = $"Server={ip};Port={port};Database={dbname};Uid={uid};Pwd={pwd};CharSet=utf8;";
        // 연결 확인
        conn = new MySqlConnection(connectString);
        conn.Open();
        conn.Ping();
        return conn;
    }

    public class MainServer
    {


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

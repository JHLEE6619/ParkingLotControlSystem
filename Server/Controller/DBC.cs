using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Server.Model;

namespace Server.Controller
{
    public class DBC
    {
        public MySqlConnection Conn { get; set; }

        public DBC()
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
        }

        public int dif_date(DateTime entryDate, DateTime exitDate)
        {
            TimeSpan timeDiff = entryDate - exitDate;
            int min = (int)timeDiff.TotalMinutes;
            return min;
        }

        public int Cal_totalFee(int parkingTime)
        {
            int totalFee = (Fee.fee * parkingTime / 10);
            return totalFee;
        }

        private Record Date_format(Entry_exit_record entry_exit_record)
        {
            Record record = new();
            string dateForamt = "MM월 dd일 HH시 mm분";
            record.EntryDate = entry_exit_record.EntryDate.ToString(dateForamt);
            DateTime now = DateTime.Now;
            record.ExitDate = now.ToString(dateForamt);

            int parkingTime = dif_date(entry_exit_record.EntryDate, entry_exit_record.ExitDate);
            record.ParkingTime = parkingTime + "분";
            int totalFee = Cal_totalFee(parkingTime);
            record.TotalFee = totalFee.ToString() + "원";
            // 같은 시점에서 업데이트하면 안되고, 결제하는 시점으로 바뀌어야함
            Update_paymentInfo(entry_exit_record.VehicleNum, now, totalFee);

            return record;
        }

        // 정기 등록차량인지 조회 -> 일반차량이면 사전 정산 차량인지 조회 ->
        // 정기 등록 차량인지 조회
        public void Select_user(string vehicleNum)
        {
            string query = $"SELECT EXP_DATE FROM USER WHERE VEHICLE_NUM = '{vehicleNum}'";
            MySqlDataReader? dr = null;
            MySqlCommand cmd = new MySqlCommand(query, Conn);

            dr = cmd.ExecuteReader();
            // 정기 등록된 차량이면
            if (dr.Read())
            {
                DateTime exp_date = dr.GetDateTime(0);
                DateTime now = DateTime.Now;
                if(exp_date.Date <= now.Date)
                {

                }
            }
        }

        public Record Select_record(string vehicleNum)
        {
            Entry_exit_record record = new Entry_exit_record()
            {
                VehicleNum = vehicleNum
            };

            string query = $"SELECT ENTRY_DATE, CLASSIFICATION FROM Entry_Exit_Record WHERE VEHICLE_NUM = '{vehicleNum}' AND EXIT_DATE IS NULL;";
            MySqlDataReader? dr = null;
            MySqlCommand cmd = new MySqlCommand(query, Conn);

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                record.EntryDate = dr.GetDateTime(0);
                record.Classification = dr.GetByte(1);
            }
            dr.Close();
            
            return Date_format(record);
        }

        private void Update_paymentInfo(string vehicleNum, DateTime ExitDate, int TotalFee)
        {
            string query =  $"UPDATE ENTRY_EXIT_RECORD " +
                            $"SET EXIT_DATE = {ExitDate}," +
                            $"FEE = {TotalFee}," +
                            $"CLASSIFICATION = 1 "+
                            $"WHERE VEHICLE_NUM = '{vehicleNum}' AND EXIT_DATE IS NULL;";
            MySqlCommand cmd = new MySqlCommand(query, Conn);
            cmd.ExecuteNonQuery();
        }

        public void Insert_regInfo(User user)
        {
            DateTime First_reg_date = DateTime.Now;
            DateTime exp_date = First_reg_date.AddDays(user.Reg_period);
            string query = $"INSERT INTO USER VALUES " +
                            $"({user.VehicleNum}, {user.Pw}, {First_reg_date}, {exp_date}, {user.Fee});";
            MySqlCommand cmd = new MySqlCommand(query, Conn);
            cmd.ExecuteNonQuery();
        }

        public void Update_regInfo(User user)
        {
            string query = $"UPDATE USER SET EXP_DATE = date_add((SELECT EXP_DATE FROM (SELECT EXP_DATE FROM USER WHERE VEHICLE_NUM = '{user.VehicleNum}') AS A)," +
                $"INTERVAL {user.Reg_period} DAY)," +
                $"TOTAL_FEE = (SELECT TOTAL_FEE FROM (SELECT TOTAL_FEE FROM USER WHERE VEHICLE_NUM = '{user.VehicleNum}') AS B) + {user.Fee} WHERE VEHICLE_NUM ={user.VehicleNum};";
            MySqlCommand cmd = new MySqlCommand( query, Conn);
            cmd.ExecuteNonQuery();
        }

        


    }
}

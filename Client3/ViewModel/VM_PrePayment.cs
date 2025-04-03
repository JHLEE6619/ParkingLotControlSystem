using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Client3.Model;
using Client3.ViewModel.Commands;
using MySqlConnector;

namespace Client3.ViewModel
{
    public class VM_PrePayment
    {
        public User User { get; set; } = new();
        public Record Record { get; set; } = new();
        public Command Show_fee { get; set; }
        public Command Payment { get; set; }

        public VM_PrePayment()
        {
            Show_fee = new Command(Send_vehicleNum);
            Payment = new Command(Go_to_paymentPage);
        }

        public void Send_vehicleNum()
        {
            Send_msg msg = new()
            {
                MsgId = (byte)Network.MsgId.ENTRY_RECORD,
                Record = this.Record
            };
            byte[] buf = Network.Serialize_to_json(msg);
            Network.stream.Write(buf);

            // 입/출차 일시, 주차 시간, 주차 요금 입력
            Entry_exit_record entry_Exit_Record = Network.Receive_message().Vehicle;
            string dateForamt = "MM월 dd일 HH시 mm분";
            Record.EntryDate = entry_Exit_Record.EntryDate.ToString(dateForamt);
            Record.ExitDate = DateTime.Now.ToString(dateForamt);
            int parkingTime = dif_date(entry_Exit_Record.EntryDate, entry_Exit_Record.ExitDate);
            Record.ParkingTime = parkingTime + "분";
            Record.TotalFee = Cal_totalFee(parkingTime);

            // 결제버튼 활성화
        }

        public void Go_to_paymentPage()
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
            string totalFee = (Fee.fee * parkingTime).ToString() + "원";
            return totalFee;
        }
    }
}

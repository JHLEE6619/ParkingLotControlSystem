namespace Server.Model
{
    public class Receive_msg
    {
        public int MsgId { get; set; }
        public User User { get; set; }
        public Record Record { get; set; }
    }
}
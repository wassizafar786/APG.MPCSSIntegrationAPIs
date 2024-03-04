namespace APG.MessageQueue.Mpcss
{
    public class HeartBeatStatus
    {
        public DateTime? LastHeartBeatReceived { get; set; }
        public bool ConnectionIsProcessing { get; set; } 
    }
}

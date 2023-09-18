using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimDpsBot
{
    public class QueueDTO
    {
        public int queueSpot { get; set; }
        public Process Process { get; set; }
        public SettingsDTO Settings { get; set; }
        public ulong MessageID { get; set; }
        public ulong ChannelID { get; set; }
        public string Realm { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public bool IsComplete { get; set; }
        public string ReportLink { get; set; }
        public string Simdps { get; set; }
        public string HTMLFilelocation { get; set; }
        public DateTime TimeStarted { get; set; }
    }
}

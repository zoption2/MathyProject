using System;

namespace Mathy.Services
{
    [Serializable]
    public class DayResultData
    {
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public Achievements Reward { get; set; }
        public int MiddleRate { get; set; }
    }
}


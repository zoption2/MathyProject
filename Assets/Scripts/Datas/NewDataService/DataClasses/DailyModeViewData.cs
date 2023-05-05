using System;


namespace Mathy.Data
{
    [Serializable]
    public class DailyModeViewData
    {
        public TaskMode Mode { get; set; }
        public int TotalCompletedModes { get; set; }
        public int TotalTasks { get; set; }
        public int TotalCorrect { get; set; }
        public int MiddleRate { get; set; }
        public double TotalTime { get; set; }
    }
}


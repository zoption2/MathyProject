using System;

namespace Mathy.Services.Data
{
    [Serializable]
    public class DailyModeViewModel
    {
        public string Mode { get; set; }
        public int ModeIndex { get; set; }
        public int TotalCompletedModes { get; set; }
        public int TotalTasks { get; set; }
        public int TotalCorrect { get; set; }
        public int MiddleRate { get; set; }
        public double TotalTime { get; set; }
    }
}


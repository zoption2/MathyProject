using System;


namespace Mathy.Data
{
    [Serializable]
    public class DailyModeViewData
    {
        public TaskMode Mode { get; set; }
        public int TotalCompleted { get; set; }
    }
}


using System;

namespace Mathy.Services.Data
{
    [Serializable]
    public class DailyModeViewModel
    {
        public string Mode { get; set; }
        public int ModeIndex { get; set; }
        public int TotalCompleted { get; set; }
    }
}


using System;

namespace Mathy.Services
{
    [Serializable]
    public class DailyModeData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TaskMode Mode { get; set; }
        public bool IsComplete { get; set; }
        public int PlayedCount { get; set; }
    }
}


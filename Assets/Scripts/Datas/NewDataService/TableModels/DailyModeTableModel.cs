using System;

namespace Mathy.Services
{
    [Serializable]
    public class DailyModeTableModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Mode { get; set; }
        public int ModeIndex { get; set; }
        public bool IsComplete { get; set; }
        public int PlayedTasks { get; set; }
    }
}


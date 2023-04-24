using System;

namespace Mathy.Services.Data
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
        public int CorrectAnswers { get; set; }
        public int CorrectRate { get; set; }
        public double Duration { get; set; }
        public int TotalTasks { get; set; }
    }
}


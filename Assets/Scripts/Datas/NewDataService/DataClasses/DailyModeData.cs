using System;
using System.Collections.Generic;

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
        public int CorrectAnswers { get; set; }
        public int CorrectRate { get; set; }
        public double Duration { get; set; }
        public int TotalTasks { get; set; }
        public List<int> TasksIds { get; set; } = new();
    }
}


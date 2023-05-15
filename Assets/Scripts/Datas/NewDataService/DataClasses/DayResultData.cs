using System;
using System.Collections.Generic;

namespace Mathy.Services
{
    [Serializable]
    public class DayResultData
    {
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public Achievements Reward { get; set; }
        public int TotalTasks { get; set; }
        public int CorrectTasks { get; set; }
        public int MiddleRate { get; set; }
        public List<TaskMode> CompletedModes { get; set; } = new();
        public double Duration { get; set; }
    }
}


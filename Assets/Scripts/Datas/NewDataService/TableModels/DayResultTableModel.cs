using System;

namespace Mathy.Services.Data
{
    [Serializable]
    public class DayResultTableModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public bool IsComplete { get; set; }
        public string Reward { get; set; }
        public int RewardIndex { get; set; }
        public int TotalTasks { get; set; }
        public int CorrectTasks { get; set; }
        public int MiddleRate { get; set; }
        public string CompletedModes { get; set; }
        public double Duration { get; set; }
    }
}


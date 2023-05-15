using System;


namespace Mathy.Data
{
    [Serializable]
    public class DetailedTasksViewData
    {
        public TaskType TaskType { get; set; }
        public int TotalTasksPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int MiddleRate { get; set; }
        public double TotalPlayedTime { get; set; }
    }
}


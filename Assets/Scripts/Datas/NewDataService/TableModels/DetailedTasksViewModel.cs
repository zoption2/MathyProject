using System;

namespace Mathy.Services.Data
{
    [Serializable]
    public class DetailedTasksViewModel
    {
        public string TaskType { get; set; }
        public int TaskTypeIndex { get; set; }
        public int TotalTasksPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int MiddleRate { get; set; }
        public double TotalPlayedTime { get; set; }
    }
}


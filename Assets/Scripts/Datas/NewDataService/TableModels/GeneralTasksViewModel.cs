using System;

namespace Mathy.Services.Data
{
    [Serializable]
    public class GeneralTasksViewModel
    {
        public int TotalTasksPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int MiddleRate { get; set; }
        public double TotalPlayedTime { get; set; }
    }
}


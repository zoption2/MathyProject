using System;


namespace Mathy.Data
{
    [Serializable]
    public class GeneralTasksViewData
    {
        public int TotalTasksPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public int MiddleRate { get; set; }
        public double TotalPlayedTime { get; set; }
    }
}


using System;

namespace Mathy.Services.Data
{
    [Serializable]
    public class GeneralResultsTableModel
    {
        public int TotalTasksPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public double TotalPlayedTime { get; set; }
        public string EachTaskPlayedJson { get; set; }
        public string TaskMiddleRatingJson { get; set; }
        public string EachModePlayedJson { get; set; }
        public string ModeMiddleRatingJson { get; set; }
        public string ModeCompletedJson { get; set; }
        public string SkillTypeMiddleRatingJson { get; set; }
    }
}


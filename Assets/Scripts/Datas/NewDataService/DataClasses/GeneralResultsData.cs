using Mathy.Data;
using System;
using System.Collections.Generic;

namespace Mathy.Services
{
    [Serializable]
    public class GeneralResultsData
    {
        public int TotalTasksPlayed { get; set; }
        public int TotalCorrectAnswers { get; set; }
        public double TotalPlayedTime { get; set; }
        public Dictionary<TaskType, int> EachTaskPlayed { get; set; } = new();
        public Dictionary<TaskType, int> TaskMiddleRating { get; set; } = new();
        public Dictionary<TaskMode, int> EachModePlayed { get; set; } = new();
        public Dictionary<TaskMode, int> ModeMiddleRating { get; set; } = new();
        public Dictionary<TaskMode, int> ModeCompleted { get; set; } = new();
        public Dictionary<SkillType, int> SkillTypeMiddleRating { get; set; } = new();
    }
}


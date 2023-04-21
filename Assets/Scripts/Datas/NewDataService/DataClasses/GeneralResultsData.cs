using System;
using System.Collections.Generic;

namespace Mathy.Data
{
    public interface IReadonlyGeneralResultsData
    {
        public int TotalTasksPlayed { get; }
        public int TotalCorrectAnswers { get; }
        public double TotalPlayedTime { get; }
        public IReadOnlyDictionary<TaskType, int> ReadonlyEachTaskPlayed { get; }
        public IReadOnlyDictionary<TaskType, int> ReadonlyTaskMiddleRating { get; }
        public IReadOnlyDictionary<TaskMode, int> ReadonlyEachModePlayed { get; }
        public IReadOnlyDictionary<TaskMode, int> ReadonlyModeMiddleRating { get; }
        public IReadOnlyDictionary<TaskMode, int> ReadonlyModeCompleted { get; }
        public IReadOnlyDictionary<SkillType, int> ReadonlySkillTypeMiddleRating { get; }
    }

    [Serializable]
    public class GeneralResultsData : IReadonlyGeneralResultsData
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



        public IReadOnlyDictionary<TaskType, int> ReadonlyEachTaskPlayed => EachTaskPlayed;
        public IReadOnlyDictionary<TaskType, int> ReadonlyTaskMiddleRating => TaskMiddleRating;
        public IReadOnlyDictionary<TaskMode, int> ReadonlyEachModePlayed => EachModePlayed;
        public IReadOnlyDictionary<TaskMode, int> ReadonlyModeMiddleRating => ModeMiddleRating;
        public IReadOnlyDictionary<TaskMode, int> ReadonlyModeCompleted => ModeCompleted;
        public IReadOnlyDictionary<SkillType, int> ReadonlySkillTypeMiddleRating => SkillTypeMiddleRating;
    }


    public class SkillSettingsData
    {
        public int Grade { get; set; } = 1;
        public SkillType Skill { get; set; }
        public bool IsEnabled { get; set; } = true;
        public int Value { get; set; } = 20;
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 100;
    }

    public class SkillPlanTableModel
    {
        public int Grade { get; set; }
        public string Skill { get; set; }
        public bool IsEnabled { get; set; }
        public int Value { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}


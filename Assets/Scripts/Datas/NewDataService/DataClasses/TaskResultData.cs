using System;
using System.Collections.Generic;

namespace Mathy.Data
{
    [Serializable]
    public class TaskResultData
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TaskMode Mode { get; set; }
        public TaskType TaskType { get; set; }
        public SkillType SkillType { get; set; }
        public List<string> ElementValues { get; set; } = new();
        public List<string> OperatorValues { get; set; } = new();
        public List<string> VariantValues { get; set; } = new();
        public List<int> SelectedAnswerIndexes { get; set; } = new();
        public List<int> CorrectAnswerIndexes { get; set; } = new();
        public bool IsAnswerCorrect { get; set; }
        public double Duration {get; set; }
        public int MaxValue { get; set; }
        public int Grade { get; set; }
    }
}

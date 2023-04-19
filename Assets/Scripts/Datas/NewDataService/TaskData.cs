using System;
using System.Collections.Generic;

namespace Mathy.Data
{
    [Serializable]
    public class TaskData
    {
        //Task seed
        public int Seed { get; set; }
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TaskMode Mode { get; set; }
        public int TaskModeIndex { get; set; }
        public TaskType TaskType { get; set; }
        public List<string> ElementValues { get; set; } = new();
        public List<string> OperatorValues { get; set; } = new();
        public List<string> VariantValues { get; set; } = new();
        public List<int> SelectedAnswerIndexes { get; set; } = new();
        public List<int> CorrectAnswerIndexes { get; set; } = new();
        public bool IsAnswerCorrect { get; set; }
        public bool IsModeDone { get; set; } = false;
        public double Duration {get; set; }
        public int MaxValue { get; set; }
    }
}

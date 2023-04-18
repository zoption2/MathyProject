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
        public List<string> ElementValues { get; set; }
        public List<string> OperatorValues { get; set; }
        public List<string> VariantValues { get; set; }
        public List<int> SelectedAnswerIndexes { get; set; }
        public List<int> CorrectAnswerIndexes { get; set; }
        public bool IsAnswerCorrect { get; set; }
        public bool IsModeDone { get; set; } = false;
        public double Duration {get; set; }
        public int MaxValue { get; set; }
    }

    [Serializable]
    public class TaskDataTableModel
    {
        public int ID { get; set; }
        public string Date { get; set; }
        public string Mode { get; set; }
        public int TaskModeIndex { get; set; }
        public string TaskType { get; set; }
        public int TaskTypeIndex { get; set; }
        public string ElementValues { get; set; }
        public string OperatorValues { get; set; }
        public string VariantValues { get; set; }
        public string SelectedAnswerIndexes { get; set; }
        public string CorrectAnswerIndexes { get; set; }
        public bool IsAnswerCorrect { get; set; }
        public double Duration { get; set; }
        public int MaxValue { get; set; }
    }
}

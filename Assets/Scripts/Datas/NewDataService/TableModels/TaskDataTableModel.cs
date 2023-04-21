using System;

namespace Mathy.Services.Data
{
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

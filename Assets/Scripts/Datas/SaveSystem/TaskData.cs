using System;
using System.Collections.Generic;

namespace Mathy.Data
{
    [Serializable]
    public class TaskData
    {
        //Task seed
        public int Seed { get; set; }
        public TaskMode Mode { get; set; }
        public TaskType TaskType { get; set; }
        public bool IsModeDone { get; set; } = false;
        //Task index in game mode, idk if its really necessary
        public int TaskModeIndex { get; set; }
        //Contains start date and time of the task
        public System.DateTime StartTime { get; set; }
        //Contains date and time when task is finnished
        public System.DateTime EndTime { get; set; }
        public TimeSpan Duration { get => (EndTime - StartTime); }
        public double TaskPlayDuration { get; set; }
        public List<int> SelectedAnswerIndexes { get; set; }
        public List<int> CorrectAnswerIndexes { get; set; }
        public bool IsAnswerCorrect { get; set; }
        public List<string> ElementValues { get; set; }
        public List<string> OperatorValues { get; set; }
        public List<string> VariantValues { get; set; }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

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
        public double Duration {get; set; }
        public List<int> SelectedAnswerIndexes { get; set; }
        public List<int> CorrectAnswerIndexes { get; set; }
        public bool IsAnswerCorrect { get; set; }
        public List<string> ElementValues { get; set; }
        public List<string> OperatorValues { get; set; }
        public List<string> VariantValues { get; set; }
    }
}

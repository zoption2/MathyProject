using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Data
{
    [Serializable]
    public class ChallengeData
    {
        public int Seed { get; set; }
        public TaskMode Mode { get; set; }
        public TaskType TaskType { get; set; }
        public TimeSpan Duration { get; set; }
        public int MaxNumber { get; set; }
        public bool IsDone { get; set; }
        public float CorrectRate { get; set; }
    }
}
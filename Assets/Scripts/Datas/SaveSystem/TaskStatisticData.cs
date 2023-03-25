using System;
using System.Collections.Generic;

namespace Mathy.Data
{
    [Serializable]
    public class TaskStatisticData
    {
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int TotalPlayed { get; set; }
        public double CorrectRate { get; set; }
        public double AverageTime { get; set; }      
    }
}

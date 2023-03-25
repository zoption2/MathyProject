using System.Collections.Generic;
using System.Threading.Tasks;
using CustomRandom;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class PairsNumbers : PairsTask
    {
        public PairsNumbers(int seed, ScriptableTask taskSettings)
        {
            this.TaskSettings = taskSettings;
            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;

            CreateTaskElementsAsync();
        }


    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using CustomRandom;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class OpenImage : OpenImageTask
    {
        public OpenImage(int seed, ScriptableTask taskSettings)
        {
            this.variants = new List<Variant>();
            this.operators = new List<Operator>();

            this.TaskSettings = taskSettings;

            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;
            this.LivesAmount = 3;//by default 3 hp

            CreateTaskElementsAsync();
        }


        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            int oprIndex = 0;
            List<ArithmeticSigns> signs = new List<ArithmeticSigns>() { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

            while (oprIndex < TaskSettings.ElementsAmount - 1)
            {
                this.operators.Add(new Operator(signs[Random.Range(0, 1)]));
                oprIndex++;
            }
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }

    }
}
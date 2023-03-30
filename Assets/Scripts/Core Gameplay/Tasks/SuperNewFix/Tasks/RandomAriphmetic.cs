using System.Collections.Generic;
using CustomRandom;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class RandomAriphmetic : ArithmeticTask
    {
        public RandomAriphmetic(int seed, ScriptableTask taskSettings)
        {
            this.variants = new List<Variant>();
            this.operators = new List<Operator>();

            this.TaskSettings = taskSettings;

            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;

            CreateTaskElementsAsync();
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            int oprIndex = 0;
            List<ArithmeticSigns> signs = 
                new List<ArithmeticSigns>() { ArithmeticSigns.Plus, ArithmeticSigns.Minus, 
                    ArithmeticSigns.Multiply, ArithmeticSigns.Divide };

            while (oprIndex < TaskSettings.ElementsAmount - 1)
            {
                this.operators.Add(new Operator(signs[Random.Range(0, 3)]));
                oprIndex++;
            }
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }


    }
}
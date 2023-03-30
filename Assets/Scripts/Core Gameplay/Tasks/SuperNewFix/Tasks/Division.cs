using System.Collections.Generic;
using CustomRandom;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class Division : ArithmeticTask
    {
        public Division(int seed, ScriptableTask taskSettings)
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
            while (oprIndex < TaskSettings.ElementsAmount - 1)
            {
                this.operators.Add(new Operator(ArithmeticSigns.Divide));
                oprIndex++;
            }
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }
    }
}
using System.Collections.Generic;
using CustomRandom;
using Cysharp.Threading.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class MissingNumber : SequenceTask
    {
        public MissingNumber(int seed, ScriptableTask taskSettings)
        {
            this.variants = new List<Variant>();
            this.operators = new List<Operator>();

            this.TaskSettings = taskSettings;

            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;

            CreateTaskElementsAsync();
        }

        protected override async System.Threading.Tasks.Task CreateElements()
        {
            int startValue = Random.Range(TaskSettings.MinNumber, TaskSettings.MaxNumber);

            bool isPositive = startValue + (TaskSettings.ElementsAmount - 1) < TaskSettings.MaxNumber;

            for (int i = 0; i < TaskSettings.ElementsAmount; i++)
            {
                if (isPositive)
                {
                    this.Elements.Add(new TaskElement(startValue + i));
                }
                else
                {
                    this.Elements.Add(new TaskElement(startValue - i));
                }
            }
        }
        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            unknownElementIndex = Random.Range(0, TaskSettings.ElementsAmount - 1);
            this.unknownElementIndex = unknownElementIndex;
            int answer = (int)Elements[unknownElementIndex].Value;
            //making element value uknown
            Elements[unknownElementIndex] = new TaskElement(ArithmeticSigns.QuestionMark);
            int answerIndex = Random.Range(0, TaskSettings.VariantsAmount - 1);
            CorrectVariantIndexes.Add(answerIndex);

            List<int> variants = await Random.ExclusiveNumericRange(TaskSettings.MinNumber, TaskSettings.MaxNumber, TaskSettings.VariantsAmount, answer);

            for (int i = 0; i < TaskSettings.VariantsAmount; i++)
            {
                if (i == answerIndex)
                {
                    this.variants.Add(new Variant(answer, true));
                }
                else
                {
                    this.variants.Add(new Variant(variants[i], false));
                }
            }
            /*
            int index = 0;
            while (index < TaskSettings.BaseStats.VariantsAmount)
            {
                int value = Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber);
                if (index == answerIndex)
                {
                    this.variants.Add(new Variant(answer, true));
                    index++;
                }
                else
                {
                    if (value != answer)
                    {
                        this.variants.Add(new Variant(value, false));
                        index++;
                    }
                }
            }
            */

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }


        protected async override UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            unknownElement = this.Elements[unknownElementIndex].ElementView;
        }
    }
}
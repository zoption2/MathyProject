using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CustomRandom;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class Comparison : ComparisonTask
    {
        public Comparison(int seed, ScriptableTask taskSettings)
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
            for (int i = 0; i < TaskSettings.BaseStats.ElementsAmount; i++)
            {
                this.Elements.Add(new TaskElement(this.Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber)));
            }
        }

        protected async override UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            //Bad code, need to refactor later
            uknownElement = this.operators[0].ElementView;
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            //Only question mark coz operator is always unknown
            this.operators.Add(new Operator(ArithmeticSigns.QuestionMark));
        }

        //refactor a bit later
        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            ArithmeticSigns answerSign = Compare(Elements[0].Value, Elements[1].Value);

            for(int i = 0; i < Signs.Count; i++)
            {
                if (Signs[i] == answerSign)
                {
                    this.variants.Add(new Variant(Signs[i], true));
                    this.CorrectVariantIndexes.Add(i);
                }
                else
                {
                    this.variants.Add(new Variant(Signs[i], false));
                }
            }

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override ArithmeticSigns Compare(object value1, object value2)
        {
            if ((int)value1 == (int)value2)
            {
                return ArithmeticSigns.Equal;
            }
            else if ((int)value1 < (int)value2)
            {
                return ArithmeticSigns.LessThan;
            }
            else
            {
                return ArithmeticSigns.GreaterThan;
            }
        }
    }
}
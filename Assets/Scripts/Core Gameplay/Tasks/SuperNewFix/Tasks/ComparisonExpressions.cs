using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CustomRandom;
using System;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class ComparisonExpressions : ComparisonTask
    {
        public ComparisonExpressions(int seed, ScriptableTask taskSettings)
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
            for (int i = 0; i < TaskSettings.ElementsAmount; i++)
            {
                int value = this.Random.Range(TaskSettings.MinNumber, TaskSettings.MaxNumber);
                this.Elements.Add(new TaskElement(value));
            }
        }

        protected async override UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            //Bad code, need to refactor later
            uknownElement = this.operators[0].ElementView;
            foreach (Element element in Elements)
            {
                (int x, int y, string expression) = GetSumDiffPair((int)element.Value);
                element.ElementView.Value = expression;
            }
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

            List<ArithmeticSigns> tempSigns =
                new List<ArithmeticSigns>() { ArithmeticSigns.LessThan, ArithmeticSigns.Equal, ArithmeticSigns.GreaterThan };

            for (int i = 0; i < tempSigns.Count; i++)
            {
                if (tempSigns[i] == answerSign)
                {
                    this.variants.Add(new Variant(tempSigns[i], true));
                    this.CorrectVariantIndexes.Add(i);
                }
                else
                {
                    this.variants.Add(new Variant(tempSigns[i], false));
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

        public (int, int, string) GetSumDiffPair(int n)
        {
            System.Random rnd = new System.Random();
            int x, y;
            string expression;

            if (rnd.Next(2) == 0)
            {
                // Return a pair that gives n when x + y is calculated
                if (n >= 0)
                {
                    x = rnd.Next(n + 1);
                    y = n - x;
                    expression = $"{x} + {y}";
                }
                else
                {
                    x = rnd.Next(-n + 1);
                    y = x - n;
                    expression = $"{x} + {y}";
                }
            }
            else
            {
                // Return a pair that gives n when x - y is calculated
                if (n >= 0)
                {
                    y = rnd.Next(n + 1);
                    x = n + y;
                    expression = $"{x} - {y}";
                }
                else
                {
                    y = rnd.Next(-n + 1);
                    x = y - n;
                    expression = $"{x} - {y}";
                }
            }

            return (x, y, expression);
        }
    }
}
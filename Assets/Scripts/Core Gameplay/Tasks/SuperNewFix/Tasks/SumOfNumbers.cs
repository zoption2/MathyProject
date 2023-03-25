using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CustomRandom;
using System;
using System.Linq;
using Mathy.UI.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class SumOfNumbers : ArithmeticTask
    {
        public List<int> CorrectVariantsValues { get; private set; }
        public List<Variant> SelectedVariants { get; private set; } = new List<Variant>();

        public SumOfNumbers(int seed, ScriptableTask taskSettings)
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
                this.Elements.Add(new TaskElement(ArithmeticSigns.QuestionMark));
            }
            int answer = this.Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber);
            this.Elements.Add(new TaskElement(answer));
            CorrectVariantsValues = SplitNumberIntoAddends(answer, TaskSettings.BaseStats.ElementsAmount).ToList();
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            int oprIndex = 0;
            List<ArithmeticSigns> signs = new List<ArithmeticSigns>()
            { ArithmeticSigns.Plus };

            while (oprIndex < TaskSettings.BaseStats.OperatorsAmount - 1)
            {
                this.operators.Add(new Operator(signs[Random.Range(0, signs.Count)]));
                oprIndex++;
            }
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }

        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            var randomizedVariantIndexes = Enumerable.Range(0, TaskSettings.BaseStats.VariantsAmount - 1).ToList().
                OrderBy(x => Guid.NewGuid()).ToList();
            var answerIndexes = randomizedVariantIndexes.Take(TaskSettings.BaseStats.ElementsAmount).ToList();

            CorrectVariantIndexes = answerIndexes;

            List<int> variantsValues = await Random.ExclusiveNumericRange(
                TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, 
                TaskSettings.BaseStats.VariantsAmount, CorrectVariantsValues);
            var correctValues = CorrectVariantsValues;

            for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
            {
                if (CorrectVariantIndexes.Contains(i))
                {
                    this.variants.Add(new Variant(correctValues.First(), true));
                    correctValues.RemoveAt(0);
                }
                else
                {
                    this.variants.Add(new Variant(variantsValues[i], false));
                }
            }

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override async UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
        }

        protected async override void VariantOnPressedEvent(object sender, EventArgs e)
        {
            Variant variant = (Variant)sender;
            SelectedVariants.Add(variant);
            int index = SelectedVariants.IndexOf(variant);
            SelectedVariantIndexes.Add(variants.FindIndex(v => v == variant));

            if (SelectedVariants.Count < TaskSettings.BaseStats.ElementsAmount)
            {
                SetViewElementAnswer(Elements[index].ElementView, variant.Value);

                if (!variant.IsVariantCorrect)
                {
                    foreach (Variant var in this.variants)
                    {
                        ((VariantView)var.ElementView).SetInteractable(false);
                    }
                    TaskManager.Instance.WrongAnswer();
                    SaveResult();
                }
            }
            else
            {
                foreach (Variant var in this.variants)
                {
                    ((VariantView)var.ElementView).SetInteractable(false);
                }

                SetViewElementAnswer(Elements[index].ElementView, variant.Value);

                if (variant.IsVariantCorrect)
                {
                    TaskManager.Instance.CorrectAnswer();
                    SaveResult();
                }
                else
                {
                    TaskManager.Instance.WrongAnswer();
                    SaveResult();
                }
            }
        }

        private void SetViewElementAnswer(TaskViewElement uknownElement, object variantValue)
        {
            if (uknownElement != null)
            {
                TextTaskViewElement element = (TextTaskViewElement)uknownElement;
                element.SetState(TaskElementState.Default);
                element.Value = variantValue;
            }
        }

        public static int[] SplitNumberIntoAddends(int numToSplit, int amount)
        {
            int[] results = new int[amount];
            int sum = 0;

            for (int i = 0; i < amount - 1; i++)
            {
                int maxVal = numToSplit - sum - (amount - i - 1); // Maximum value for this integer
                int value = 0;
                if (maxVal > 0) // Only generate positive integers
                {
                    value = new System.Random().Next(1, maxVal + 1); // Generate random integer between 1 and maxVal
                }
                results[i] = value;
                sum += value;
            }

            results[amount - 1] = numToSplit - sum;

            if (results[amount - 1] < 0) // If sum is greater than numToSplit, set all values to 0
            {
                for (int i = 0; i < amount - 1; i++)
                {
                    results[i] = 0;
                }
                results[amount - 1] = numToSplit;
            }

            return results;
        }

        //public static int[] SplitNumberIntoAddends(int numToSplit, int amount)
        //{
        //    int[] results = new int[amount];
        //    if (numToSplit == 0)
        //    {
        //        return results;
        //    }
        //    if (numToSplit == 1)
        //    {
        //        results[0] = 1;
        //        return results;
        //    }

        //    int sum = 0;

        //    for (int i = 0; i < amount - 1; i++)
        //    {
        //        int maxVal = numToSplit - sum - (amount - i - 1); // Calculate maximum value for this integer
        //        int value = new System.Random().Next(1, maxVal + 1); // Generate random integer between 1 and maxVal
        //        results[i] = value;
        //        sum += value;
        //    }

        //    results[amount - 1] = numToSplit - sum;

        //    return results;
        //}
    }
}
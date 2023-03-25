using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using CustomRandom;
using System.Linq;
using TMPro;
using UnityEngine;
using Mathy.Core;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class MissingExpression : ArithmeticTask
    {
        public MissingExpression(int seed, ScriptableTask taskSettings)
        {
            this.variants = new List<Variant>();
            this.operators = new List<Operator>();

            this.TaskSettings = taskSettings;

            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;

            _ = CreateTaskElementsAsync();

            //Debug.LogError("Missing expression!!");
        }

        protected override async System.Threading.Tasks.Task CreateElements()
        {
            int value = this.Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber);
            this.Elements.Add(new TaskElement(ArithmeticSigns.QuestionMark));
            for (int i = 0; i < TaskSettings.BaseStats.ElementsAmount; i++)
            {
                this.Elements.Add(new TaskElement(value));
            }
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            int oprIndex = 0;
            List<ArithmeticSigns> signs = new List<ArithmeticSigns>()
            { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

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
            int answer = (int)Elements.Last().Value;
            int answerIndex = Random.Range(0, TaskSettings.BaseStats.VariantsAmount - 1);
            CorrectVariantIndexes.Add(answerIndex);

            List<int> variants = await Random.ExclusiveNumericRange(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, TaskSettings.BaseStats.VariantsAmount, answer);

            for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
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

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override async UniTask InitializeVariantsView()
        {
            foreach (Variant variant in this.variants)
            {
                await variant.CreateView(((DefaultTaskBehaviour)TaskBehaviour).VariantsPanel, this.TaskType);
                //(int x, int y, string expression) = GetSumDiffPair((int)variant.Value);
                (int x, int y, string expression) = MathOperations.GetSumDiffPair((int)variant.Value);

                ((VariantView)variant.ElementView).Value = expression;
            }
        }

        protected override async UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            unknownElement = this.Elements[0].ElementView;
        }
    }
}
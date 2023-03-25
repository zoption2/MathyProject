using System.Collections.Generic;
using CustomRandom;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class IsThatTrue : ArithmeticTask
    {
        public IsThatTrue(int seed, ScriptableTask taskSettings)
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
            List<ArithmeticSigns> signs = new List<ArithmeticSigns>() { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

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
            string expression = GetExpression;

            int realTaskAnswer = MathOperations.EvaluateInt(expression);
            int taskAnswer = 0;

            if (this.Random.TossACoin())
            {
                taskAnswer = realTaskAnswer;
            }
            else
            {
                taskAnswer = this.Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber);
            }
            this.Elements[Elements.Count - 1] = new TaskElement(taskAnswer);

            if (realTaskAnswer == taskAnswer)
            {
                this.variants.Add(new Variant(true, true));
                this.variants.Add(new Variant(false, false));
                CorrectVariantIndexes.Add(0);
            }
            else
            {
                this.variants.Add(new Variant(true, false));
                this.variants.Add(new Variant(false, true));
                CorrectVariantIndexes.Add(1);
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
    }
}
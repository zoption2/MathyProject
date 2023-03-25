using System.Collections.Generic;
using CustomRandom;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class MissingSign : ArithmeticTask
    {
        public MissingSign(int seed, ScriptableTask taskSettings)
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
                int minNumber = TaskSettings.BaseStats.MinNumber;
                //seeting minimal number to 1 to prevent some task glitches
                if(TaskSettings.BaseStats.MinNumber == 0)
                {
                    minNumber = 1;
                }
                this.Elements.Add(new TaskElement(this.Random.Range(minNumber, TaskSettings.BaseStats.MaxNumber)));
            }
            this.Elements.Add(new TaskElement(ArithmeticSigns.QuestionMark));

            string expression = GetExpression;
            int answer = MathOperations.EvaluateInt(expression);
            if (answer < 0 || answer > TaskSettings.BaseStats.MaxNumber)
            {
                await ClearElements();
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
            string expression = GetExpression;

            int taskAnswer = MathOperations.EvaluateInt(expression);
            this.Elements[Elements.Count-1] = new TaskElement(taskAnswer);

            ArithmeticSigns answerSign = (ArithmeticSigns)this.operators[0].Value;
            List<ArithmeticSigns> tempSigns =
                new List<ArithmeticSigns>() { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

            for (int i = 0; i < tempSigns.Count; i++)
            {
                if (tempSigns[i] == answerSign)
                {
                    this.variants.Add(new Variant(answerSign, true));
                    CorrectVariantIndexes.Add(i);
                }
                else
                {
                    this.variants.Add(new Variant(tempSigns[i], false));
                }
            }

            for (int i = 0; i < operators.Count-1; i++) 
            {
                this.operators[i] = new Operator(ArithmeticSigns.QuestionMark);
            }

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override async UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            unknownElement = this.operators[0].ElementView;
        }
    }
}
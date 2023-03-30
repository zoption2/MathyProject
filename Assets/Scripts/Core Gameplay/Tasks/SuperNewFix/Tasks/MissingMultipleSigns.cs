using System;
using System.Collections.Generic;
using CustomRandom;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class MissingMultipleSigns : ArithmeticTask
    {
        private ArithmeticSigns[] signs = new ArithmeticSigns[2];
        private bool isFirstVariantSelected = false;

        public MissingMultipleSigns(int seed, ScriptableTask taskSettings)
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
            List<ArithmeticSigns> signs = new List<ArithmeticSigns>() 
            { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

            while (oprIndex < TaskSettings.ElementsAmount - 1)
            {
                this.operators.Add(new Operator(signs[Random.Range(0, signs.Count)]));
                oprIndex++;
            }
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }

        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            if (!isFirstVariantSelected)
            {
                string expression = GetExpression;
                int expressionAnswer = MathOperations.EvaluateInt(expression);
                this.Elements[Elements.Count - 1] = new TaskElement(expressionAnswer);

                for (int i = 0; i < operators.Count - 1; i++)
                {
                    signs[i] = (ArithmeticSigns)this.operators[i].Value;
                    this.operators[i] = new Operator(ArithmeticSigns.QuestionMark);
                }

                List<ArithmeticSigns> tempSigns =
                    new List<ArithmeticSigns>() { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

                for (int i = 0; i < tempSigns.Count; i++)
                {
                    if (tempSigns[i] == signs[0])
                    {
                        this.variants.Add(new Variant(signs[0], true));
                    }
                    else
                    {
                        this.variants.Add(new Variant(tempSigns[i], false));
                    }
                }
            }
            else
            {
                List<ArithmeticSigns> tempSigns =
                    new List<ArithmeticSigns>() { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

                for (int i = 0; i < tempSigns.Count; i++)
                {
                    if (tempSigns[i] == signs[1])
                    {
                        this.variants.Add(new Variant(signs[1], true));
                    }
                    else
                    {
                        this.variants.Add(new Variant(tempSigns[i], false));
                    }
                }
            }

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected async override void VariantOnPressedEvent(object sender, EventArgs e)
        {
            Variant variant = (Variant)sender;

            if (!isFirstVariantSelected)
            {
                if (!variant.IsVariantCorrect)
                {
                    TaskManager.Instance.WrongAnswer();
                }

                foreach (Variant var in this.variants)
                {
                    ((VariantView)var.ElementView).SetInteractable(false);
                }

                SetViewElementAnswer(operators[0].ElementView, (ArithmeticSigns)variant.Value);

                for (int i = 0; i < variants.Count; i++)
                {
                    await variants[i].DisposeAsync();
                }
                variants.Clear();
                isFirstVariantSelected = true;

                await CreateVariants();
                await InitializeVariantsView();

            }
            else
            {
                SetViewElementAnswer(operators[1].ElementView, (ArithmeticSigns)variant.Value);
                foreach (Variant var in this.variants)
                {
                    ((VariantView)var.ElementView).SetInteractable(false);
                }
                if (variant.IsVariantCorrect)
                {
                    TaskManager.Instance.CorrectAnswer();
                }
                else
                {
                    TaskManager.Instance.WrongAnswer();
                }
            }
        }

        private void SetViewElementAnswer(TaskViewElement uknownElement, object variantValue)
        {
            if (uknownElement != null)
            {
                OperatorView element = ((OperatorView)uknownElement);
                element.SetState(TaskElementState.Default);
                element.Value = variantValue;
            }
        }


        protected override async UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            unknownElement = this.Elements[Elements.Count - 1].ElementView;
        }

    }
}
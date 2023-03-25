using System;
using System.Collections.Generic;
using CustomRandom;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class ComparisonMissingElements : ComparisonTask
    {
        private ArithmeticSigns sign;

        private bool isFirstVariantSelected = false;
        private int firstElement;

        public ComparisonMissingElements(int seed, ScriptableTask taskSettings)
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
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            List<ArithmeticSigns> tempSigns =
                    new List<ArithmeticSigns>() { ArithmeticSigns.LessThan, ArithmeticSigns.Equal, ArithmeticSigns.MoreThan };
            sign = tempSigns[Random.Range(0, tempSigns.Count)];
            this.operators.Add(new Operator(sign));
        }

        //Need to be SERIOUSLY refactored, very bad code
        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            if (!isFirstVariantSelected)
            {
                if(variants.Count == 0) 
                {
                    List<int> variantsTemp = await Random.ExclusiveNumericRange(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, TaskSettings.BaseStats.VariantsAmount, -1);
                    for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
                    {
                        this.variants.Add(new Variant(variantsTemp[i], true));
                    }
                }
            }
            else
            {
                List<int> variantsTemp = await Random.ExclusiveNumericRange(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, TaskSettings.BaseStats.VariantsAmount, -1);
                bool isCorrectAnswExists = false;
                for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
                {
                    //if last and still no correct - set it
                    if (i == (TaskSettings.BaseStats.VariantsAmount - 1))
                    {
                        if (!isCorrectAnswExists)
                        {
                            if (sign == ArithmeticSigns.Equal)
                            {
                                this.variants.Add(new Variant(firstElement, true));
                            }
                            else if (sign == ArithmeticSigns.LessThan)
                            {
                                this.variants.Add(new Variant(Random.Range(firstElement, TaskSettings.BaseStats.MaxNumber+1), true));
                            }
                            else if (sign == ArithmeticSigns.MoreThan)
                            {
                                this.variants.Add(new Variant(Random.Range(TaskSettings.BaseStats.MinNumber, firstElement), true));
                            }
                        }
                        else
                        {
                            if (sign == Compare(firstElement, variantsTemp[i]))
                            {
                                this.variants.Add(new Variant(variantsTemp[i], true));
                                isCorrectAnswExists = true;
                            }
                            else
                            {
                                this.variants.Add(new Variant(variantsTemp[i], false));
                            }
                        }

                    }
                    else
                    {
                        if (sign == Compare(firstElement, variantsTemp[i]))
                        {
                            this.variants.Add(new Variant(variantsTemp[i], true));
                            isCorrectAnswExists = true;
                        }
                        else
                        {
                            this.variants.Add(new Variant(variantsTemp[i], false));
                        }
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
                firstElement = (int)variant.Value;
                this.Elements[0].ElementView.Value = variant.Value;

                foreach (Variant var in this.variants)
                {
                    ((VariantView)var.ElementView).SetInteractable(false);
                }

                for (int i = 0; i < variants.Count; i++)
                {
                    await variants[i].DisposeAsync(); 
                }
                variants.Clear();
                isFirstVariantSelected = true;

                await CreateVariants();
                await InitializeVariantsView();

                SetViewElementAnswer(Elements[0].ElementView, variant.Value);
            }
            else
            {
                SetViewElementAnswer(Elements[1].ElementView, variant.Value);
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
                TextTaskViewElement element = ((TextTaskViewElement)uknownElement);
                element.SetState(TaskElementState.Default);
                element.Value = variantValue;
            }
        }

        protected override ArithmeticSigns Compare(object value1, object value2)
        {
            if ((int)value1 == (int)value2)
            {
                //Debug.LogError((int)value1 + " = " + (int)value2);
                return ArithmeticSigns.Equal;
            }
            else if ((int)value1 < (int)value2)
            {
                //Debug.LogError((int)value1 + " < " + (int)value2);
                return ArithmeticSigns.LessThan;
            }
            else if ((int)value1 > (int)value2)
            {
                //Debug.LogError((int)value1 + " > " + (int)value2);
                return ArithmeticSigns.MoreThan;
            }
            throw new ArgumentOutOfRangeException();
        }

    }
}

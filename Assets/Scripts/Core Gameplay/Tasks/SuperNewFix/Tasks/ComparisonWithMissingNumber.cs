using System;
using System.Collections.Generic;
using System.Linq;
using CustomRandom;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class ComparisonWithMissingNumber : ComparisonTask
    {
        private ArithmeticSigns sign;
        private int unknownElementIndex;

        public ComparisonWithMissingNumber(int seed, ScriptableTask taskSettings)
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
            uknownElement = this.Elements[this.unknownElementIndex].ElementView;
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            sign = Compare(Elements[0].Value, Elements[1].Value);
            this.operators.Add(new Operator(sign));
        }

        //Need to be SERIOUSLY refactored, very bad code
        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            int unknownElementIndex = this.Random.Range(0, Elements.Count - 1);

            this.unknownElementIndex = unknownElementIndex;
            this.Elements[unknownElementIndex] = new TaskElement(ArithmeticSigns.QuestionMark);

            List<int> variants = await Random.ExclusiveNumericRange(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, TaskSettings.BaseStats.VariantsAmount, -1);

            int knownElement = (int)this.Elements.Find(x => x.Value.GetType() != typeof(ArithmeticSigns)).Value;

            bool isCorrectAnswExists = false;
            for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
            {
                if(i == TaskSettings.BaseStats.VariantsAmount - 1)
                {
                    if (!isCorrectAnswExists)
                    {
                        if(sign == ArithmeticSigns.Equal)
                        {
                            this.variants.Add(new Variant(knownElement, true));                            
                        }
                        else if (sign == ArithmeticSigns.LessThan)
                        {
                            this.variants.Add(new Variant(Random.Range(TaskSettings.BaseStats.MinNumber, knownElement), true));
                        }
                        else if(sign == ArithmeticSigns.MoreThan)
                        {
                            this.variants.Add(new Variant(Random.Range(knownElement, TaskSettings.BaseStats.MaxNumber), true));
                        }
                        CorrectVariantIndexes.Add(i);
                    }
                    else
                    {
                        if (sign == Compare(variants[i], knownElement))
                        {
                            this.variants.Add(new Variant(variants[i], true));
                            isCorrectAnswExists = true;
                            CorrectVariantIndexes.Add(i);
                        }
                        else
                        {
                            this.variants.Add(new Variant(variants[i], false));
                        }
                    }
                    //if last and still no correct - set it
                }
                else
                {
                    if (sign == Compare(variants[i], knownElement))
                    {
                        this.variants.Add(new Variant(variants[i], true));
                        isCorrectAnswExists = true;
                        CorrectVariantIndexes.Add(i);
                    }
                    else
                    {
                        this.variants.Add(new Variant(variants[i], false));
                    }
                }

            }

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override void VariantOnPressedEvent(object sender, EventArgs e)
        {
            Variant variant = (Variant)sender;
            int variantIndex = variants.FindIndex(a => a == variant);
            SelectedVariantIndexes.Add(variantIndex);

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
            SetViewElementAnswer(variant.Value);
            SaveResult();
        }

        protected override void SetViewElementAnswer(object variantValue)
        {
            if (uknownElement != null)
            {
                TextTaskViewElement element = ((TextTaskViewElement)uknownElement);
                element.SetState(TaskElementState.Correct);
                element.Value = variantValue;
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
                return ArithmeticSigns.MoreThan;
            }
        }

        protected override void SaveResult()
        {
            TaskManager.Instance.SaveTaskData(
                this.TaskType,
                this.CorrectVariantIndexes.Intersect(this.SelectedVariantIndexes).Any(),
                this.SelectedVariantIndexes,
                this.CorrectVariantIndexes,
                this.Elements,
                this.operators,
                this.variants);
        }
    }
}
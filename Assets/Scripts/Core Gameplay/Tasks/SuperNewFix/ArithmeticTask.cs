using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public abstract class ArithmeticTask : Task
    {
        //Temp public
        public List<Variant> variants;
        public List<Operator> operators;
        public List<int> SelectedVariantIndexes { get; protected set; } = new List<int>();
        public List<int> CorrectVariantIndexes { get; protected set; } = new List<int>();
        public int VariantsAmount
        {
            get => variants.Count;
        }

        protected TaskViewElement unknownElement;

        public override string GetExpression
        {
            get
            {
                string expression = "";

                //Element amount -1 coz last element is ? mark
                for (int i = 0; i < (ElementsAmount - 1); i++)
                {
                    expression += ((int)Elements[i].Value).ToString();
                    //Skip the last operator coz last is always =
                    if (i < operators.Count - 1)
                    {
                        expression += (char)((ArithmeticSigns)operators[i].Value);
                    }
                }
                return expression;
            }
        }

        protected override async System.Threading.Tasks.Task CreateTaskElementsAsync()
        {
            await CreateOperators();
            await CreateElements();
            await CreateVariants();
        }

        protected virtual async System.Threading.Tasks.Task CreateElements()
        {
            for (int i = 0; i < TaskSettings.BaseStats.ElementsAmount; i++)
            {
                this.Elements.Add(new TaskElement(this.Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber)));
            }
            this.Elements.Add(new TaskElement(ArithmeticSigns.QuestionMark));


            string expression = GetExpression;
            int answer = MathOperations.EvaluateInt(expression);
            if (answer < 0 || answer > TaskSettings.BaseStats.MaxNumber)
            {
                await ClearElements();
            }
            
        }

        protected abstract System.Threading.Tasks.Task CreateOperators();
        protected virtual async System.Threading.Tasks.Task CreateVariants()
        {
            string expression = GetExpression;
            int answer = MathOperations.EvaluateInt(expression);
            int answerIndex = Random.Range(0, TaskSettings.BaseStats.VariantsAmount - 1);
            CorrectVariantIndexes.Add(answerIndex);

            List<int> variants = await Random.ExclusiveNumericRange(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, TaskSettings.BaseStats.VariantsAmount, answer);

            for(int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
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

        protected virtual async UniTask ClearElements()
        { 
            for(int i = 0; i < Elements.Count; i++)
            {
                await Elements[i].DisposeAsync();
            }
            Elements.Clear();
            await CreateElements();
        }

        protected virtual async UniTask InitializeVariantsView()
        {
            foreach (Variant variant in this.variants)
            {
                await variant.CreateView(((DefaultTaskBehaviour)TaskBehaviour).VariantsPanel, this.TaskType);
            }
        }

        protected virtual async UniTask InitializeElementsView()
        {
            for (int i = 0; i < ElementsAmount; i++)
            {
                await this.Elements[i].CreateView(((DefaultTaskBehaviour)TaskBehaviour).ElementsPanel, this.TaskType)
                    .ContinueWith(async () =>
                    {
                        if (i < operators.Count && operators.Count != 0)
                        {
                            await this.operators[i].CreateView(((DefaultTaskBehaviour)TaskBehaviour).ElementsPanel, this.TaskType);
                        }
                    });
            }
        }

        protected virtual void VariantOnPressedEvent(object sender, EventArgs e)
        {
            Variant variant = (Variant)sender;
            SelectedVariantIndexes.Add(variants.FindIndex(a => a == variant));

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

            SetViewElementAnswer();
            SaveResult();
        }


        public override async UniTask CreateTaskView(Transform gameplayPanel)
        {
            using (AddressableResourceLoader<GameObject> loader = new AddressableResourceLoader<GameObject>())
            {
                var gameObject = await loader.LoadAndInstantiateSingle("TaskView", gameplayPanel);
                this.TaskBehaviour = gameObject.GetComponent<DefaultTaskBehaviour>();
            }
            await this.TaskBehaviour.Initialize(this);

            await UniTask.WhenAll(InitializeElementsView(), InitializeVariantsView());
        }

        public override async UniTask DisposeTaskView()
        {
            if (this.TaskBehaviour != null)
            {
                try
                {
                    System.Threading.Tasks.Task timer = System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.5f)); // Next task display delay
                    var completedTask = await System.Threading.Tasks.Task.WhenAny(timer);
                    if (completedTask.IsCompleted)
                    {
                        await TaskBehaviour.DisposeAsync();
                        TaskBehaviour = null;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("Error occurred" + e);
                    return;
                }
            }
        }

        protected override void SaveResult()
        {
            TaskManager.Instance.SaveTaskData(
                this.TaskType,
                this.CorrectVariantIndexes.All(this.SelectedVariantIndexes.Contains) &&
                CorrectVariantIndexes.Count == this.SelectedVariantIndexes.Count,
                this.SelectedVariantIndexes, 
                this.CorrectVariantIndexes,
                this.Elements,
                this.operators, 
                this.variants);
        }

        protected virtual void SetViewElementAnswer()
        {
            if (unknownElement != null)
            {
                TextTaskViewElement element = ((TextTaskViewElement)unknownElement);

                element.SetState(TaskElementState.Correct);
                element.Value = this.variants[CorrectVariantIndexes[0]].Value;
            }
        }
    }
}
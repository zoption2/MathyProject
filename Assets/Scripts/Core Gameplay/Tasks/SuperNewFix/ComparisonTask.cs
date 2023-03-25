using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public abstract class ComparisonTask : Task
    {
        protected abstract ArithmeticSigns Compare(object value1, object value2);
        //Temp public
        public List<Variant> variants;
        protected List<Operator> operators;
        public List<int> SelectedVariantIndexes { get; protected set; } = new List<int>();
        public List<int> CorrectVariantIndexes { get; protected set; } = new List<int>();
        public int VariantsAmount
        {
            get => variants.Count;
        }

        public override string GetExpression
        {
            get
            {
                string expression = "";

                //Element amount -1 coz last element is ? mark
                for (int i = 0; i < Elements.Count; i++)
                {
                    expression += ((int)Elements[i].Value).ToString();
                    //Skip the last operator coz last is always =
                    if (i < operators.Count)
                    {
                        expression += (char)(ArithmeticSigns)operators[i].Value;
                    }
                }
                return expression;
            }
        }

        public List<ArithmeticSigns> Signs =
                new List<ArithmeticSigns>() { ArithmeticSigns.LessThan, ArithmeticSigns.Equal, ArithmeticSigns.MoreThan };

        protected TaskViewElement uknownElement;

        protected override async System.Threading.Tasks.Task CreateTaskElementsAsync()
        {
            await CreateElements();
            await CreateOperators();
            await CreateVariants();
        }

        protected abstract System.Threading.Tasks.Task CreateElements();
        protected abstract System.Threading.Tasks.Task CreateOperators();
        protected abstract System.Threading.Tasks.Task CreateVariants();

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
            SetViewElementAnswer(this.variants[CorrectVariantIndexes[0]].Value);
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
            //Debug.Log("DisposeTaskView Comparison");
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
                    /*
                    Debug.Log("Trying to destroy Comparison");
                    await this.TaskBehaviour.DisposeAsync();
                    TaskBehaviour = null;
                    */
                }
                catch (Exception e)
                {
                    Debug.Log("Error occurred" + e);
                    return;
                }
            }
        }

        protected virtual void SetViewElementAnswer(object variantValue)
        {
            if (uknownElement != null)
            {
                OperatorView element = ((OperatorView)uknownElement);

                element.SetState(TaskElementState.Correct);
                element.Value = variantValue;
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
    }
}
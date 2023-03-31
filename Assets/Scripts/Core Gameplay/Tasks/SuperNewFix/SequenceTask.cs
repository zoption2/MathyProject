using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    //Rename it to something better
    public abstract class SequenceTask : Task
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

        public int unknownElementIndex { get; protected set; }

        public override string GetExpression
        {
            get
            {
                string expression = "";

                for (int i = 0; i < Elements.Count; i++)
                {
                    expression += (i != unknownElementIndex) ? $"{((int)Elements[i].Value)} " : " ? ";
                }
                return expression;
            }
        }

        protected TaskViewElement unknownElement;

        protected override async System.Threading.Tasks.Task CreateTaskElementsAsync()
        {
            await CreateElements();
            await CreateVariants();
        }

        protected virtual async System.Threading.Tasks.Task CreateElements()
        {
            for (int i = 0; i < TaskSettings.ElementsAmount; i++)
            {
                this.Elements.Add(new TaskElement(this.Random.Range(TaskSettings.MinNumber, TaskSettings.MaxNumber)));
            }
            this.Elements.Add(new TaskElement(ArithmeticSigns.QuestionMark));
        }

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
                await this.Elements[i].CreateView(((DefaultTaskBehaviour)TaskBehaviour).ElementsPanel, this.TaskType);
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
            //Debug.Log("DisposeTaskView MissingEle");
            if (this.TaskBehaviour != null)
            {
                try
                {
                    //Temp for delay
                    System.Threading.Tasks.Task timer = System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.5f)); // Next task display delay
                    var completedTask = await System.Threading.Tasks.Task.WhenAny(timer);
                    if (completedTask.IsCompleted)
                    {
                        await TaskBehaviour.DisposeAsync();
                        TaskBehaviour = null;
                    }

                    /*
                    Debug.Log("Trying to destroy MissingEle");
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

        protected virtual void SetViewElementAnswer()
        {
            if(unknownElement != null)
            {
                TextTaskViewElement element = ((TextTaskViewElement)unknownElement);

                element.SetState(TaskElementState.Correct);
                element.Value = this.variants[CorrectVariantIndexes[0]].Value;
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
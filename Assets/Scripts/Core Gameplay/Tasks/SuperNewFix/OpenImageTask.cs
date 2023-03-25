using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    //Need to change the name because Open Image doesn't look right for the task we actually have
    public abstract class OpenImageTask : ArithmeticTask
    {
        public virtual int LivesAmount { get; protected set; }

        protected List<int> availableIndexes = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, };

        public override async UniTask CreateTaskView(Transform gameplayPanel)
        {
            using (AddressableResourceLoader<GameObject> loader = new AddressableResourceLoader<GameObject>())
            {
                var gameObject = await loader.LoadAndInstantiateSingle("ChallengeView", gameplayPanel);
                this.TaskBehaviour = gameObject.GetComponent<ChallengeTaskBehaviour>();
            }
            await this.TaskBehaviour.Initialize(this);

            await UniTask.WhenAll(InitializeElementsView(), InitializeVariantsView());
            ((ChallengeTaskBehaviour)TaskBehaviour).SetLives(this.LivesAmount);
        }

        protected override async UniTask InitializeElementsView()
        {
            for (int i = 0; i < ElementsAmount; i++)
            {
                await this.Elements[i].CreateView(((ChallengeTaskBehaviour)TaskBehaviour).TaskPanel, this.TaskType)
                    .ContinueWith(async () =>
                    {
                        if (i < operators.Count && operators.Count != 0)
                        {
                            await this.operators[i].CreateView(((ChallengeTaskBehaviour)TaskBehaviour).TaskPanel, this.TaskType);
                        }
                    });
            }
        }

        protected override async UniTask InitializeVariantsView()
        {
            foreach (Variant variant in this.variants)
            {
                await variant.CreateView(((ChallengeTaskBehaviour)TaskBehaviour).VariantsPanel, this.TaskType);
            }

            for (int i = 0; i < VariantsAmount; i++)
            {
                if (this.availableIndexes.IndexOf(i) == -1)
                {
                    //!!!!!!!!!!!!! rework it a bit later!!!
                    //((VariantView)variants[i].ElementView).SetActiveVisual(false);
                }
            }
        }

        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            string expression = GetExpression;

            int answer = MathOperations.EvaluateInt(expression);
            int answerIndex = availableIndexes[Random.Range(0, availableIndexes.Count-1)];
            CorrectVariantIndexes.Add(answerIndex);

            int index = 0;
            while (index < this.TaskSettings.BaseStats.VariantsAmount)
            {
                if(this.availableIndexes.IndexOf(index) != -1)
                {
                    int value = Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber);
                    if (index == answerIndex)
                    {
                        this.variants.Add(new Variant(answer, true));
                        index++;
                    }
                    else
                    {
                        if (value != answer)
                        {
                            this.variants.Add(new Variant(value, false));
                            index++;
                        }
                    }
                }
                else
                {
                    Variant variant = new Variant(-1, false);
                    this.variants.Add(variant);
                    index++;
                }
            }

            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override async void VariantOnPressedEvent(object sender, EventArgs e)
        {
            Variant variant = (Variant)sender;

            SelectedVariantIndexes.Add(variants.FindIndex(a => a == variant));

            //!!!! rework!
            //((VariantView)variant.ElementView).SetActiveVisual(false);
            if (availableIndexes.Count == 1)
            {
                TaskManager.Instance.ShowResult(true);
                SaveResult();
            }
            else
            {
                this.availableIndexes.Remove(this.variants.FindIndex(a => a == variant));
                if (!variant.IsVariantCorrect)
                {
                    this.LivesAmount -= 1;
                    ((ChallengeTaskBehaviour)TaskBehaviour).SetDamage(1);
                    if (LivesAmount == 0)
                    {
                        TaskManager.Instance.ShowResult(true);
                        SaveResult();
                    }
                }
                await RegenerateValues();
            }


        }

        protected async UniTask RegenerateValues()
        {
            await ((ChallengeTaskBehaviour)TaskBehaviour).ResetToDefault();
            Elements.Clear();
            operators.Clear();
            variants.Clear();
            await CreateTaskElementsAsync();
            await UniTask.WhenAll(InitializeElementsView(), InitializeVariantsView());
        }

        protected override void SaveResult()
        {
            //Need to trigger some code in TaskManager to save some data
        }

    }
}
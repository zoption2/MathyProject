using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class PairsTask : Task
    {
        protected List<Element> selectedElements = new List<Element>();
        protected int ActiveElementCount;

        public override async UniTask CreateTaskView(Transform gameplayPanel)
        {
            using (AddressableResourceLoader<GameObject> loader = new AddressableResourceLoader<GameObject>())
            {
                var gameObject = await loader.LoadAndInstantiateSingle("ChallengeView", gameplayPanel);
                this.TaskBehaviour = gameObject.GetComponent<ChallengeTaskBehaviour>();
            }
            await this.TaskBehaviour.Initialize(this);

            await UniTask.WhenAll(InitializeElementsView());
        }

        public override async UniTask DisposeTaskView()
        {
            //Looks like old trash

            try
            {
                System.Threading.Tasks.Task timer = System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2));
                var completedTask = await System.Threading.Tasks.Task.WhenAny(timer);
                if (completedTask.IsCompleted)
                    await this.TaskBehaviour.DisposeAsync();
            }
            catch (Exception e)
            {
                Debug.Log("Error occurred" + e);
            }
        }


        protected override async System.Threading.Tasks.Task CreateTaskElementsAsync()
        {
            await CreateElements();
        }

        protected virtual async System.Threading.Tasks.Task CreateElements()
        {
            List<Element> tempList = new List<Element>();
            for (int i = 0; i < (TaskSettings.ElementsAmount / 2); i++)
            {
                tempList.Add(new ButtonTaskElement(this.TaskSettings.MaxNumber - i));
                tempList.Add(new ButtonTaskElement(this.TaskSettings.MaxNumber - i));
            }

            System.Random random = new System.Random();
            this.Elements = tempList.OrderBy(item => random.Next()).ToList();
            ActiveElementCount = Elements.Count;
            await HideAllElements();
        }

        protected virtual async UniTask InitializeElementsView()
        {
            for (int i = 0; i < ElementsAmount; i++)
            {
                await this.Elements[i].CreateView(((ChallengeTaskBehaviour)TaskBehaviour).VariantsPanel, this.TaskType);
            }

            foreach (Element element in this.Elements)
            {
                ((ButtonTaskElement)element).OnPressedEvent += ElementOnPressedEvent;
            }
        }


        protected virtual void ElementOnPressedEvent(object sender, EventArgs e)
        {
            ButtonTaskElement selectedElement = (ButtonTaskElement)sender;

            ((ButtonTaskElementView)selectedElement.ElementView).SelectTween(true);
            if (selectedElements.Count == 1)
            {
                selectedElements.Add(selectedElement);

                if ((int)selectedElements[0].Value == (int)selectedElements[1].Value)
                {
                    ((ButtonTaskElementView)selectedElements[0].ElementView).SetActiveVisual(false);
                    ((ButtonTaskElementView)selectedElements[1].ElementView).SetActiveVisual(false);
                    ActiveElementCount -= 2;
                }
                else
                {
                    //WrongVariant(selectedElements);
                }

                selectedElements = new List<Element>();
            }
            else
            {
                selectedElements.Add(selectedElement);
            }

            if (ActiveElementCount == 0)
            {
                TaskManager.Instance.ShowResult(true);
            }

        }

        protected override void SaveResult()
        {
            //Saving some data
        }


        protected virtual async UniTask HideAllElements()
        {
            foreach (Element element in Elements)
            {
                ((ButtonTaskElementView)selectedElements[0].ElementView).SelectTween(true);
                ((ButtonTaskElementView)selectedElements[0].ElementView).SetInteractable(false);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(2));

            foreach (Element element in Elements)
            {
                ((ButtonTaskElementView)selectedElements[0].ElementView).SelectTween(false);
                ((ButtonTaskElementView)selectedElements[0].ElementView).SetInteractable(true);
            }
        }
    }
}

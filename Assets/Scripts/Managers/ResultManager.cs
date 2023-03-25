using Mathy.Core.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mathy.UI.Tasks;
using Cysharp.Threading.Tasks;
using System;

namespace Mathy.UI
{
    public class ResultManager : StaticInstance<ResultManager>
    {
        [Header("Components:")]
        [SerializeField] private Image bgImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private TextMeshProUGUI headerTitle;
        [SerializeField] private TaskCounterPanel taskCounterPanel;
        [SerializeField] private Transform taskPanel;

        [Header("GFX References:")]
        [SerializeField] private List<Sprite> bgImages;
        [SerializeField] private List<Color> headerColors;

        private List<Task> taskList;
        private List<KeyValuePair<int, bool>> userAnswers;
        private List<int> correctAnswers;
        private Task? activeTask;

        private bool isIndicatorButtonPressed = false;

        public void Initialize(List<Task> taskList, List<bool> answers, List<int> userAnswersId)
        {
            this.taskList = taskList;
            this.userAnswers = new List<KeyValuePair<int, bool>>();

            for (int i = 0; i < answers.Count; i++)
            {
                this.userAnswers.Add(new KeyValuePair<int, bool>(userAnswersId[i], answers[i]));
            }

            taskCounterPanel.UpdateIndicatorsAmount(taskList.Count);
            taskCounterPanel.ResetToDefault(false);

            for (int i = 0; i < taskCounterPanel.TaskIndicators.Count; i++)
            {
                TaskIndicatorButton button = (TaskIndicatorButton)taskCounterPanel.TaskIndicators[i];
                if (answers[i])
                {
                    button.Status = TaskStatus.Right;
                }
                else
                {
                    button.Status = TaskStatus.Wrong;
                }
                button.Id = i;
                button.OnPressedEvent += OnIndicatorButtonPressed;
                //button.indicatorButton.onClick.AddListener(OnIndicatorButtonPressed);
            }

            _ = PreloadAllTaskViews();
        }

        private async void OnIndicatorButtonPressed(object sender, EventArgs e)
        {
            if (!isIndicatorButtonPressed)
            {
                TaskIndicatorButton button = (TaskIndicatorButton)sender;
                isIndicatorButtonPressed = true;
                await this.RunTask(button.Id);
            }
        }

        public async UniTask PreloadAllTaskViews()
        {
            await ClearAllTaskViews();

            foreach (Task task in taskList)
            {
                _ = task.CreateTaskView(taskPanel);
            }
        }

        public async UniTask ClearAllTaskViews()
        {
            taskPanel.DestroyChildren();
            await UniTask.WaitWhile(() => taskPanel.childCount > 0);
            activeTask = null;
        }


        public async System.Threading.Tasks.Task RunTask(int taskIndex)
        {
            if (activeTask != null)
            {
                Task previousTask = activeTask;

                activeTask = taskList[taskIndex];

                await activeTask.CreateTaskView(taskPanel);
                activeTask.TaskBehaviour.SetActiveViewPanels(false);

                //await previousTask.DisposeTaskView();
                previousTask.TaskBehaviour.SetActiveViewPanels(false);
            }
            else
            {
                activeTask = taskList[taskIndex];
                //await activeTask.CreateTaskView(taskPanel);
            }

            taskCounterPanel.UpdateCounterText(taskIndex, taskList.Count);
            activeTask.TaskBehaviour.SetActiveViewPanels(true);
            headerTitle.text = activeTask.TaskSettings.Title;

            isIndicatorButtonPressed = false;

            //Set variant Elements state

            switch (activeTask.TaskType)
            {
                case TaskType t when (activeTask.TaskType == TaskType.Addition || activeTask.TaskType == TaskType.ComplexAddSub
                || activeTask.TaskType == TaskType.Multiplication || activeTask.TaskType == TaskType.Division || activeTask.TaskType == TaskType.RandomArithmetic
                || activeTask.TaskType == TaskType.AddSubMissingNumber || activeTask.TaskType == TaskType.IsThatTrue
                || activeTask.TaskType == TaskType.MissingSign):
                    {
                        for (int i = 0; i < ((ArithmeticTask)activeTask).VariantsAmount; i++)
                        {
                            VariantView view = (VariantView)(((ArithmeticTask)activeTask).variants[i].ElementView);
                            view.SetInteractable(false);

                            if (i == userAnswers[taskIndex].Key && userAnswers[taskIndex].Value)
                            {
                                //view.SetAsCorrect();
                                view.State = TaskElementState.Correct;
                            }
                            else if (i == userAnswers[taskIndex].Key && !userAnswers[taskIndex].Value)
                            {
                                //view.SetAsWrong();
                                view.State = TaskElementState.Wrong;
                            }
                    }
                        break;
                    }
                case TaskType t when (activeTask.TaskType == TaskType.Comparison || activeTask.TaskType == TaskType.ComparisonWithMissingNumber):
                    {
                        for (int i = 0; i < ((ComparisonTask)activeTask).VariantsAmount; i++)
                        {
                            VariantView view = (VariantView)(((ComparisonTask)activeTask).variants[i].ElementView);
                            view.SetInteractable(false);

                            if (i == userAnswers[taskIndex].Key && userAnswers[taskIndex].Value)
                            {
                                //view.SetAsCorrect();
                                view.State = TaskElementState.Correct;
                            }
                            else if (i == userAnswers[taskIndex].Key && !userAnswers[taskIndex].Value)
                            {
                                //view.SetAsWrong();
                                view.State = TaskElementState.Wrong;
                            }
                        }
                        break;
                    }
                case TaskType.MissingNumber:
                    {
                        for (int i = 0; i < ((SequenceTask)activeTask).VariantsAmount; i++)
                        {
                            VariantView view = (VariantView)(((SequenceTask)activeTask).variants[i].ElementView);
                            view.SetInteractable(false);

                            if (i == userAnswers[taskIndex].Key && userAnswers[taskIndex].Value)
                            {
                                view.SetAsCorrect();
                            }
                            else if (i == userAnswers[taskIndex].Key && !userAnswers[taskIndex].Value)
                            {
                                view.SetAsWrong();
                            }
                        }
                        break;
                    }
            }

        }
    }
}
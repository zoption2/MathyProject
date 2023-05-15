using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mathy.Data;
using System;
using Mathy.Core.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Cysharp.Threading.Tasks;
using System.Linq;
using Mathy.Core;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Mathy.Services;

namespace Mathy.UI
{
    public class DailyResultPanel : PopupPanel
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private Image modeIcon;
        [SerializeField] private Image gradeIcon;
        [SerializeField] private TMP_Text completedTasksAmountText;
        [SerializeField] private TMP_Text time;
        [SerializeField] private List<ResultListElement> resultListElements;

        [Header("REFERENCES:")]
        [SerializeField] private List<Sprite> dailyModeIcons;
        [SerializeField] private List<Sprite> gradeIcons;

        private TaskMode selectedTaskMode;
        public TaskMode SelectedTaskMode
        {
            get => selectedTaskMode;
            set
            {
                selectedTaskMode = value;
                modeIcon.sprite = dailyModeIcons[(int)selectedTaskMode];
                switch (value)
                {
                    case TaskMode.Small:
                        taskAmount = 10;
                        break;
                    case TaskMode.Medium:
                        taskAmount = 20;
                        break;
                    case TaskMode.Large:
                        taskAmount = 30;
                        break;
                }
            }
        }

        private int correctAnswersCount = 0;
        private int taskAmount = 0;
        private bool isTaskRegenerated;

        #endregion
        [Inject] private IDataService dataService;

        public async void Initialize(TaskMode taskMode, bool isTodayTasks)
        {
            SelectedTaskMode = taskMode;
            await RegenerateTaskList(isTodayTasks);
            //UpdateInfoPanel();
        }

        public override void OpenPanel()
        {
            _ = AsyncOpen();
        }

        private async UniTask AsyncOpen()
        {
            background.alpha = 0;
            popupPanel.localScale = Vector3.zero;

            await UniTask.WaitUntil(() => isTaskRegenerated);

            background.DOFade(1, tweenDuration);
            popupPanel.DOScale(Vector3.one, tweenDuration).OnComplete(() => OnComplete(true));
        }

        private async System.Threading.Tasks.Task RegenerateTaskList(bool isTodayTasks)
        {
            isTaskRegenerated = false;
            correctAnswersCount = 0;

            DateTime date = isTodayTasks ? DateTime.UtcNow : CalendarManager.Instance.SelectedDate;

            var modeData = await dataService.TaskData.GetDailyModeData(date, SelectedTaskMode);

            if (modeData.IsComplete) //await DataManager.Instance.IsDateModeCompleted(SelectedTaskMode, date))//await DatabaseHandler.IsDateModeCompleted(SelectedTaskMode, date))
            {
                //Debug.LogError("DailyResultPanel request");
                var tasksResults = await dataService.TaskData.GetResultsByModeAndDate(SelectedTaskMode, date);
                List<string> results = await dataService.TaskData.GetTaskResultsFormatted(SelectedTaskMode, date);
                //List<bool> answers = await DataManager.Instance.GetAnswers(SelectedTaskMode, date);
                List<bool> answers = tasksResults.Select(x => x.IsAnswerCorrect).ToList();
                //foreach (var result in results)
                //{
                //    Debug.Log(result);
                //}

                taskAmount = answers.Count();
                //List<TimeSpan> timeSpanes = await DataManager.Instance.GetTimeSpansByModeAndDate(SelectedTaskMode, date);
                List<TimeSpan> timeSpanes = tasksResults.Select(x => TimeSpan.FromMilliseconds(x.Duration)).ToList();
                //Создаем кнокпи снизу в результ панели и вешаем на них переходы на запуск нужных тасок
                for (int i = 0; i < results.Count; i++)
                {
                    bool isAnswerCorrect;

                    if (answers[i])
                    {
                        isAnswerCorrect = true;
                        correctAnswersCount++;
                    }
                    else
                    {
                        isAnswerCorrect = false;
                    }

					var element = resultListElements[i];
					
                    string taskTime = $"{Math.Round(timeSpanes[i].TotalSeconds, 2)} " +
                        $"{LocalizationManager.GetLocalizedString("GUI Elements", "DailyResult_Sec")}";
                    string result = results[i];
                    element.Initialize(i + 1, result, taskTime, isAnswerCorrect);
				}

                for (int i = 0; i < resultListElements.Count; i++)
                {
                    resultListElements[i].gameObject.SetActive(i < taskAmount);
                }
                resultListElements[0].transform.parent.localPosition = Vector2.zero;
            }
            else
            {
                Debug.LogError("No data");
            }
            isTaskRegenerated = true;
        }

        public void UpdateInfoPanel(int correctRate, double modeTime, string completedTasks)
        {
            gradeIcon.sprite = gradeIcons[CalculateGrade(correctRate)];
            var timeSpan = TimeSpan.FromMilliseconds(modeTime);
            time.text = string.Format("{0:D2} : {1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            completedTasksAmountText.text = completedTasks /*correctAnswersCount + "/" + taskAmount*/;
        }

        private int CalculateGrade(int correctRate)
        {
            //int percent = CalculatePercent();
            if (correctRate <= 59)
            {
                return 0; //Grade F
            }
            else if (correctRate > 59 && correctRate <= 69)
            {
                return 1; //Grade D
            }
            else if (correctRate > 69 && correctRate <= 79)
            {
                return 2; //Grade C
            }
            else if (correctRate > 79 && correctRate <= 89)
            {
                return 3; //Grade B
            }
            else if (correctRate > 89)
            {
                return 4; //Grade A
            }

            return 0;
        }

    }
}
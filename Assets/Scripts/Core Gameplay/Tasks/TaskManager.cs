using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Mathy.Data;
using Mathy.UI;
using Mathy.UI.Tasks;
using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks.DailyTasks;
using Cysharp.Threading.Tasks.Linq;

namespace Mathy.Core.Tasks
{
    /// <summary>
    /// Generating the Tasks list, managing task data for Save Manager
    /// </summary>
    public class TaskManager : StaticInstance<TaskManager>, ISaveable
    {
        #region FIELDS

        [Header("Components:")]
        [SerializeField] private Image bgImage;
        [SerializeField] private Image headerImage;
        [SerializeField] private TextMeshProUGUI headerTitle;
        [SerializeField] private ResultWindow resultWindow;
        [SerializeField] private TimerDifficultyMenu timerDifficultyMenu;
        [SerializeField] public Transform GameplayPanel;

        [Header("GFX References:")]
        [SerializeField] private List<Sprite> bgImages;
        [SerializeField] private List<Color> headerColors;

        private List<Task> taskList = new List<Task>();
        //List for preload some tasks 
        private List<Task> taskListSmall = new List<Task>();
        private List<Task> taskListMedium = new List<Task>();
        private List<Task> taskListLarge = new List<Task>();

        public event EventHandler<EventArgs> OnSaveEvent;
        public int TasksAmount { get => taskList.Count; }
        public int CorrectAnswers { get; set; } = 0;

        private bool isPractice;
        public bool IsPractice
        {
            get => isPractice;
            set
            {
                isPractice = value;
                currenTaskData.Mode = TaskMode.Practic;
            }
        }
        private int currentDiff;
        private int currentDifficulty { get => currentDiff; set => currentDiff = value;}

        private string currentTaskType;
        private string BestScoreKey
        {
            get
            {
                return currentTaskType + "BestScore" + currentDifficulty;
            }
        }
        private int bestScore
        {
            get => PlayerPrefs.GetInt(BestScoreKey, 0);
            set
            {
                PlayerPrefs.SetInt(BestScoreKey, value);
            }
        }


        private ChallengeData currenChallengeData { get; set; } = new ChallengeData();
        private TaskData currenTaskData { get; set; } = new TaskData();
        public bool IsTasksGenerated { get; private set; } = false;

        private int currentTaskIndex = 0;
        private float timerValue;
        private Task activeTask;
        private ProgressBar activeTimer;

        #endregion
        [SerializeField] private Transform taskParent;

        private void OnEnable()
        {
            UpdateSceneVisual();
        }

        private string GetLocalizedTaskTitle()
        {
            var title = LocalizationManager.GetLocalizedString("Game Names", activeTask.TaskSettings.Title);
            return title;
        }

        private void UpdateBestScore(int score)
        {
            if (score > bestScore)
            {
                bestScore = score;
                timerDifficultyMenu.bestScoreTextLabels[currentDifficulty].text = score.ToString();
            }
        }

        public void UpdateAllBestScoreText()
        {
            int selectedDifficulty = currentDifficulty;
            for (int i = 0; i < 4; i++)
            {
                currentDifficulty = i;
                timerDifficultyMenu.bestScoreTextLabels[currentDifficulty].text = bestScore.ToString();
            }
            currentDifficulty = selectedDifficulty;
        }


        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            DataManager.Instance.Subscribe(this);
            IsPractice = false;
        }

        public async UniTask ClearAllTaskViews()
        {
            GameplayPanel.DestroyChildren();
            await UniTask.WaitWhile(() => GameplayPanel.childCount > 0);
        }

        public void SaveTaskData(TaskType taskType, bool isCorrect, List<int> selectedVariantIndexes, List<int> corectVariantIndexes, List<Element> elements, List<Operator> operators, List<Variant> variants)
        {
            currenTaskData.TaskModeIndex = currentTaskIndex;
            currenTaskData.SelectedAnswerIndexes = selectedVariantIndexes;
            currenTaskData.CorrectAnswerIndexes = corectVariantIndexes;
            currenTaskData.IsAnswerCorrect = isCorrect;
            currenTaskData.Seed = activeTask.Seed;
            currenTaskData.TaskType = taskType;
            currenTaskData.ElementValues = elements.ConvertAll<string>(x => x.Value.ToString());
            currenTaskData.VariantValues = variants.ConvertAll<string>(x => x.Value.ToString());

            if(operators != null)
            {
                currenTaskData.OperatorValues = operators.ConvertAll<string>(x => x.Value.ToString());
            }

            bool isDone = TasksAmount == currentTaskIndex && currenTaskData.Mode != TaskMode.Practic;
            currenTaskData.IsModeDone = isDone;
            DataManager.Instance.Save();
        }

        //ISaveable method
        public void Save()
        {
            SaveEventArgs args = new SaveEventArgs();
            args.TaskData = currenTaskData;

            OnSaveEvent.Invoke(this, args);
        }

        public async void CreateTaskList(TaskMode mode)
        {
            currenTaskData = new TaskData();
            currenTaskData.Mode = mode;

            bool isTodayDateExists = await DataManager.Instance.IsTodayModeExist(mode);

            if (isTodayDateExists)
            {
                int taskIndex = await DataManager.Instance.GetLastTaskIndexOfMode(mode);
                currentTaskIndex = taskIndex + 1;
            }
            else
            {
                currentTaskIndex = 0;
            }

            await GenerateAllTasks();

            switch (mode)
            {
                case TaskMode.Small:
                    taskList = taskListSmall;
                    break;
                case TaskMode.Medium:
                    taskList = taskListMedium;
                    break;
                case TaskMode.Large:
                    taskList = taskListLarge;
                    break;
                default:
                    goto case TaskMode.Small;
            }
            //await UniTask.WaitUntil(() => taskList.Count > 0);
            await PreloadAllTaskViewsOfMode(mode);
        }

        public async UniTask GenerateAllTasks()
        {
            await DestroyAllTasks();
            
            if (await DataManager.Instance.IsTodayModeCompleted(TaskMode.Small))
            {
                taskListSmall = null;
            }
            else
            {
                taskListSmall = await GenerateAllTasksOfMode(TaskMode.Small);
            }

            if (await DataManager.Instance.IsTodayModeCompleted(TaskMode.Medium))
            {
                taskListMedium = null;
            }
            else
            {
                taskListMedium = await GenerateAllTasksOfMode(TaskMode.Medium);
            }

            if (await DataManager.Instance.IsTodayModeCompleted(TaskMode.Large))
            {
                taskListLarge = null;
            }
            else
            {
                taskListLarge = await GenerateAllTasksOfMode(TaskMode.Large);
            }
        }

        private async UniTask DestroyAllTasks()
        {
            if (taskListSmall != null)
            {
                foreach (Task t in taskListSmall)
                {
                    await t.DisposeAsync();
                }
                taskListSmall = null;
            }
            else if (taskListMedium != null)
            {
                foreach (Task t in taskListMedium)
                {
                    await t.DisposeAsync();
                }
                taskListMedium = null;
            }
            else if (taskListLarge != null)
            {
                foreach (Task t in taskListLarge)
                {
                    await t.DisposeAsync();
                }
                taskListLarge = null;
            }
        }

        private async System.Threading.Tasks.Task<List<Task>> GenerateAllTasksOfMode(TaskMode mode)
        {
            return await CreateTaskList(GetTasksAmountOfMode(mode), mode);
        }

        private async System.Threading.Tasks.Task<List<Task>> CreateTaskList(int amount, TaskMode mode)
        {
            bool isTodayModeCompleted = await DataManager.Instance.IsDateModeCompleted(mode, DateTime.Now);

            List<Task> tasks = new List<Task>();

            if (!isTodayModeCompleted)
            {
                bool isTodayDateExists = await DataManager.Instance.IsTodayModeExist(mode);

                if (isTodayDateExists)
                {
                    bool isCompleted = await DataManager.Instance.IsTodayModeCompleted(mode);
                    if (!isCompleted || currenTaskData.Mode == TaskMode.Practic)
                    {
                        int lastTaskIndex = await DataManager.Instance.GetLastTaskIndexOfMode(mode);
                        int additionalTasksCount = GetTasksAmountOfMode(mode) - (lastTaskIndex + 1);

                        for(int i = 0; i <= lastTaskIndex; i++)
                        {
                            //временная заглушка
                            tasks.Add(new Addition( 1, new ScriptableTask()) );
                        }

                        using (TaskGenerator taskGenerator = new TaskGenerator())
                        {
                            tasks.AddRange(await taskGenerator.GenerateByAvailableSkills(additionalTasksCount));
                        }

                        //+1 coz we store only last task done index
                        currentTaskIndex = lastTaskIndex + 1;
                    }
                }
                else
                {
                    using (TaskGenerator taskGenerator = new TaskGenerator())
                    {
                        //tasks = await taskGenerator.Generate(amount);
                        tasks = await taskGenerator.GenerateByAvailableSkills(amount);
                    }
                    currentTaskIndex = 0;
                }
            }

            //Debug.Log($"{mode}");
            //Debug.Log($"Tasks count = {tasks.Count}");
            //for (int i = 0; i < tasks.Count; i++)
            //{
            //    Debug.Log($"{i}) {tasks[i].TaskType}");
            //}
            //Debug.Log("________________________________________________");

            return tasks;
        }

        public async UniTask PreloadAllTaskViewsOfMode(TaskMode mode)
        {
            await ClearAllTaskViews();

            for (int i = 0; i < taskList.Count; i++)
            {
                Task task = taskList[i];
                await task.CreateTaskView(GameplayPanel);
            }

            for (int i = 0; i < taskList.Count; i++)
            {
                DefaultTaskBehaviour taskBehaviour = (DefaultTaskBehaviour)taskList[i].TaskBehaviour;
                taskBehaviour.SetActiveViewPanels(false);
                taskBehaviour.SetActiveProgressBar(false);
                taskBehaviour.gameObject.name = taskBehaviour.Task.TaskType.ToString();
            }

            if(await DataManager.Instance.IsTodayModeExist(mode))
            {
                currentTaskIndex = await DataManager.Instance.GetLastTaskIndexOfMode(mode) + 1;
            }

            await ActivateTaskAsync(currentTaskIndex);

            await UniTask.Delay(1000);
            LoadingManager.Instance.ClosePanel();
        }

        private int GetTasksAmountOfMode(TaskMode mode)
        {
            int amount;
            switch (mode)
            {
                case TaskMode.Small:
                    amount = 10;
                    break;
                case TaskMode.Medium:
                    amount = 20;
                    break;
                case TaskMode.Large:
                    amount = 30;
                    break;
                default:
                    goto case TaskMode.Small;
            }
            return amount;
        }

        public async void StartTaskPractice(ScriptableTask taskParam, int amount)
        {  
            currentTaskType = taskParam.TaskType.ToString() + taskParam.MaxNumber.ToString();

            using (TaskGenerator taskGenerator = new TaskGenerator())
            {
                this.taskList = await taskGenerator.GenerateBySetting(taskParam, amount);
            }
            currentTaskIndex = 0;

            await ActivateTaskAsync(currentTaskIndex);

        }

        public async void StartTaskPractice(List<ScriptableTask> taskParams, int amount)
        {
            taskList.Clear();
            for (int i = 0; i < amount;i++)
            {
                ScriptableTask tast = taskParams[UnityEngine.Random.Range(0, taskParams.Count)];

                currentTaskType = tast.TaskType.ToString() + tast.MaxNumber.ToString();

                using (TaskGenerator taskGenerator = new TaskGenerator())
                {
                    this.taskList.Add(await taskGenerator.GenerateSingleBySetting(tast));
                }
            }
            currentTaskIndex = 0;
            await ActivateTaskAsync(currentTaskIndex);
        }

        public async void RunNextTask()
        {
            currentTaskIndex++;

            if (activeTimer != null)
            {
                activeTimer.StopTimer();
            }
            if (currentTaskIndex < TasksAmount)
            {
                await ActivateTaskAsync(currentTaskIndex);
            }
            else if (IsPractice)
            {
                // Recreating a new task list after playing the last task
                StartTaskPractice(activeTask.TaskSettings, TasksAmount);
            }
            else
            {
                ShowResult(true);
            }
        }

        public void CorrectAnswer()
        {
            CorrectAnswers++;
            currenTaskData.EndTime = DateTime.UtcNow;
            VibrationManager.Instance.TapPeekVibrate();

            RunNextTask();
        }

        public void WrongAnswer()
        {
            currenTaskData.EndTime = DateTime.UtcNow;
            VibrationManager.Instance.TapNopeVibrate();

            if (isPractice)
            {
                ShowResult(true);
            }
            else
            {
                RunNextTask();
            }
        }

        public async System.Threading.Tasks.Task ActivateTaskAsync(int taskIndex)
        {
            //Later a bit refactor
            if (activeTask != null)
            {
                Task previousTask = activeTask;
                activeTask = taskList[taskIndex];

                await activeTask.CreateTaskView(GameplayPanel);
                //if (activeTask.TaskBehaviour == null)
                //    await activeTask.CreateTaskView(GameplayPanel);
                activeTask.TaskBehaviour.SetActiveViewPanels(false);

                await previousTask.DisposeTaskView();
                //previousTask.TaskBehaviour.SetActiveViewPanels(false);


                currenTaskData.StartTime = DateTime.UtcNow;
            }
            else
            {
                activeTask = taskList[taskIndex];
                if (activeTask.TaskBehaviour == null)
                    await activeTask.CreateTaskView(GameplayPanel);
                activeTask.TaskBehaviour.SetActiveViewPanels(true);

                currenTaskData.StartTime = DateTime.UtcNow;
            }
            if (isPractice)
            {
                if (currentDifficulty == 0)
                {
                    SetActiveProgressBar(false);
                }
                else
                {
                    SetActiveProgressBar(true);
                }
                StartTimer();
            }
            else
            {
                SetActiveProgressBar(false);
            }

            activeTask.TaskBehaviour.SetActiveViewPanels(true);

            headerTitle.text = GetLocalizedTaskTitle();
            currentTaskIndex = taskIndex;
        }

        private void UpdateSceneVisual()
        {
            int index = UnityEngine.Random.Range(0, bgImages.Count);
            headerImage.color = headerColors[index];
            bgImage.sprite = bgImages[index];
        }

        public void ActivatePracticeMode(bool isChallenge)
        {
            IsPractice = true;
            if (isChallenge)
            {
                EnableDifficultyMenu(false);
            }
            else
            {
                EnableDifficultyMenu(true);
            }

        }

        public async void ShowResult(bool isActive)
        {
            if (activeTimer != null)
            {
                activeTimer.StopTimer();
            }

            int index = currentTaskIndex > 0 ? currentTaskIndex - 1 : currentTaskIndex;
            bool isChallenge = false;
            resultWindow.gameObject.SetActive(isActive);

            float correctRate = CorrectAnswers / (float)TasksAmount * 100f;

            if (isActive) 
            { 
                resultWindow.DisplayResult(CorrectAnswers, TasksAmount, correctRate, isChallenge); 
            }

            if (IsPractice)
            {
                UpdateBestScore(CorrectAnswers);
            }
        }

        public void SetActiveProgressBar(bool isActive)
        {
            switch (activeTask.TaskType)
            {
                case TaskType t when (activeTask.TaskType == TaskType.Addition || activeTask.TaskType == TaskType.Subtraction || activeTask.TaskType == TaskType.Multiplication
                || activeTask.TaskType == TaskType.Division || activeTask.TaskType == TaskType.Comparison || activeTask.TaskType == TaskType.ComplexAddSub
                || activeTask.TaskType == TaskType.RandomArithmetic || activeTask.TaskType == TaskType.MissingNumber || activeTask.TaskType == TaskType.ImageOpening):
                    {
                        ((DefaultTaskBehaviour)this.activeTask.TaskBehaviour).SetActiveProgressBar(isActive);
                        break;
                    }
            }
        }

        public void EnableDifficultyMenu(bool isActive)
        {
            timerDifficultyMenu.gameObject.SetActive(isActive);
        }

        public void ResetToDefault()
        {
            ShowResult(false);
            CorrectAnswers = 0;
            timerValue = 0;
        }

        public void RestartTasks()
        {
            if (isPractice)
            {
                ResetToDefault();
                StartTaskPractice(activeTask.TaskSettings, TasksAmount);
            }
        }

        
        private void StartTimer()
        {
            if (activeTask != null)
            {
                activeTimer = ((DefaultTaskBehaviour)activeTask.TaskBehaviour).ProgressBar;

                if (timerValue > 0)
                {
                    activeTimer.gameObject.SetActive(true);
                    activeTimer.StartTimer(timerValue);
                }
                else
                {
                    activeTimer.gameObject.SetActive(false);
                }
            }
        }

        public void StopTimer()
        {
            if (activeTimer != null)
            {
                activeTimer.StopTimer();
                timerDifficultyMenu.gameObject.SetActive(true);
            }
        }

        public void SetTimerValue(float newValue, int timerMode)
        {
            timerValue = newValue;
            currentDifficulty = timerMode;
            StartTimer();
        }
    }
}
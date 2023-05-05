using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fallencake.Tools;
using System;
using Mathy.Data;
using Mathy.UI;
using Mathy.Core;

/// <summary>
/// Generating the Tasks list, managing task data for Save Manager
/// </summary>
public class ChallengesManager : StaticInstance<ChallengesManager>//, ISaveable
{
    #region Fields

    [Header("Task config:")]
    [SerializeField] private List<ScriptableTask> taskList;

    [Header("Components:")]
    [SerializeField] private Image bgImage;
    [SerializeField] private Image headerImage;
    [SerializeField] private TextMeshProUGUI headerTitle;
    [SerializeField] private ResultWindow resultWindow;

    [Header("GFX References:")]
    [SerializeField] private List<Sprite> bgImages;
    [SerializeField] private List<Color> headerColors;

    public int TasksAmount { get => taskList.Count; }
    private bool isPractice;
    public bool IsPractice 
    { 
        get => isPractice;
        set 
        {
            isPractice = value;
        }
    }

    public System.DateTime dateTime { get; private set; } = System.DateTime.Now;
    public string today { get; private set; }

    private ChallengeData currenData { get; set; } = new ChallengeData();

    private int currentTaskIndex = 0;
    private int correctAnswers = 0; //Not supposed to be here, need to be in its own class I think
    private TaskOLD activeTask;

    public event EventHandler<EventArgs> OnSaveEvent;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        //Load();
        IsPractice = false;
        //DataManager.Instance.Subscribe(this);
    }

    private void OnEnable()
    {
        taskList = new List<ScriptableTask>();
    }

    public void SaveTaskData(ChallengeData data)
    {
        //var currentTask = taskList[currentTaskIndex];

        //currenData.Seed = 0;
        //currenData.TaskType = activeTask.TaskType;


        //DataManager.Instance.TodayChallengeStatus = true;
        //PlayerDataManager.Instance.ChallengesDoneAmount++;
        //if (!isPractice)
        //{
        //    PlayerDataManager.Instance.ChallengesDoneAmount++;
        //    DataManager.Instance.SaveChallenge(data);
        //}
        

        //currenData.CurrentTaskIndex = currentTaskIndex;
        
        //currenData.SelectedAnswers.Add(currentTaskIndex, activeTask.selectedVariantIndex);

        /*
        if (taskList.Count == currentTaskIndex + 1)
        {
            //currenData.IsModeDone = true;
            //currenData.CurrentTaskIndex = null;

            //DataManager.Instance.Save();
            //Here supposed to be an immediate Save withou any timeouts, 
            //that need to cancell all debounced saves if they exists
            //DataManager.Instance.ImmediateSave();
        }
        else
        {
            //Saving data, later will be with some timeout
            //DataManager.Instance.Save();
        }
        */
    }

    public void CreateTaskList(ScriptableTask task, int tasksAmount)
    {
        //currenData = new ChallengeData();
        //currenData.Mode = TaskMode.Challenge;

        if (tasksAmount > 0)
        {
            for (int i = 0; i < tasksAmount; i++)
            {
                taskList.Add(task);
            }
            taskList.FCShuffle();
            //ActivateTask(0);
        }
        else
        {
            Debug.LogError("Argument exeption"); //Rewrite a bit later
        }
        currentTaskIndex = IsPractice ? 0 : currentTaskIndex; //Need reset it to 0 to Activate Challenges practice
        ActivateTask(currentTaskIndex); // Bug with Challenges
                                       // Challenges are not activating if we try to play practice mode after daily tasks mode
                                       // Need to reset currentTaskIndex to 0 in thise case
    }

    public void RunNextTask()
    {
        currentTaskIndex++;
        if(currentTaskIndex < TasksAmount)
        {
            ActivateTask(currentTaskIndex);
        }
        else if (IsPractice)
        {
            currentTaskIndex = 0;
            ActivateTask(currentTaskIndex);
        }
        else
        {
            ShowResult(true);
        }
    }

    public void CorrectAnswer()
    {
        correctAnswers++;
        VibrationManager.Instance.TapPeekVibrate();
        RunNextTask();
    }

    public void WrongAnswer()
    {
        VibrationManager.Instance.TapNopeVibrate();
        RunNextTask();
    }

    public void ActivateTask(int taskIndex)
    {
        if (activeTask != null)
        {
            activeTask.gameObject.SetActive(false);
        }
        currentTaskIndex = taskIndex;
        var task = taskList[currentTaskIndex];

        //Here in task logic we have switch between what kind of task it supposed to be (Basic Tasks or Challenges)
        //Its very bad solution and need to be changed
        TaskOLD taskLogic;
        taskLogic = TaskSystemOLD.Instance.GetChallenge(task.TaskType);

        taskLogic.gameObject.SetActive(true);
        activeTask = taskLogic;

        UpdateSceneVisual();
        headerTitle.text = LocalizationManager.GetLocalizedTaskTitle(task.Title);

        taskLogic.ElementsAmount = task.ElementsAmount;
        taskLogic.VariantsAmount = task.VariantsAmount;
        taskLogic.MaxNumber = task.MaxNumber;

        taskLogic.RunTask();
    }

    private void UpdateSceneVisual()
    {
        int index = UnityEngine.Random.Range(0, bgImages.Count);
        headerImage.color = headerColors[index];
        bgImage.sprite = bgImages[index];
    }

    public void ShowResult(bool isActive)
    {
        int index = currentTaskIndex > 0 ? currentTaskIndex - 1 : currentTaskIndex;
        bool isChallenge = TaskSystemOLD.Instance.IsChallengeOfTypeExits(taskList[index].TaskType);
        resultWindow.gameObject.SetActive(isActive);

        float correctRate = isChallenge 
            ? (activeTask as ChallengeOLD).GetCorrectRate() 
            : correctAnswers / (float)TasksAmount * 100f;

        if (isActive) resultWindow.DisplayResult(correctAnswers, TasksAmount, correctRate, isChallenge);
        
        /*
        if (!IsPractice)
        {
            if (correctRate > 0)
            {
                //SaveTaskData();
                DataManager.Instance.TodayChallengeStatus = true;
                PlayerDataManager.Instance.ChallengesDoneAmount++;
            }
        }
        */
    }

    public void ResetToDefault()
    {
        ShowResult(false);
        correctAnswers = 0;
    }

    public void RestartTasks()
    {
        currentTaskIndex = 0;
        ResetToDefault();
        taskList.FCShuffle();
        ActivateTask(currentTaskIndex);
    }
}
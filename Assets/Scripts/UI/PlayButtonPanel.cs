using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Mathy.Data;
using Mathy.Core;
using Mathy.UI;
using Zenject;
using Mathy.Core.Tasks;

public class PlayButtonPanel : StaticInstance<PlayButtonPanel>
{
    [Inject] private IGameplayService gameplayService;
    public enum PlayPanelState
    {
        Default = 0,
        Inactive = 1,
        Completed = 2
    }

    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private PlayButton playButton;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform popUpCloud;
    [SerializeField] private CanvasGroup mathyCanvasGroup;
    [SerializeField] private Image mathyImage;
    [SerializeField] private CanvasGroup joyCanvasGroup;
    [SerializeField] private RectTransform modeSelector;

    [Header("REFERENCES:")]
    [SerializeField] private List<Sprite> mathyImages;

    [Header("CONFIG:")]
    [SerializeField] private int modeButtonsInactiveDelay = 500;
    [SerializeField] private List<int> tasksAmountValues = new List<int> {10, 20, 30, 1 };
    [SerializeField] private List<float> selectorRotationValues = new List<float> { 30f, 0, -30f };

    public TaskMode SelectedTaskMode { get; private set; }
    public bool IsAllModesDone { get; private set; } = false;
    [SerializeField] private List<PlayModeIndicator> modeButtons;

    private int tasksAmount;
    private bool isInitialized = false;

    private bool isTweening = false;
    private Vector3 rotationTo = Vector3.zero;
    
    private PlayPanelState state;
    public PlayPanelState State
    {
        get => state;
        set
        {
            state = value;
            UpdatePanelState();
        }
    }

    private void UpdatePanelState()
    {
        switch (state)
        {
            case PlayPanelState.Default:
                mathyCanvasGroup.alpha = 1;
                panelCanvasGroup.alpha = 1;
                panelCanvasGroup.interactable = true;
                popUpCloud.gameObject.SetActive(false);
                mathyImage.sprite = mathyImages[0];
                break;
            case PlayPanelState.Inactive:
                mathyCanvasGroup.alpha = 1;
                panelCanvasGroup.alpha = 0;
                panelCanvasGroup.interactable = false;
                popUpCloud.gameObject.SetActive(true);
                mathyImage.sprite = mathyImages[1];
                break;
            case PlayPanelState.Completed:
                mathyCanvasGroup.DOFade(0, 0.5f);
                panelCanvasGroup.DOFade(0, 0.5f);
                panelCanvasGroup.interactable = false;
                popUpCloud.gameObject.SetActive(false);
                break;
            default:
                goto case PlayPanelState.Default;
        }
    }

    private void Start()
    {
        Initizlize();
    }

    private async void Initizlize()
    {
        for (int i = 0; i < modeButtons.Count; i++)
        {
            int modeIndex = i;
            var button = modeButtons[i];
            button.OnClick.AddListener(() => SelectMode(modeIndex));
            button.OnClick.AddListener(playButton.OnPress);
        }
        await UpdateModeButtons();
        SelectedNextMode();
        isInitialized = true;
    }

    private async UniTask UpdateModeButtons()
    {
        var calendarData = await DataManager.Instance.GetCalendarData(System.DateTime.UtcNow.Date);
        foreach (TaskMode mode in calendarData.ModeData.Keys)
        {
            if(mode != TaskMode.Practic)
                modeButtons[(int)mode].gameObject.SetActive(!calendarData.ModeData[mode]);
        }
    }

    #endregion

    private void OnEnable()
    {
        //Temp solution, it works coz OnEnabled called before Start
        if (isInitialized)
        {
            playButton.UpdateDisplayStyle(tasksAmount);
            foreach (var button in modeButtons)
            {
                button.IsInteractable = true;
            }
        }
        _ = CheckSkills();
    }

    public async UniTask CheckSkills()
    {
        await UniTask.WaitUntil(() => GradeManager.Instance.IsInitialized);
        if(IsAllModesDone)
            State = PlayPanelState.Completed;
        else if (GradeManager.Instance.IsAnySkillActivated)
            State = PlayPanelState.Default;
        else
            State = PlayPanelState.Inactive;
    }

    public void SelectMode(int mode)
    {
        if (!isTweening)
        {
            _ = SetButtonsTempInactive(modeButtonsInactiveDelay);
            SelectedTaskMode = (TaskMode)mode;
            tasksAmount = tasksAmountValues[mode];
            rotationTo = new Vector3(0, 0, selectorRotationValues[mode]);
            playButton.UpdateDisplayStyle(tasksAmount);
            UpdateSelectedModeIndicator();

            for (int i = 0; i < modeButtons.Count; i++)
            {
                modeButtons[i].Select(i == mode);
            }

            isTweening = true;
            modeSelector.DOLocalRotate(rotationTo, 0.1f, RotateMode.Fast).OnComplete(() => isTweening = false);
        }
    }

    private async UniTask SetButtonsTempInactive(int millisecondsDelay)
    {
        foreach (var button in modeButtons)
        {
            button.IsInteractable = false;
        }
        await UniTask.Delay(millisecondsDelay);
        foreach (var button in modeButtons)
        {
            button.IsInteractable = true;
        }
    }

    public void SelectedNextMode()
    {
        bool allModesDone = true;
        int modeToSelect = modeButtons.Count + 1;

        for (int i = 0; i < modeButtons.Count; i++)
        {
            var modeIndicator = modeButtons[i];
            if (modeIndicator.gameObject.activeInHierarchy && !modeIndicator.isDone)
            {
                modeToSelect = i;
                allModesDone = false;
                break;
            }
        }

        if (!allModesDone)
        {
            SelectMode(modeToSelect);
        }
        else
        {
            _ = AllModesDone();
        }
    }

    public async UniTask AllModesDone()
    {
        var calendarData = await DataManager.Instance.GetCalendarData(System.DateTime.UtcNow.Date);
        List<bool> modeStatuses = new List<bool>();
        foreach (TaskMode mode in calendarData.ModeData.Keys)
        {
            if (mode != TaskMode.Practic)
                modeStatuses.Add(calendarData.ModeData[mode]);
        }
        IsAllModesDone = modeStatuses.Count == 4 && modeStatuses.All(x => x == true);
        if (IsAllModesDone)
        {
            Debug.Log("All modes done!");
            DailyStatusPanel.Instance.AllModesDone = true;
            if(gameObject.activeInHierarchy) DailyStatusPanel.Instance.OpenPanel();
            State = PlayPanelState.Completed;
        }
    }

    private async void UpdateSelectedModeIndicator()
    {
        var modeIndicator = modeButtons[(int)SelectedTaskMode];
        modeIndicator.State = (DailyModeState)await modeStatusIndex(SelectedTaskMode);
    }

    private async System.Threading.Tasks.Task<int> modeStatusIndex(TaskMode mode)
    {
        int status;
        if (mode != TaskMode.Challenge)
        {
            //Debug.LogError("Here was TodayDoneTasksAmount");
            int doneTaskAmount = await DataManager.Instance.TodayDoneTasksAmount(mode);
            //int doneTaskAmount = 0;
            status = (doneTaskAmount == 0) ? 0 : (doneTaskAmount == tasksAmountValues[(int)mode]) ? 2 : 1;
        }
        else
        {
            status = await DataManager.Instance.TodayChallengeStatus() ? 2 : 0;
        }
        return status;
    }

    public async void UpdateModeIndicators()
    {
        for (int i = 0; i < modeButtons.Count; i++)
        {
            modeButtons[i].State = (DailyModeState)await modeStatusIndex((TaskMode)i);
        }
    }

    public void RunTask()
    {
        AudioSystem.Instance.FadeMusic(0, 1f, true);
        _ = ScenesManager.Instance.CreateTasks(tasksAmount); // generate tasks
    }

    public async void Play()
    {
        AudioSystem.Instance.FadeMusic(0, 1f, true);
        _ = ScenesManager.Instance.SetGameplaySceneActive();
        var settings = GradeManager.Instance.AvailableTaskSettings();
        gameplayService.Prepare(SelectedTaskMode, settings);
        await UniTask.Delay(1000);
        gameplayService.Start();
        LoadingManager.Instance.ClosePanel();
    }
}

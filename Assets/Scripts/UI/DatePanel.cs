using System;
using System.Collections.Generic;
using UnityEngine;
using Mathy.Data;
using DG.Tweening;
using System.Globalization;
using UnityEngine.Localization.Settings;
using TMPro;
using Cysharp.Threading.Tasks;
using Zenject;
using Mathy.Services;
using UnityEngine.UI;
using Mathy;

public class DatePanel : ButtonFX
{
    [Inject] private IDataService dataService;
    //[SerializeField] private CalendarManager calendar;
    //[SerializeField] private List<DailyModeIndicator> modeIndicators;

    [SerializeField] private GameObject smallIcon;
    [SerializeField] private GameObject mediumIcon;
    [SerializeField] private GameObject largeIcon;
    [SerializeField] private GameObject challengeIcon;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private Sprite[] rewardSprites;

    //[SerializeField] private DailyModeIndicator awardIndicator;
    [SerializeField] private TMP_Text dateLable;

    private bool isInitialization = true;
    //private bool isOpened = false;
    private CalendarData calendarData;

    public CalendarData CalendarData
    {
        get
        {
            return calendarData;
        }
        set
        {
            calendarData = value;
            //UpdateVisual();
        }
    }

    private CultureInfo cultureInfo
    {
        get
        {
            string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
            switch (localeCode)
            {
                case "en":
                    return new CultureInfo("en-US");
                case "ru":
                    return new CultureInfo("ru-RU");
                case "uk":
                    return new CultureInfo("uk-UA");
                default:
                    goto case "en";
            }
        }
    }


    //protected override void Awake()
    //{
    //    base.Awake();
    //    DailyStatusPanel.OnAllModesDone.AddListener(UpdateIndicator);
    //}

    private async void OnEnable()
    {
        if (!isInitialization)
        {
            CalendarData = await GetTodayCalendarDate();
        }
        UpdateIndicators();
        LocalizationManager.OnLanguageChanged.AddListener(Localize);
        Localize();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged.RemoveListener(Localize);
    }


    private void Start()
    {
        Initializing();
    }

    public async void ResetToDefault()
    {
        CalendarData = await GetTodayCalendarDate();
        Initializing();
    }

    private async void Initializing()
    {
        CalendarData = await GetTodayCalendarDate();
        //calendar.SelectCurrentDate();
        //UpdateIndicator(); //Temp solution, need to update award icon on start
        isInitialization = false;
    }

	private void Localize()
    {
        dateLable.text = $"{cultureInfo.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month)} {DateTime.UtcNow.Day}";
    }
	
    private void ModeDone(TaskMode mode, bool isDone)
    {
        var status = isDone ? DailyModeStatus.Done : DailyModeStatus.InProgress;
        //if(mode != TaskMode.Practic)
        //{
        //    modeIndicators[(int)mode].Status = status;
        //}
    }

    //private void UpdateVisual()
    //{
    //    foreach (TaskMode mode in calendarData.ModeData.Keys)
    //    {
    //        ModeDone(mode, calendarData.ModeData[mode]);
    //    }
    //    //Temp solution, need to refactor using CalendarData
    //   // ModeDone(TaskMode.Challenge, await DataManager.Instance.TodayChallengeStatus());
    //}

    public void OpenPanel()
    {
        rTransform.DOAnchorPosY(-138, 0.25f).SetEase(Ease.InOutQuad);
        //DailyStatusPanel.Instance.OpenPanel();
        //UpdateIndicator();
    }

    public void ClosePanel()
    {
        rTransform.DOAnchorPosY(0, 0.2f).SetEase(Ease.InOutQuad);
        DailyStatusPanel.Instance.ClosePanel();
    }

    private async void UpdateIndicators()
    {
        var today = DateTime.UtcNow;
        var dayResult = await dataService.TaskData.GetDayResult(today);
        var completedModes = dayResult.CompletedModes;
        smallIcon.SetActive(completedModes.Contains(TaskMode.Small));
        mediumIcon.SetActive(completedModes.Contains(TaskMode.Medium));
        largeIcon.SetActive(completedModes.Contains(TaskMode.Large));
        challengeIcon.SetActive(completedModes.Contains(TaskMode.Challenge));
        var rewardIndex = GetAwardIndex(dayResult.Reward);
        rewardIcon.gameObject.SetActive(rewardIndex != -1);
        if (dayResult.IsCompleted && rewardIndex != -1)
        {
            rewardIcon.sprite = rewardSprites[rewardIndex];
        }
        //var status = DailyStatusPanel.Instance.AllModesDone ? DailyModeStatus.Done : DailyModeStatus.InProgress;
        //awardIndicator.Status = status;
    }

    private async UniTask<CalendarData> GetTodayCalendarDate()
    {
        var data = new CalendarData(DateTime.UtcNow.Date);
        var modes = (TaskMode[])Enum.GetValues(typeof(TaskMode));
        foreach (var mode in modes)
        {
            var dateResults = await dataService.TaskData.GetDailyModeData(DateTime.UtcNow, mode);
            data.ModeData.Add(mode, dateResults.IsComplete);
        }
        return data;
    }

    private int GetAwardIndex(Achievements reward)
    {
        switch (reward)
        {
            case Achievements.GoldMedal: return 0;
            case Achievements.SilverMedal: return 1;
            case Achievements.BronzeMedal: return 2;

            default:
                return -1;
        }
    }
}

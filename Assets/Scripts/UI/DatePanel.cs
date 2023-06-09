using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mathy.Data;
using Mathy.Core;
using DG.Tweening;
using System.Globalization;
using UnityEngine.Localization.Settings;
using TMPro;
using Cysharp.Threading.Tasks;

public class DatePanel : ButtonFX
{
    #region FIELDS

    [Header("Components:")]

    [SerializeField] private CalendarManager calendar;
    [SerializeField] private List<DailyModeIndicator> modeIndicators;
    [SerializeField] private DailyModeIndicator awardIndicator;
    [SerializeField] private TMP_Text dateLable;

    private bool isInitialization = true;
    private bool isOpened = false;
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
            UpdateVisual();
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

    #endregion

    protected override void Awake()
    {
        base.Awake();
        DailyStatusPanel.OnAllModesDone.AddListener(UpdateAwardIndicator);
    }

    private async void OnEnable()
    {
        if (!isInitialization)
        {
            //Debug.LogError("Here is supposed to be GetCalendarData from database");
            CalendarData = await DataManager.Instance.GetCalendarData(System.DateTime.UtcNow.Date);
            //CalendarData = await DatabaseHandler..GetCalendarData(System.DateTime.UtcNow.Date);
        }
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
        //Debug.LogError("Here is supposed to be GetCalendarData from database");
        CalendarData = await DataManager.Instance.GetCalendarData(System.DateTime.UtcNow.Date);
        Initializing();
    }

    private async void Initializing()
    {
        //Debug.LogError("Here is supposed to be GetCalendarData from database");
        //this.CalendarData = new CalendarData(DateTime.Today);//temp

        CalendarData = await DataManager.Instance.GetCalendarData(System.DateTime.UtcNow.Date);
        //calendar.SelectCurrentDate();
        UpdateAwardIndicator(); //Temp solution, need to update award icon on start
        isInitialization = false;
    }

	private void Localize()
    {
        dateLable.text = $"{cultureInfo.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month)} {DateTime.UtcNow.Day}";
    }
	
    private void ModeDone(TaskMode mode, bool isDone)
    {
        var status = isDone ? DailyModeStatus.Done : DailyModeStatus.InProgress;
        if(mode != TaskMode.Practic)
        {
            modeIndicators[(int)mode].Status = status;
        }
    }

    private async void UpdateVisual()
    {
        foreach (TaskMode mode in calendarData.ModeData.Keys)
        {
            ModeDone(mode, calendarData.ModeData[mode]);
        }
        //Temp solution, need to refactor using CalendarData
        ModeDone(TaskMode.Challenge, await DataManager.Instance.TodayChallengeStatus());
    }

    public void OpenPanel()
    {
        rTransform.DOAnchorPosY(-138, 0.25f).SetEase(Ease.InOutQuad);
        DailyStatusPanel.Instance.OpenPanel();
        UpdateAwardIndicator();
    }

    public void ClosePanel()
    {
        rTransform.DOAnchorPosY(0, 0.2f).SetEase(Ease.InOutQuad);
        DailyStatusPanel.Instance.ClosePanel();
    }

    public void UpdateAwardIndicator()
    {
        var status = DailyStatusPanel.Instance.AllModesDone ? DailyModeStatus.Done : DailyModeStatus.InProgress;
        awardIndicator.Status = status;
    }
}

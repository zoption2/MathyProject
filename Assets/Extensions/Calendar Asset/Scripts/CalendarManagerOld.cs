using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Data;
using System;
using Mathy.UI;
using Mathy.Core;

/* Скорее всего на удаление
public class CalendarManagerOLD : StaticInstance<CalendarManagerOLD>, IChangeableStyle
{
	#region Fields

	[Header("Components:")]

	[SerializeField] private HeaderManager headerManager;
	[SerializeField] private BodyManager bodyManager;
	[SerializeField] private TailManager tailManager;
	[SerializeField] private DatePanel datePanel;

	[Header("GUI:")]

	[SerializeField] private Image weekImage;
	[SerializeField] private Image headerImage;

	[Header("References:")]

	[SerializeField] private List<Sprite> weekImages;
	[SerializeField] private List<Sprite> headerImages;
    [SerializeField] private DailyResultPanel dailyResultPanel;

	[Header("Config:")]

	[SerializeField] private bool isSundayFirst;

	public DateTime TargetDateTime { get; private set; }

    private CultureInfo cultureInfo;

	#endregion

	#region Public Methods
 
	public void OnGoToPreviousOrNextMonthButtonClicked(string param)
	{
		TargetDateTime = TargetDateTime.AddMonths(param == "Prev" ? -1 : 1);
		Refresh(TargetDateTime.Year, TargetDateTime.Month);
	}

	public void SetLanguage(string language)
	{
		cultureInfo = new CultureInfo(language);
		isSundayFirst = (language == "en-US");
		Refresh(TargetDateTime.Year, TargetDateTime.Month);
		this.gameObject.SetActive(false);
	}

    #endregion

    #region Private Methods

    public DateTime SelectCurrentDate()
    {
		gameObject.SetActive(true);
		Initialize();
		bodyManager.SelectCurrentDay(TargetDateTime.Day.ToString());
		gameObject.SetActive(false);

        return TargetDateTime;
    }

	public void SelectToday()
    {
		bodyManager.SelectCurrentDay(TargetDateTime.Day.ToString());
	}

    private void OnEnable()
    {
		SelectToday();
	}

    private void Start()
	{
		Initialize();
	}

    protected override void Awake()
    {
		base.Awake();
		SubscribeOnDifficultyModeChanged();
	}

	public void ResetToDefault()
    {
		//SelectToday();
		//Initialize();
		datePanel.ResetToDefault();
	}

	private void Initialize()
    {
		TargetDateTime = DateTime.Now;
		//cultureInfo = new CultureInfo("uk-UA");
		cultureInfo = new CultureInfo("en-US");
		Refresh(TargetDateTime.Year, TargetDateTime.Month);
	}
    #endregion

    #region Event Handlers

    private void Refresh(int year, int month)
	{
		headerManager.SetTitle($"{year} {cultureInfo.DateTimeFormat.GetMonthName(month)}");
		bodyManager.Initialize(year, month, isSundayFirst, OnButtonClicked);
	}

	private void OnButtonClicked((string day, string legend, ButtonManager bm) param)
	{
		tailManager.SetLegend($"You have clicked day {param.day}.");
		//Date panel title updates here
		//tailManager.SetDate($"{DateTime.UtcNow.Day} {cultureInfo.DateTimeFormat.GetMonthName(TargetDateTime.Month)}");
		tailManager.SetDate($"{DateTime.UtcNow.Day} {cultureInfo.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month)}");

		TargetDateTime = new DateTime(param.bm.calendarDay.CalendarData.Date.Year, 
			param.bm.calendarDay.CalendarData.Date.Month, Convert.ToInt32(param.day));
		//Debug.Log("TargetDateTime is " + TargetDateTime);

		bodyManager.SelectDay(param.bm);
		string date = $"{param.day}.0{TargetDateTime.Month}.{TargetDateTime.Year}";
	}

    //Used from button in Unity
    public void ViewDetailsOfMode(int mode)
    {
        if (DataManager.Instance.IsDateModeCompleted((TaskMode)mode, TargetDateTime))
        {
			dailyResultPanel.gameObject.SetActive(true);
            dailyResultPanel.Initialize((TaskMode)mode);
        }
        else 
        {
            Debug.LogError("Nothing");
        }
    }

    //Wtf is this
	public List<CalendarDayCell> GetDays()
    {
		return bodyManager.days;
    }

    public void SubscribeOnDifficultyModeChanged()
    {
		GameManager.OnDifficultyModeChanged.AddListener(UpdateDisplayStyle);
	}

    public void UpdateDisplayStyle(int index)
    {
		weekImage.sprite = weekImages[index];
		headerImage.sprite = headerImages[index];
	}

    #endregion
}

//Calendar from new brench, need to be merged MANUALY!!
/* 

public class CalendarManagerOld : StaticInstance<CalendarManagerOld>
{
	#region Fields

	[Header("Components:")]
	[SerializeField] private HeaderManager headerManager;
	[SerializeField] private BodyManager bodyManager;
	[SerializeField] private TailManager tailManager;
	[SerializeField] private DatePanel datePanel;
    [SerializeField] private DailyResultPanel dailyResultPanel;
	[SerializeField] private CalendarPanel calendarPanel;

	[Header("Config:")]
	[SerializeField] private bool isSundayFirst;

	public DateTime TargetDateTime { get; private set; }

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

	#region Public Methods
 
	public void OnGoToPreviousOrNextMonthButtonClicked(string param)
	{
		TargetDateTime = TargetDateTime.AddMonths(param == "Prev" ? -1 : 1);
		Refresh(TargetDateTime.Year, TargetDateTime.Month);
	}

	
    #endregion

    #region Private Methods

    public DateTime SelectCurrentDate()
    {
		gameObject.SetActive(true);
		Initialize();
		bodyManager.SelectCurrentDay(TargetDateTime.Day.ToString());
		gameObject.SetActive(false);

        return TargetDateTime;
    }

	public void SelectToday()
    {
		bodyManager.SelectCurrentDay(TargetDateTime.Day.ToString());
	}

    private void OnEnable()
    {
		Initialize();
		SelectToday();
	}

    private void Start()
	{
		Initialize();
	}

	public void ResetToDefault()
    {
		//SelectToday();
		//Initialize();
		datePanel.ResetToDefault();
	}

	private void Initialize()
    {
		TargetDateTime = DateTime.Now;

		Refresh(TargetDateTime.Year, TargetDateTime.Month);
	}

    #endregion

    #region Event Handlers

    private void Refresh(int year, int month)
	{
		headerManager.SetTitle($"{year} {cultureInfo.DateTimeFormat.GetMonthName(month)}");
		bodyManager.Initialize(year, month, isSundayFirst, OnButtonClicked);
	}

	private void OnButtonClicked((string day, string legend, ButtonManager bm) param)
	{
		tailManager.SetLegend($"You have clicked day {param.day}.");


		bodyManager.SelectDay(param.bm);
		calendarPanel.UpdateViewButtons();
	}

    //Used from button in Unity
    public async void ViewDetailsOfMode(int mode)
    {
        if (await DataManager.Instance.IsDateModeCompleted((TaskMode)mode, TargetDateTime))
        {
			dailyResultPanel.gameObject.SetActive(true);
            dailyResultPanel.Initialize((TaskMode)mode, false);
        }
        else 
        {
            Debug.LogError("Nothing");
        }
    }

    //Wtf is this
	public List<CalendarDayCell> GetDays()
    {
		return bodyManager.days;
    }

    #endregion
}

*/

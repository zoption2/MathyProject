using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Mathy.Data;
using Mathy.UI;
using System;
using TMPro;
using System.Collections;
using Zenject;
using Mathy.Services;
using UnityEngine.Analytics;

public class CalendarManager : StaticInstance<CalendarManager>
{
	private const string kAnswersFormat = "{0}/{1}";
	#region FIELDS

	[Header("COMPONENTS:")]
	[SerializeField] private HeaderManager headerManager;
	[SerializeField] private DatePanel datePanel;
	[SerializeField] private DailyResultPanel dailyResultPanel;
	[SerializeField] private CalendarPanel calendarPanel;
	[SerializeField] private TMP_Text monthYearText;
	[SerializeField] private Button prevMonthButton;
	[SerializeField] private Button nextMonthButton;
	[SerializeField] private Transform cellsContainer;

	[Header("CONFIG:")]
	[SerializeField] private bool isSundayFirst;

	[Header("INFO TEXT:")]
	[SerializeField] private List<TMP_Text> gradeLables;
	[SerializeField] private List<TMP_Text> answerLables;
	[SerializeField] private List<TMP_Text> timeLables;
	[SerializeField] private TMP_Text totalTimeLable;
	[SerializeField] private TMP_Text lessonsTimeLable;
	[SerializeField] private TMP_Text otherTimeLable;

	public bool IsSundayFirst
	{
		get => isSundayFirst;
        set
        {
			isSundayFirst = value;

		}
	}
	public int DaysInMonth
	{
		get => DateTime.DaysInMonth(selectedYear, selectedMonth);
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

	private CalendarData selectedCalendarData;
	public CalendarData SelectedCalendarData
    {
		get => selectedCalendarData;
        set
        {
			selectedCalendarData = value;
			UpdateInfoText();
		}
	}

	public DateTime SelectedDate { get; private set; }
	private int selectedYear { get => SelectedDate.Year; }
	private int selectedMonth { get => SelectedDate.Month; }

	private List<CalendarDayCell> calendarCellButtons;

	private CalendarDayCell selectedCalendarCell;

	private int sModeCorrectAnswers;
	private int mModeCorrectAnswers;
	private int lModeCorrectAnswers;

	private int sModeCorrectRate;
	private int mModeCorrectRate;
	private int lModeCorrectRate;

	private int sModeTotalTasks;
	private int mModeTotalTasks;
	private int lModeTotalTasks;

    private double sTime = 0;
	private double sModeTime
	{
		get => sTime;
		set
		{
            sTime = value;
			SetModeTimeText(0, value);
		}
	}
    private double mTime = 0;
    private double mModeTime
	{
		get => mTime;
		set
		{
            mTime = value;
			SetModeTimeText(1, value);
		}
	}
    private double lTime = 0;
    private double lModeTime
    {
        get => lTime;
        set
        {
            lTime = value;
            SetModeTimeText(2, value);
        }
    }

    private double total = 0;
    private double totalTime
	{
		get => total;
		set
		{
            total = value;
            TimeSpan t = TimeSpan.FromMilliseconds(value);

            string text = string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);
			string title = LocalizationManager.GetLocalizedString("GUI Elements", "Calendar_Total_Time");
			totalTimeLable.text = $"{title} <color=#3097EF>{text}</color>";
		}
	}
    private double lessons = 0;
    private double lessonsTime
	{
		get => lessons;
		set
		{
            lessons = value;
            TimeSpan t = TimeSpan.FromMilliseconds(value);
            string text = string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);
			string title = LocalizationManager.GetLocalizedString("GUI Elements", "Calendar_Lessons");
			lessonsTimeLable.text = $"{title} {text}";
		}
	}
    private double other = 0;
    private double otherTime
	{
		get => other;
		set
		{
            other = value;
            TimeSpan t = TimeSpan.FromMilliseconds(value);
			//string text = string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);
			string text = "";
			string title = LocalizationManager.GetLocalizedString("GUI Elements", "Calendar_Other");
			otherTimeLable.text = $"{title} {text}";
		}
	}

	#endregion
	[Inject] private IDataService dataService;

	#region INITIALIZATION

	protected override void Awake()
	{
		base.Awake();
		Initialize();
	}

    private void Initialize()
    {
		SelectedDate = DateTime.Today;
		if(calendarCellButtons == null)
        {
			calendarCellButtons = new List<CalendarDayCell>();
			CalendarDayCell dayCell;
			foreach (Transform child in cellsContainer)
            {
                if (child.TryGetComponent<CalendarDayCell>(out dayCell))
                {
					calendarCellButtons.Add(dayCell);
					dayCell.Initialize();
				}
			}
				
		}
		prevMonthButton.onClick.AddListener(PreviousMonth);
		nextMonthButton.onClick.AddListener(NextMonth);

		dailyResultPanel.OnPanelOpened.AddListener(calendarPanel.SetInteractableAllViewButtons);

	}

    public void ResetToDefault()
	{
		datePanel.ResetToDefault();
		UpdateCalendar();
	}

	#endregion

	#region CALENDAR PANEL

	public void UpdateHorizontalLayoutGroup(GameObject layoutGroupObject)
	{
		// Get the HorizontalLayoutGroup component
		HorizontalLayoutGroup layoutGroup = layoutGroupObject.GetComponent<HorizontalLayoutGroup>();

		// Check if the layout group exists
		if (layoutGroup == null)
		{
			Debug.LogError("Error: HorizontalLayoutGroup component not found on " + layoutGroupObject.name);
			return;
		}

		// Get all the child Elements of the layout group
		RectTransform[] childElements = layoutGroupObject.GetComponentsInChildren<RectTransform>();

		// Set the layout group's child force expand value to true
		layoutGroup.childForceExpandWidth = true;

		// Update the layout
		layoutGroup.CalculateLayoutInputHorizontal();
		layoutGroup.SetLayoutHorizontal();

		// Set the layout group's child force expand value back to false
		layoutGroup.childForceExpandWidth = false;

		// Update the layout again
		layoutGroup.CalculateLayoutInputHorizontal();
		layoutGroup.SetLayoutHorizontal();

		// Update the ContentSizeFitter components on all the child Elements
		foreach (RectTransform child in childElements)
		{
			ContentSizeFitter fitter = child.GetComponent<ContentSizeFitter>();
			if (fitter != null)
			{
				fitter.SetLayoutHorizontal();
			}
		}
	}

	private void SetGradeText(int modeIndex, string text)
	{
		string title = LocalizationManager.GetLocalizedString("GUI Elements", "Calendar_Grade");
		gradeLables[modeIndex].text = $"{title} <color=#FF6752>{text}</color>";
	}

	private void SetAnswersText(int modeIndex, int correctAnswers, int totalTasks)
	{
		var text = correctAnswers.ToString() + "/" + totalTasks.ToString();
		string title = LocalizationManager.GetLocalizedString("GUI Elements", "Calendar_Answers");
		answerLables[modeIndex].text = $"{title} {text}";		
	}

	private void SetModeTimeText(int modeIndex, double time)
	{
        TimeSpan t = TimeSpan.FromMilliseconds(time);
        string answer = string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);

		string title = LocalizationManager.GetLocalizedString("GUI Elements", "Calendar_Time");
		timeLables[modeIndex].text = $"{title} {answer}";
	}

	public void UpdateAnswersText()
    {
		SetAnswersText(0, sModeCorrectAnswers, sModeTotalTasks);
		SetAnswersText(1, mModeCorrectAnswers, mModeTotalTasks);
		SetAnswersText(2, lModeCorrectAnswers, lModeTotalTasks);
	}

	public void UpdateGradeText(int modeIndex, int correctRate)
	{
        this.SetGradeText(modeIndex, CalculateGrade(correctRate));
    }

    //Later change to GradeSystem
    private string CalculateGrade(int correctRate)
    {
        string grade = "";
        if (correctRate <= 59)
        {
            grade = "F";
        }
        else if (correctRate > 59 && correctRate <= 69)
        {
            grade = "D";
        }
        else if (correctRate > 69 && correctRate <= 79)
        {
            grade = "C";
        }
        else if (correctRate > 79 && correctRate <= 89)
        {
            grade = "B";
        }
        else if (correctRate > 89)
        {
            grade = "A";
        }
        return grade + " (" + correctRate.ToString() + "%)";
    }

    public async void UpdateInfoText()
    {
		//sModeCorrectAnswers = await DataManager.Instance.GetCorrectAnswersOfModeByDate(TaskMode.Small,this.SelectedDate);
		//      mModeCorrectAnswers = await DataManager.Instance.GetCorrectAnswersOfModeByDate(TaskMode.Medium, this.SelectedDate);
		//lModeCorrectAnswers = await DataManager.Instance.GetCorrectAnswersOfModeByDate(TaskMode.Large, this.SelectedDate);
		var smallModeResults = await dataService.TaskData.GetDailyModeData(SelectedDate, TaskMode.Small);
        var mediumModeResults = await dataService.TaskData.GetDailyModeData(SelectedDate, TaskMode.Medium);
        var largeModeResults = await dataService.TaskData.GetDailyModeData(SelectedDate, TaskMode.Large);

		sModeCorrectAnswers = smallModeResults.CorrectAnswers;
		mModeCorrectAnswers = mediumModeResults.CorrectAnswers;
		lModeCorrectAnswers = largeModeResults.CorrectAnswers;

		sModeTotalTasks = smallModeResults.TotalTasks;
		mModeTotalTasks = mediumModeResults.TotalTasks;
		lModeTotalTasks = largeModeResults.TotalTasks;

		sModeCorrectRate = smallModeResults.CorrectRate;
		mModeCorrectRate = mediumModeResults.CorrectRate;
		lModeCorrectRate = largeModeResults.CorrectRate;

		UpdateAnswersText();
		UpdateGradeText(0, sModeCorrectRate);
        UpdateGradeText(1, mModeCorrectRate);
        UpdateGradeText(2, lModeCorrectRate);

		sModeTime = smallModeResults.Duration;
		mModeTime = mediumModeResults.Duration;
		lModeTime = largeModeResults.Duration;

        lessonsTime = sModeTime + mModeTime + lModeTime;
        otherTime = 0; //need to calculate and show ingame time without lessonsTime
		totalTime = lessonsTime + otherTime;
	}

    #endregion

    #region CALENDAR GUI

    public async void ViewDetailsOfMode(int mode)
	{
		//Debug.LogError("Mode details request");
		var modeResult = await dataService.TaskData.GetDailyModeData(SelectedDate, (TaskMode)mode);
		if (modeResult.IsComplete)
		{
            var dayResults = await dataService.TaskData.GetResultsByModeAndDate((TaskMode)mode, SelectedDate);
			int totalTasks = dayResults.Length;
			int correctAnswers = 0;
			double duration = 0;
			for (int i = 0; i < totalTasks; i++)
			{
				correctAnswers = dayResults[i].IsAnswerCorrect
					? ++correctAnswers
					: correctAnswers;
				duration += dayResults[i].Duration;
            }
			int correctRate = (correctAnswers * 100) / totalTasks;
			string answers = string.Format(kAnswersFormat, correctAnswers, totalTasks);

            dailyResultPanel.gameObject.SetActive(true);
            dailyResultPanel.UpdateInfoPanel(correctRate, duration, answers);
            dailyResultPanel.Initialize((TaskMode)mode, false);
        }
        else
        {
            Debug.LogError("Nothing");
        }


        //     if (await DataManager.Instance.IsDateModeCompleted((TaskMode)mode, SelectedDate)) //await DatabaseHandler.IsDateModeCompleted((TaskMode)mode, SelectedDate))
        //     {
        //int correctRate;
        //long modeTime;
        //string completedTasks;

        //switch (mode)
        //{
        //	case 0:
        //		correctRate = sModeCorrectRate;
        //		modeTime = sModeTime;
        //		completedTasks = $"{sModeCorrectAnswers}/{sModeTotalTasks}";
        //		break;
        //	case 1:
        //		correctRate = mModeCorrectRate;
        //		modeTime = mModeTime;
        //		completedTasks = $"{mModeCorrectAnswers}/{mModeTotalTasks}";
        //		break;
        //	case 2:
        //		correctRate = lModeCorrectRate;
        //		modeTime = lModeTime;
        //		completedTasks = $"{lModeCorrectAnswers}/{lModeTotalTasks}";
        //		break;
        //	default:
        //		goto case 0;
        //}

        //dailyResultPanel.gameObject.SetActive(true);
        //dailyResultPanel.UpdateInfoPanel(correctRate, modeTime, completedTasks);
        //dailyResultPanel.Initialize((TaskMode)mode, false);
        //     }

    }

	public async virtual void UpdateCalendar()
    {
		monthYearText.text = $"{SelectedDate.Year} {cultureInfo.DateTimeFormat.GetMonthName(SelectedDate.Month)}";

		//List<CalendarData> calendarDatas = await DataManager.Instance.GetCalendarData(selectedMonth, selectedYear);

		foreach (var calendarCell in calendarCellButtons)
		{
			int cellDay = calendarCell.CurrentNumber;

			if (cellDay > 0 && cellDay <= DaysInMonth)
			{
                //if (calendarDatas.Any(x => x.Date.Day == cellDay))
                //{
                //	calendarCell.CalendarData = calendarDatas.FirstOrDefault(x => x.Date.Day == cellDay);
                //}
                //else
                //{
                //var dailyData = await dataService.TaskData.GetDailyData(new DateTime(selectedYear, selectedMonth, cellDay));
                CalendarData emptyCalendarData = new CalendarData(new DateTime(selectedYear, selectedMonth, cellDay));
					foreach (TaskMode mode in (TaskMode[])Enum.GetValues(typeof(TaskMode)))
					{
					var dailyData = await dataService.TaskData.GetDailyModeData(new DateTime(selectedYear, selectedMonth, cellDay), mode);
						//var dailyMode = dailyData.FirstOrDefault(x => x.Mode == mode);
						var modeCompleted = dailyData.IsComplete; 
						emptyCalendarData.ModeData.Add(mode, modeCompleted);
					}

					calendarCell.CalendarData = emptyCalendarData;
				//}
			}

			calendarCell.Init(SelectedDate);
		}
	}

	public void PreviousMonth()
	{
		SelectedDate = new DateTime(SelectedDate.AddYears(SelectedDate.Month == 1 ? -1 : 0).Year, SelectedDate.AddMonths(-1).Month, 1);//SelectedDate.AddMonths(-1);

        calendarPanel.UpdateSelectedDayTitle($"{cultureInfo.DateTimeFormat.GetMonthName(SelectedDate.Month)} {1}");
        calendarPanel.UpdateViewButtons();
		SelectDay(1);
		UpdateCalendar();
	}

	public void NextMonth()
	{
		SelectedDate = new DateTime(SelectedDate.AddYears(SelectedDate.Month == 12 ? 1 : 0).Year, SelectedDate.AddMonths(1).Month, 1);
        
        calendarPanel.UpdateSelectedDayTitle($"{cultureInfo.DateTimeFormat.GetMonthName(SelectedDate.Month)} {1}");
        calendarPanel.UpdateViewButtons();
		SelectDay(1);
		UpdateCalendar();
	}

	//Returns day of the week for the first day of the month. Sunday starts at 1 - Saturday is 7
	public int FirstOfMonthDay()
	{
		DateTime firstOfMonth = new DateTime(selectedYear, selectedMonth, isSundayFirst ? 1 : 7);
		return (int)firstOfMonth.DayOfWeek + 1;
	}
	
	public DateTime ReturnDate(int selectedDay)
	{
		DateTime selectedDate = new DateTime(selectedYear, selectedMonth, selectedDay);
		return selectedDate;
	}

	private void SelectDay(int selectedDay)
    {
		if (selectedCalendarCell != null) selectedCalendarCell.IsSelected = false;
		SelectedDate = ReturnDate(selectedDay);
		selectedCalendarCell = calendarCellButtons.FirstOrDefault(d => d.CurrentNumber == selectedDay);
		SelectedCalendarData = selectedCalendarCell.CalendarData;
		selectedCalendarCell.IsSelected = true;
		calendarPanel.UpdateViewButtons();
		calendarPanel.UpdateSelectedDayTitle($"{cultureInfo.DateTimeFormat.GetMonthName(SelectedDate.Month)} {SelectedDate.Day}");
	}

	public void OnDayButtonClicked(int selectedDay)
    {
		SelectDay(selectedDay);
	}

	public void SelectToday()
    {
		var today = DateTime.Now;
		SelectedDate = today;
		SelectDay(today.Day);
		UpdateCalendar();
    }

	#endregion
}
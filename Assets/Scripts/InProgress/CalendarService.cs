using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using Mathy.Data;
using System.Linq;
using ModeData = DayData.ModeData;
using static UnityEngine.Rendering.DebugUI;

public interface ICalendarService
{
    event Action<DayData> ON_DAY_DATA_SELECTED;
    event Action ON_RESET_TO_DEFAULT;
    bool IsSundayFirst { get; set; }
    UniTask<DayData> GetLastSelectedDayData();
    UniTask<DayData> GetTodayData();
    UniTask<DayData> SelectDayData(DateTime date);
    UniTask<DayData> GetDayData(DateTime date);
    UniTask<MonthCalendar> GetCalendarForMonth(DateTime date);
}

public class CalendarService : MonoBehaviour, ICalendarService
{
    public event Action<DayData> ON_DAY_DATA_SELECTED;
    public event Action ON_RESET_TO_DEFAULT;

    private const int kSModeTotalTasks = 10;
    private const int kMModeTotalTasks = 20;
    private const int kLModeTotalTasks = 30;

    private DayData lastSelectedDayData;
    private MonthCalendar lastMonthData;

    public bool IsSundayFirst { get; set; } = false;


    public async UniTask<DayData> GetLastSelectedDayData()
    {
        if(lastSelectedDayData == null)
        {
            lastSelectedDayData =  await GetTodayData();
        }
        return lastSelectedDayData;
    }

    public async UniTask<DayData> GetTodayData()
    {
        return await GetDayData(DateTime.UtcNow);
    }

    public async UniTask<DayData> SelectDayData(DateTime date)
    {
        var data = await GetDayData(date);
        lastSelectedDayData = data;
        ON_DAY_DATA_SELECTED?.Invoke(data);
        return data;
    }

    public async UniTask<DayData> GetDayData(DateTime date)
    {
        var dayData = new DayData();
        dayData.ModeS = await GetPersistantData(TaskMode.Small, date);
        dayData.ModeM = await GetPersistantData(TaskMode.Medium, date);
        dayData.ModeL = await GetPersistantData(TaskMode.Large, date);
        return dayData;
    }

    public async UniTask<MonthCalendar> GetCalendarForMonth(DateTime date)
    {
        int[,] matrix = new int[6, 7];
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        int firstDayIndex = IsSundayFirst
                          ? (int)firstDayOfMonth.DayOfWeek
                          : (int)firstDayOfMonth.DayOfWeek - 1;

        int dateCounter = 1;
        for (int matrixIndex = 0; matrixIndex < daysInMonth; matrixIndex++)
        {
            int row = (matrixIndex + firstDayIndex) / 7;
            int col = (matrixIndex + firstDayIndex) % 7;
            matrix[row, col] = dateCounter;
            dateCounter++;
        }

        List<CalendarData> calendarData = await DataManager.Instance.GetCalendarData(date.Month, date.Year);

        MonthCalendar monthData = new MonthCalendar(matrix, calendarData);
        return monthData;
    }

    public void ResetToDefault()
    {
        ON_RESET_TO_DEFAULT?.Invoke();
    }

    private async UniTask<ModeData> GetPersistantData(TaskMode mode, DateTime date)
    {
        ModeData result = new ModeData();
        result.CorrectAnswers = await DataManager.Instance.GetCorrectAnswersOfModeByDate(mode, date);
        result.LessonsTime = await DataManager.Instance.GetTimeOfModeAndDate(mode, date);
        var totalTasks = GetTotalTasksByMode(mode);
        result.Rate = (int) (((float)result.CorrectAnswers / (float)totalTasks) * 100);
        return result;
    }

    private int GetTotalTasksByMode(TaskMode mode)
    {
        switch (mode)
        {
            case TaskMode.Small: return kSModeTotalTasks;
            case TaskMode.Medium: return kMModeTotalTasks;
            case TaskMode.Large: return kLModeTotalTasks;
            default:
                throw new System.NotImplementedException();
        }
    }
}

public class DayData
{
    public ModeData ModeS;
    public ModeData ModeM;
    public ModeData ModeL;

    public TimeSpan GetLessonTimeSpan(long time)
    {
        return TimeSpan.FromTicks(time);
    }

    public struct ModeData
    {
        public int CorrectAnswers;
        public int Rate;
        public int TotalTasks;
        public long LessonsTime;
    }
}

public class MonthCalendar
{
    private readonly int[,] monthMatrix;
    private readonly List<CalendarData> monthModesDatas;

    public MonthCalendar(int[,] matrix, List<CalendarData> calendarDatas)
    {
        monthMatrix = matrix;
        monthModesDatas = calendarDatas;
    }

    public int GetDayByCellIndex(int index)
    {
        int row = index / 7;
        int col = index % 7;
        return monthMatrix[row, col];   
    }

    public DayCompletingData GetDayCompletingData(int day)
    {
        DayCompletingData data = new();
        CalendarData persistdate = monthModesDatas.FirstOrDefault(x => x.Date.Day == day);
        data.isSModeComplete = persistdate.ModeData[TaskMode.Small];
        data.isMModeComplete = persistdate.ModeData[TaskMode.Medium];
        data.isLModeComplete = persistdate.ModeData[TaskMode.Large];
        data.isChallengeComplete = persistdate.ModeData[TaskMode.Challenge];

        return data;
    }

    public struct DayCompletingData
    {
        public bool isSModeComplete;
        public bool isMModeComplete;
        public bool isLModeComplete;
        public bool isChallengeComplete;
    }
}




public abstract class BehaviourWithModel<TModel> : MonoBehaviour, IView where TModel : IModel 
{
    protected TModel Model { get; private set; }

    public void SetModel(TModel model)
    {
        if (model != null)
        {
            return;
        }

        this.Model = model;
        OnModelApply(model);
    }

    protected abstract void OnModelApply(TModel model);

    public void Show(Action onShow = null)
    {
        PrepareBeforeShow();
        gameObject.SetActive(true);
        DoOnShow(onShow);
    }

    public void Hide(Action onHide = null)
    {
        gameObject.SetActive(false);
        DoOnHide(()=>
        {
            onHide?.Invoke();
            gameObject.SetActive(false);
        });
    }

    public void Release()
    {
        DoOnRelease();
    }

    protected virtual void PrepareBeforeShow()
    {
    }

    protected virtual void DoOnShow(Action onComplete)
    {
    }

    protected virtual void DoOnHide(Action onComplete)
    {
    }

    protected virtual void DoOnRelease()
    {
    }
}

public class DayResultsModel : IModel
{
    private const string kLocalizeTableKey = "GUI Elements";
    private const string kLocalizeGradeKey = "Calendar_Grade";
    private const string kLocalizeAnswerKey = "Calendar_Answers";
    private const string kLocalizeTimeKey = "Calendar_Time";

    private ICalendarService calendarService;

    public ReactiveProperty<DayData> SelectedDayData { get; private set; }
    public string LocalizedGrade
    {
        get => LocalizationManager.GetLocalizedString(kLocalizeTableKey, kLocalizeGradeKey);
    }
    public string LocalizedAnswer
    {
        get => LocalizationManager.GetLocalizedString(kLocalizeTableKey, kLocalizeAnswerKey);
    }
    public string LocalizedTime
    {
        get => LocalizationManager.GetLocalizedString(kLocalizeTableKey, kLocalizeTimeKey);
    }

    public DayResultsModel(ICalendarService calendarService)
    {
        this.calendarService = calendarService;
    }

    public async void Init()
    {
        DayData today = await calendarService.GetTodayData();
        SelectedDayData = new ReactiveProperty<DayData>(today);
    }

    public void ClickOn(TaskMode mode)
    {
        switch (mode)
        {
            case TaskMode.Small:
                break;
            case TaskMode.Medium:
                break;
            case TaskMode.Large:
                break;
            default:
                throw new NotImplementedException(
                    string.Format("There is no implementation for {0} at {1}"
                    , mode
                    , typeof(DayResultsModel)
                    ));
        }
    }
}




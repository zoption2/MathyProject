using Mathy.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Zenject;
using Mathy.Services;


public class BodyManager : MonoBehaviour
{
	#region Fields

	[SerializeField] private GameObject buttonPrefab;

	[SerializeField] private GameObject placeHolderPrefab;

	[SerializeField] private List<Transform> week;

    //fix naming
	private List<GameObject> cells { get; set; } = new List<GameObject>();
	public List<CalendarDayCell> days { get; set; } = new List<CalendarDayCell>();
	private ButtonManager currentSelectedDate;
    
    //Again useless ?
	private string today;

	#endregion
	[Inject] private IDataService dataService;

	#region Public Methods

   
    // ТODO: Huge preformance refactoring here 
	public async void Initialize(int year, int month, bool isSundayFirst, Action<(string, string, ButtonManager)> clickEventHandler)
	{
		if (isSundayFirst) 
		{
			week[6].SetAsFirstSibling();
		}
        else
        {
			week[6].SetAsLastSibling();
		}

		var dateTime = new DateTime(year, month, 1);

		var daysInMonth = DateTime.DaysInMonth(year, month);

		var dayOfWeek = (int)dateTime.DayOfWeek - (isSundayFirst ? 0 : 1);

		if (dayOfWeek == -1) dayOfWeek = 6;

		var size = (dayOfWeek + daysInMonth) / 7;

		if ((dayOfWeek + daysInMonth) % 7 > 0)
			size++;

		var arr = new int[size * 7];

		for (var i = 0; i < daysInMonth; i++)
			arr[dayOfWeek + i] = i + 1;

		if (cells == null)
			cells = new List<GameObject>();

		foreach(var c in cells)
			Destroy(c);

		cells.Clear();
		days.Clear();


        //List<CalendarData> calendarDatas = await DataManager.Instance.GetCalendarData(month);

        //Debug.LogError("calendarDatas.Count " + calendarDatas.Count);

		//if (calendarDatas.Count != 0) Debug.Log("calendarDatas.Count = " + calendarDatas.Count);

        foreach (int index in arr)
		{
			GameObject instance = Instantiate(index == 0 ? placeHolderPrefab : buttonPrefab, transform);
			ButtonManager buttonManager = instance.GetComponent<ButtonManager>();

			if (buttonManager != null)
            {
				buttonManager.Initialize(index.ToString(), clickEventHandler);
                CalendarDayCell day = instance.GetComponent<CalendarDayCell>();
				//if (calendarDatas.Count != 0) Debug.Log("calendarData Day format is " + calendarDatas[0].Date.Day);
				//if (calendarDatas.Any(x => x.Date.Day == index))
				//{
				//    day.CalendarData = calendarDatas.FirstOrDefault(x => x.Date.Day == index);
				//}
				//else 
				//{
				var dailyData = await dataService.TaskData.GetDailyData(dateTime);
                    CalendarData temp = new CalendarData(dateTime);
                    foreach (TaskMode mode in (TaskMode[])Enum.GetValues(typeof(TaskMode)))
                    {
					bool modeCompleted = dailyData.FirstOrDefault(x => x.Mode == mode).IsComplete;
                        temp.ModeData.Add(mode, modeCompleted);
                    }

                    day.CalendarData = temp;
				//}
				//day.Date = buttonManager.label.text + "." + dateTime.Month + "." + dateTime.Year;
				days.Add(day);
				buttonManager.calendarDay = day;
			}

			cells.Add(instance);
		}
	}


	public void SelectDay(ButtonManager selectedDate)
    {
		if (currentSelectedDate != null)
		{
			currentSelectedDate.Select(false);
		}
		currentSelectedDate = selectedDate;
		currentSelectedDate.Select(true);
	}

	public void SelectCurrentDay(string date)
    {
		today = date;
		foreach (GameObject cell in cells)
        {
			cell.TryGetComponent<ButtonManager>(out ButtonManager bm);
			if (bm != null && bm.label.text == date)
            {
				cell.GetComponent<Button>().onClick.Invoke();
				StartCoroutine(ActivateSelector());
			}
        }
	}

	IEnumerator ActivateSelector()
    {
		yield return new WaitForSeconds(.1f);
		foreach (GameObject cell in cells)
		{
			cell.TryGetComponent<ButtonManager>(out ButtonManager bm);
			if (bm != null && bm.label.text == today)
			{
				cell.GetComponent<Button>().onClick.Invoke();
				bm.Select(true);
			}
		}
	}

	#endregion
}

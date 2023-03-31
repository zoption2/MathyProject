using UnityEngine;
using UnityEngine.UI;
using Mathy.Data;
using Mathy.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class CalendarDayCell : CalendarCellButton
{
    [SerializeField] private GameObject visualContainer;
    [SerializeField] private TweenedButton tweenedButton;
    [SerializeField] private List<Image> modeImages;
    [SerializeField] private List<Image> modeAreaImages;
    [SerializeField] private List<Sprite> stateImages;
    [SerializeField] private List<Sprite> modeAreaStateImages;
    [SerializeField] private List<Color> textColors;
    private bool isSelected;
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            UpdateVisual();
        }
    }
    private DateTime selectedDate;
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
            UpdateModeIndicators();
        }
    }

    private void ModeDone(int modeIndex, bool isDone)
    {
        modeImages[modeIndex].enabled = isDone;
    }

    private void UpdateModeIndicators()
    {
        foreach (TaskMode mode in calendarData.ModeData.Keys)
        {
            if(mode != TaskMode.Practic) 
            {
                ModeDone((int)mode, calendarData.ModeData[mode]);
            }
        }
    }

    private void UpdateVisual()
    {
        int baseImageIndex, modeImageIndex, textColorIndex;
        if (isSelected)
        {
            baseImageIndex = 2;
            modeImageIndex = 1;
            textColorIndex = 2;
        }
        else
        {
            baseImageIndex = 1;
            modeImageIndex = 0;            
            textColorIndex = selectedDate > DateTime.Now ? 1 : 0;

        }
        image.sprite = stateImages[baseImageIndex];
        foreach (var modeAreaImage in modeAreaImages)
            modeAreaImage.sprite = modeAreaStateImages[modeImageIndex];
        dateText.color = textColors[textColorIndex];
    }

    public override void Init(DateTime date)
    {
        UnityAction onClickAction = new UnityAction(() => CalendarManager.Instance.OnDayButtonClicked(CurrentNumber));
        button.onClick.RemoveAllListeners();
        if (CurrentNumber <= 0 || CurrentNumber > CalendarManager.Instance.DaysInMonth)
        {
            visualContainer.SetActive(false);
            dateText.text = "";
            button.interactable = false;
        }
        else
        {
            selectedDate = new DateTime(date.Year, date.Month, CurrentNumber);
            visualContainer.SetActive(true);
            dateText.text = CurrentNumber.ToString();
            tweenedButton.ResetOnPressTween();
            button.onClick.AddListener(onClickAction);
            button.interactable = true;
        }
        UpdateVisual();
    }
}

using UnityEngine;
using UnityEngine.UI;
using Mathy.Data;
using Mathy.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class CalendarDayCell : CalendarCellButton
{
    [SerializeField] private GameObject visualContainer;
    [SerializeField] private TweenedButton tweenedButton;
    [SerializeField] private List<Image> modeImages;
    [SerializeField] private List<Sprite> stateImages;

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

    public override void UpdateVisual()
    {
        UnityAction onClickAction = new UnityAction(() => CalendarManager.Instance.OnDayButtonClicked(CurrentNumber));
        button.onClick.RemoveAllListeners();
        if (CurrentNumber <= 0 || CurrentNumber > CalendarManager.Instance.DaysInMonth)
        {
            visualContainer.SetActive(false);
            image.sprite = stateImages[0];
            dateText.text = "";
            button.interactable = false;
        }
        else
        {
            visualContainer.SetActive(true);
            image.sprite = stateImages[1];
            dateText.text = CurrentNumber.ToString();
            tweenedButton.ResetOnPressTween();
            button.onClick.AddListener(onClickAction);
            button.interactable = true;
        }
    }
}

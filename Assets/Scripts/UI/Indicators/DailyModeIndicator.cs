using UnityEngine.UI;
using UnityEngine;
using System;

[Serializable]
public enum DailyModeStatus
{
    InProgress = 0,
    Done = 1
}

public class DailyModeIndicator : IndicatorElement<DailyModeStatus>
{
    #region FIELDS

    protected new DailyModeStatus status = DailyModeStatus.InProgress;
    [SerializeField] protected Image doneStatusImage;

    #endregion

    protected override void UpdateDisplayStyle()
    {
        bool isDone = Status == DailyModeStatus.Done;
        doneStatusImage.gameObject.SetActive(isDone);
    }
}

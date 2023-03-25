using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

[Serializable] public enum TaskStatus
{
    Pending = 0,
    InProgress = 1,
    Right = 2,
    Wrong = 3
}
namespace Mathy.UI
{
    public class TaskIndicator : IndicatorElement<TaskStatus>
    {
        #region FIELDS

        protected new TaskStatus status = TaskStatus.Pending;
        [SerializeField] protected Image icon;
        [SerializeField] protected List<Sprite> statusIcons;

        #endregion

        protected override void UpdateDisplayStyle()
        {
            icon.sprite = statusIcons[(int)(object)Status];
        }
    }
}
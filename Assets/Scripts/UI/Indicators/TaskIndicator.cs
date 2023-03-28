using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
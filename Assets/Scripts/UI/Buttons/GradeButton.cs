using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GradeStatus
{
    Pending = 0,
    Current = 1,
    Achieved = 2
}

public class GradeButton : ButtonFX
{
    #region Fields

    [Header("Components:")]

    [SerializeField] private Image buttonImage;
    [SerializeField] private Image icon;
    private RectTransform iconRTransform;

    [Header("References:")]

    [SerializeField] private List<Sprite> buttonImages;
    public GradeStatus Status { get; private set; } = GradeStatus.Pending;

    #endregion

    protected override void Initialization()
    {
        base.Initialization();
        iconRTransform = icon.GetComponent<RectTransform>();
        UpdateDisplayStyle();
    }

    public void SetStatus(GradeStatus newStatus)
    {
        Status = newStatus;
        UpdateDisplayStyle();
    }

    private void UpdateDisplayStyle()
    {
        int index = (int)Status;
        buttonImage.sprite = buttonImages[index];
        icon.color = index == 0 ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.white;
    }

    protected override void OnTweenComplete()
    {
        base.OnTweenComplete();

        if (Status == GradeStatus.Achieved)
        {
            //Do some staff if already achieved, maby open reward panel 
        }
    }
}

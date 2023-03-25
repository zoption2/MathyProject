using Mathy.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardButton : TweenedButton
{
    public enum ButtonState
    {
        Pending = 0,
        Active = 1,
        Claimed = 2,
        Missed = 3
    }

    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private Image checkmarkIcon;

    [Header("REFERENCES:")]
    [SerializeField] private List<Sprite> iconImages;
    [SerializeField] private List<Sprite> buttonImages;

    private ButtonState state = ButtonState.Pending;
    public int State
    {
        get => (int)state;
        set
        {
            state = (ButtonState)value;
            UpdateButtonState();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        iconImage.sprite = iconImages[(int)state];
        buttonImage.sprite = buttonImages[(int)state];
        checkmarkIcon.enabled = state == ButtonState.Claimed;
        button.interactable = state == ButtonState.Active;
    }

    #endregion
}

using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.Localization;

public class LanguageButton : ButtonFX
{
    #region FIELDS

    [SerializeField] private Locale locale;
    public static UnityEvent<Locale> OnLanguageButtonPressed = new UnityEvent<Locale>();

    #endregion

    protected override void OnTweenComplete()
    {
        base.OnTweenComplete();
        OnLanguageButtonPressed.Invoke(locale);
    }
}
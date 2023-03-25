using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

public class LocalizationManager : StaticInstance<LocalizationManager>
{
    #region FIELDS

    public static UnityEvent OnLanguageChanged = new UnityEvent();

    #endregion

    protected override void Awake()
    {
        base.Awake();
        LanguageButton.OnLanguageButtonPressed.AddListener(SetLanguage);
    }

    #region SET LANGUAGE

    public void SetLanguage(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
        OnLanguageChanged.Invoke();

        GameSettingsManager.Instance.SaveLanguageSettings(locale.Identifier.Code);
    }

    public void SetLanguage(string localeCode)
    {
        SetLanguage(GetLocale(localeCode));
    }

    #endregion

    #region GETTERS

    public static Locale GetLocale(string localeCode) => LocalizationSettings.AvailableLocales.Locales
        .First(locale => locale.Identifier.Code == localeCode);

    public static string GetLocalizedString(string table, string key)
    {
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        return localizedString;
    }

    public static Sprite GetLocalizedSprite(string table, string key)
    {
        Sprite localizedTexture = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(table, key, null);
        return localizedTexture;
    }

    public static string GetLocalizedTaskTitle(string nameKey)
    {
        var title = LocalizationManager.GetLocalizedString("Game Names", nameKey);
        return title;
    }

    #endregion
}

using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using Mathy.Core;

public class SettingsPanel : PopupPanel
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private TextMeshProUGUI versionTextLabel;
    [SerializeField] private Image languageIcon;
    [SerializeField] private LanguagesPanel languageWindow;
    [SerializeField] private PopupPanel resetWindow;
    [SerializeField] private PopupPanel maxNumberWindow;
    [SerializeField] private PopupPanel skillPlanWindow;
    [SerializeField] private List<Sprite> flags;

    [Header("BUTTONS:")]
    [SerializeField] private Button maxNumberButton;
    [SerializeField] private Button languageButton;
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private Button setMaxNumberButton;
    [SerializeField] private Button skillPlanButton;

    [Header("TOGGLES:")]
    public Toggle soundsToggle;
    public Toggle musicToggle;
    public Toggle vibrationToggle;

    [Header("SLIDERS:")]
    public Slider maxNumberSlider;

    #endregion

    private void Awake()
    {
        UpdateVersionText();
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        Subscribe(true);
        //UpdateFlagIcon();
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
        Subscribe(false);
    }

    private void UpdateVersionText()
    {
        versionTextLabel.text = "Version: " + Application.version;
    }

    private void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            languageWindow.OnPanelClosed.AddListener(OnModalWindowClose);
            //languageWindow.OnPanelClosed.AddListener(UpdateFlagIcon);
            LanguageButton.OnLanguageButtonPressed.AddListener(languageWindow.ClosePanel);
            languageButton.onClick.AddListener(() => OpenModalWindow(0));

            resetProgressButton.onClick.AddListener(() => OpenModalWindow(1));
            resetWindow.OnPanelClosed.AddListener(OnModalWindowClose);

            maxNumberButton.onClick.AddListener(() => OpenModalWindow(2));
            setMaxNumberButton.onClick.AddListener(SetMaxNumber);
            maxNumberWindow.OnPanelClosed.AddListener(OnModalWindowClose);

            skillPlanButton.onClick.AddListener(() => OpenModalWindow(3));
            skillPlanWindow.OnPanelClosed.AddListener(OnModalWindowClose);
        }
        else
        {
            languageWindow.OnPanelClosed.RemoveListener(OnModalWindowClose);
            //languageWindow.OnPanelClosed.RemoveListener(UpdateFlagIcon);
            LanguageButton.OnLanguageButtonPressed.RemoveListener(languageWindow.ClosePanel);
            languageButton.onClick.RemoveListener(() => OpenModalWindow(0));


            resetProgressButton.onClick.RemoveListener(() => OpenModalWindow(1));
            resetWindow.OnPanelClosed.RemoveListener(OnModalWindowClose);

            maxNumberButton.onClick.RemoveListener(() => OpenModalWindow(2));
            setMaxNumberButton.onClick.RemoveListener(SetMaxNumber);
            maxNumberWindow.OnPanelClosed.RemoveListener(OnModalWindowClose);

            skillPlanButton.onClick.RemoveListener(() => OpenModalWindow(3));
            skillPlanWindow.OnPanelClosed.RemoveListener(OnModalWindowClose);
        }
    }

    public void UpdateFlagIcon()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        Sprite flag;
        switch (localeCode)
        {
            case "en":
                flag = flags[0];
                break;
            case "ru":
                flag = flags[1];
                break;
            case "uk":
                flag = flags[2];
                break;
            default:
                goto case "en";
        }
        languageIcon.sprite = flag;
    }

    private void OpenModalWindow(int windowIndex)
    {
        popupPanel.SetAsFirstSibling();
        //var modalWindow = windowIndex == 0 ? 
        //    languageWindow.gameObject : resetWindow.gameObject;
        GameObject modalWindow;
        switch (windowIndex)
        {
            case 0:
                modalWindow = languageWindow.gameObject;
                break;
            case 1:
                modalWindow = resetWindow.gameObject;
                break;
            case 2:
                modalWindow = maxNumberWindow.gameObject;
                break;
            case 3:
                modalWindow = skillPlanWindow.gameObject;
                break;
            default:
                goto case 0;
        }
        modalWindow.SetActive(true);
    }

    public void OnModalWindowClose()
    {
        popupPanel.SetAsLastSibling();
    }

    public void SetMaxNumber()
    {
        GameSettingsManager.Instance.MaxNumber = (int)maxNumberSlider.value * 10;
        maxNumberWindow.ClosePanel();
    }
}
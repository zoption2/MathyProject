using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Data;
using TMPro;
using UnityEngine.Localization.Settings;

public class PlayButton : ButtonFX
{
    #region FIELDS

    [Header("Components:")]

    [SerializeField] private Image textImage;
    [SerializeField] private TMP_Text taskAmountText;
    [SerializeField] private GameObject playIcon;
    [SerializeField] private ChallengeCupButton challengeCup;

    [Header("References:")]

    [SerializeField] protected List<Sprite> startTextImages;
    [SerializeField] protected List<Sprite> viewTextImages;
    [SerializeField] protected List<Sprite> continueTextImages;
    [SerializeField] protected List<Sprite> startChallengeTextImages;

    private Sprite startTextImage;
    private Sprite viewTextImage;
    private Sprite continueTextImage;
    private Sprite startChallengeTextImage;

    private string startText = "Start Level";
    private string viewText = "View Result";
    private string continueText = "Continue";

    private float startFontSize = 90f;
    private float continueFontSize = 48f;

    private int maxTaskAmount;
    private UnityAction playAction;
    private UnityAction playChallengeAction;

    RectTransform rectTaskAmountText;
    #endregion

    protected override void Initialization()
    {
        base.Initialization();

        playAction = new UnityAction(PlayButtonPanel.Instance.Play);
        playChallengeAction = new UnityAction(challengeCup.RunChallenge);
        LocalizationManager.OnLanguageChanged.AddListener(Localize);
        rectTaskAmountText = (RectTransform)taskAmountText.transform;
    }

    private void Localize()
    {
        startText = LocalizationManager.GetLocalizedString("GUI Elements", "PlayButton_Start");
        viewText = LocalizationManager.GetLocalizedString("GUI Elements", "PlayButton_View");
        continueText = LocalizationManager.GetLocalizedString("GUI Elements", "PlayButton_Continue");
        UpdateTextImages();
        UpdateText();
    }

    public void UpdateTextImages()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        switch (localeCode)
        {
            case "en":
                startTextImage = startTextImages[0];
                viewTextImage = viewTextImages[0];
                continueTextImage = continueTextImages[0];
                startChallengeTextImage = startChallengeTextImages[0];
                break;
            case "ru":
                startTextImage = startTextImages[1];
                viewTextImage = viewTextImages[1];
                continueTextImage = continueTextImages[1];
                startChallengeTextImage = startChallengeTextImages[1];
                break;
            case "uk":
                startTextImage = startTextImages[2];
                viewTextImage = viewTextImages[2];
                continueTextImage = continueTextImages[2];
                startChallengeTextImage = startChallengeTextImages[2];
                break;
            default:
                goto case "en";
        }
    }

    private async void UpdateText()
    {
        
        //DataManager.Instance.Test();
        int taskAmount = await DataManager.Instance.TodayDoneTasksAmount(PlayButtonPanel.Instance.SelectedTaskMode);
        //Debug.LogError("Here was TodayDoneTasksAmount");
        //int taskAmount = 0;
        //bool isModeCompleted = false;
        
        bool isModeCompleted = PlayButtonPanel.Instance.SelectedTaskMode != TaskMode.Challenge ? 
            await DataManager.Instance.IsTodayModeCompleted(PlayButtonPanel.Instance.SelectedTaskMode) :
            await DataManager.Instance.TodayChallengeStatus();
        

        string text;
        if (taskAmount == 0)
        {
            text = maxTaskAmount.ToString();
            textImage.sprite = startTextImage;
            taskAmountText.fontSize = startFontSize;
            rectTaskAmountText.anchoredPosition = new Vector2(6f, rectTaskAmountText.anchoredPosition.y);
        }
        else
        {
            taskAmountText.fontSize = continueFontSize;
            rectTaskAmountText.anchoredPosition = new Vector2(16, rectTaskAmountText.anchoredPosition.y);

            text = taskAmount.ToString() + "/" + maxTaskAmount;
            if (isModeCompleted)
            {
                textImage.sprite = viewTextImage;
            }
            else
            {
                textImage.sprite = continueTextImage;
            }
        }
        if(PlayButtonPanel.Instance.SelectedTaskMode == TaskMode.Challenge)
        {
            textImage.sprite = startChallengeTextImage;
        }
        SetOnPress(isModeCompleted);
        SetText(text);
    }

    private void SetText(string newValue)
    {
        taskAmountText.text = newValue;
    }

    public void UpdateDisplayStyle(int tasksAmount)
    {
        bool isChallenge = PlayButtonPanel.Instance.SelectedTaskMode == TaskMode.Challenge;
        if (isChallenge)
        {
            playIcon.SetActive(false);
            taskAmountText.gameObject.SetActive(false);
            challengeCup.gameObject.SetActive(true);
        }
        else
        {
            playIcon.SetActive(true);
            taskAmountText.gameObject.SetActive(true);
            challengeCup.gameObject.SetActive(false);
            maxTaskAmount = tasksAmount;
        }
        UpdateText();
    }

    private void SetOnPress(bool isCompleted)
    {
        button.onClick.RemoveAllListeners();
        UnityAction onPressAction = PlayButtonPanel.Instance.SelectedTaskMode != TaskMode.Challenge ? 
            playAction : playChallengeAction;
        button.interactable = !isCompleted;
        button.onClick.AddListener(onPressAction);
    }
}
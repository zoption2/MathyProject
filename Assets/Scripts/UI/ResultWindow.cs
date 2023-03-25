using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;
using Mathy.Core;
using Mathy.Core.Tasks;
using UnityEngine.Localization.Settings;

public class ResultWindow : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private Transform popupPanel;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private RawImage bgImage;
    [SerializeField] private Image titleTextImage;
    [SerializeField] private GameObject cupIcon;
    [SerializeField] private ShinyStarIcon starIcon;
    [SerializeField] private Slider gradeSlider;
    [SerializeField] private TextMeshProUGUI titleTextLabel;
    [SerializeField] private TextMeshProUGUI gradeTextLabel;    
    [SerializeField] private TextMeshProUGUI doneTasksAmount;
    [SerializeField] private List<TextMeshProUGUI> resultTextLabels;
    [SerializeField] private List<GameObject> infoPanels;
    [SerializeField] private List<TextMeshProUGUI> awardTextLabels;
    [SerializeField] private List<TextMeshProUGUI> gradeLetters;
    [SerializeField] private List<GameObject> toolbarButtons;
    [SerializeField] private ButtonFX closeButton;

    [SerializeField] private List<Sprite> lessonCompletedTextImages;
    [SerializeField] private List<Sprite> challengeCompletedTextImages;

    [Header("TWEEN:")]
    [SerializeField] private Vector3 scaleTo = new Vector3(0.05f, 0.025f, 0);
    [SerializeField] private float startSliderValue = 15f;
    [SerializeField] private float sliderStepFactor = 18.75f;

    private int lastExperienceReward;
    private string titleText;
    private string resultText;
    private string awardValue;
    private string lessonCompletedText;
    private string challengeCompletedText;
    private string challengeFailedText;
    private string wellDoneText;
    private string gameOverText;
    private Sprite lessonTextImage;
    private Sprite challengeTextImage;
    private Sprite currentTitleTextSprite;

    private ToolbarButton doubleButton;
    private Transform gradeLabel;
    private GameObject infoPanel;
    private TextMeshProUGUI awardTextLabel;
    private TextMeshProUGUI resultTextLabel;
    private bool isChallenge;
    private bool wasRewardedAdShown = false;

    public bool IsChallenge
    {
        get => isChallenge;
        set
        {
            isChallenge = value;
            UpdateToolbarButtons();
            UpdateInfoPanel(); // activating/deactivating GUI Elements for the correct display style
        }
    }

    #endregion

    public void LocalizeTextImages()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        switch (localeCode)
        {
            case "en":
                lessonTextImage = lessonCompletedTextImages[0];
                challengeTextImage = challengeCompletedTextImages[0];

                break;
            case "ru":
                lessonTextImage = lessonCompletedTextImages[1];
                challengeTextImage = challengeCompletedTextImages[1];
                break;
            case "uk":
                lessonTextImage = lessonCompletedTextImages[2];
                challengeTextImage = challengeCompletedTextImages[2];
                break;
            default:
                goto case "en";
        }
        UpdateTextImages();
    }

    private void UpdateToolbarButtons()
    {
        bool isPractice = isChallenge ? ChallengesManager.Instance.IsPractice : TaskManager.Instance.IsPractice;
        bool isActive = isChallenge && isPractice;

        RectTransform homeButtonRect = (RectTransform)toolbarButtons[0].transform;
        RectTransform doubleRewardButtonRect = (RectTransform)toolbarButtons[1].transform;

        //toolbarButtons[1].SetActive(isActive);
        homeButtonRect.anchoredPosition = new Vector2(isActive ? 280 : 380, homeButtonRect.anchoredPosition.y);
        doubleRewardButtonRect.anchoredPosition = new Vector2(isActive ? -280 : -380, doubleRewardButtonRect.anchoredPosition.y);

        closeButton.OnTweenCompleteEvent.RemoveAllListeners();
        if (isPractice && !isChallenge)
        {
            closeButton.OnTweenCompleteEvent.AddListener(() => OpenDifficultyMenu());
        }
        else
        {
            closeButton.OnTweenCompleteEvent.AddListener(() => ClosePanel());
        }
        //closeButton.OnTweenCompleteEvent.AddListener(isPractice && !isChallenge ? () => OpenDifficultyMenu() : () => ClosePanel());
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        OpenPanel();
    }

    private void Initialize()
    {
        LocalizationManager.OnLanguageChanged.AddListener(Localize);
        Localize();
        gradeLabel = gradeTextLabel.transform.parent;
        doubleButton = toolbarButtons[1].GetComponent<ToolbarButton>();
        bool isIOS = Application.platform == RuntimePlatform.IPhonePlayer;
        doubleButton.gameObject.SetActive(!isIOS);
    }

    private void Localize()
    {
        LocalizeTextImages();
        lessonCompletedText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_LessonCompleted");
        challengeCompletedText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_ChallengeCompleted");
        challengeFailedText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_ChallengeFailed");
        wellDoneText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_WellDone");
        gameOverText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_GameOver");
    }

    public void DisplayResult(int correctAnswers, int tasksAmount, float correctRate, bool isChallengeTask)
    {
        IsChallenge = isChallengeTask;        
        if (isChallenge)
        {
            CheckIfFailed(correctRate);
            currentTitleTextSprite = challengeTextImage;
        }
        else
        {
            UpdateDoneTasksAmount(correctAnswers, tasksAmount);
            //titleText = lessonCompletedText;
            currentTitleTextSprite = lessonTextImage;
        }
        
        CalculateGrade(correctRate, isChallenge);
        doubleButton.IsInteractable = correctRate > 0;
    }

    private void CheckIfFailed(float correctRate)
    {
        bool isFailed = correctRate <= 0;
        cupIcon.SetActive(!isFailed);
        //starIcon.UpdateDisplayStyle(!isFailed);
        if (isFailed)
        {
            //titleText = challengeFailedText;
            currentTitleTextSprite = lessonTextImage;
            resultText = gameOverText;
        }
        else
        {
            //titleText = challengeCompletedText;
            currentTitleTextSprite = lessonTextImage;
            resultText = wellDoneText;
        }
    }

    private void CalculateGrade(float correctRate, bool isChallenge)
    {
        int experience = 0;
        int stars = 0;
        string grade = "F";
        
        // Grade A
        if (correctRate > 96)
        {
            grade = "A+";
            experience = 200;
            stars = 25;
        }
        else if (correctRate > 92)
        {
            grade = "A";
            experience = 150;
            stars = 20;
        }
        else if (correctRate > 89)
        {
            grade = "A-";
            experience = 120;
            stars = 15;
        }

        // Grade B
        else if (correctRate > 86)
        {
            grade = "B+";
            experience = 100;
            stars = 10;
        }
        else if (correctRate > 82)
        {
            grade = "B";
            experience = 90;
            stars = 9;
        }
        else if (correctRate > 79)
        {
            grade = "B-";
            experience = 80;
            stars = 8;
        }

        // Grade C
        else if (correctRate > 76)
        {
            grade = "C+";
            experience = 70;
            stars = 7;
        }
        else if (correctRate > 72)
        {
            grade = "C";
            experience = 60;
            stars = 6;
        }
        else if (correctRate > 69)
        {
            grade = "C-";
            experience = 50;
            stars = 5;
        }

        // Grade D
        else if (correctRate > 66)
        {
            grade = "D+";
            experience = 40;
            stars = 4;
        }
        else if (correctRate > 62)
        {
            grade = "D";
            experience = 30;
            stars = 3;
        }
        else if (correctRate > 59)
        {
            grade = "D-";
            experience = 20;
            stars = 2;
        }

        // Grade F
        else if (correctRate > 0)
        {
            grade = "F+";
            experience = 10;
            stars = 1;
        }
        else
        {
            grade = "F";
            experience = 0;
            stars = 0;
        }

        SetResultText(grade);

        if (!isChallenge)
        {            
            TweenSlider(grade);
        }

        PlayerDataManager.Instance.AddExperience(experience);
        lastExperienceReward = experience;
        awardValue = "+" + experience + " XP";
        UpdateTextLabels();
        UpdateTextImages();
    }

    public void DoubleReward()
    {
        AdManager.Instance.OnUserEarnedRewardEvent.AddListener(RewardedAdEarned);
        AdManager.Instance.ShowRewardedAd(lastExperienceReward);
    }

    private void RewardedAdEarned()
    {
        wasRewardedAdShown = true;
        doubleButton.IsInteractable = false;
        awardTextLabel.text = "+" + (lastExperienceReward * 2) + " XP"; //Temp, need to change text on Earned Reward Event in AdManager
        AdManager.Instance.OnUserEarnedRewardEvent.RemoveListener(RewardedAdEarned);
    }

    private void SetResultText(string grade)
    {
        switch (grade)
        {
            case "A+":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_Great");
                break;
            case "A":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_Wonderful");
                break;
            case "A-":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_Perfect");
                break;
            case "B+":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_YouSmart");
                break;
            case "B":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_Fantastic");
                break;
            case "B-":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_YouFantastic");
                break;
            case "C+":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_YouClever");
                break;
            case "C":
                goto case "C+";
            case "C-":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_Okay");
                break;
            case "D+":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_NextTime");
                break;
            case "D":
                goto case "D+";
            case "D-":
                goto case "D+";
            case "F+":
                resultText = LocalizationManager.GetLocalizedString("GUI Elements", "ResultWindow_TryAgain");
                break;
            case "F":
                goto case "F+";
            default:
                goto case "A+";
        }
    }

    private void TweenSlider(string grade)
    {
        gradeTextLabel.text = grade;
        gradeSlider.value = startSliderValue;

        TextMeshProUGUI gradeLetter = gradeLetters.First(i => i.text.Contains(grade.First()));
        float gradeIndex = gradeLetters.IndexOf(gradeLetter);
        float signFactor = 0;
        
        if (grade.Contains("+") || grade.Contains("-"))
        {
            signFactor = sliderStepFactor / 2 * (grade.Contains("+") ? 1 : -1);
        }

        float endValue = startSliderValue + sliderStepFactor * gradeIndex + signFactor;

        float duration = AudioManager.Instance.GetAudioLength(AudioManager.Instance.gradeSliderSound) * 0.65f;
        AudioManager.Instance.GradeSliderSound();

        var sequence = DOTween.Sequence();
        sequence.Join(gradeSlider.DOValue(endValue, duration).SetEase(Ease.InOutQuad));
        sequence.Append(gradeLabel.DOPunchScale(new Vector2(0.25f, -0.1f), 0.5f).SetEase(Ease.InOutQuad));
    }

    private void UpdateInfoPanel ()
    {
        int index = isChallenge ? 1 : 0;
        foreach (GameObject panel in infoPanels) panel.SetActive(false);
        infoPanel = infoPanels[index];
        infoPanel.SetActive(true);
        awardTextLabel = awardTextLabels[index];
        resultTextLabel = resultTextLabels[index];
    }

    private void UpdateDoneTasksAmount (int correctAnswers, int tasksAmount)
    {
        string text = TaskManager.Instance.IsPractice ?
            correctAnswers.ToString() :
            correctAnswers.ToString() + "/" + tasksAmount.ToString();
        doneTasksAmount.text = text;
    }

    private void UpdateTextLabels()
    {
        awardTextLabel.text = awardValue;
        //titleTextLabel.text = titleText;
        resultTextLabel.text = resultText;
    }

    private void UpdateTextImages()
    {
        titleTextImage.sprite = currentTitleTextSprite;
    }

    public void OpenPanel()
    {
        background.alpha = 0;
        background.DOFade(1, 0.5f);

        popupPanel.localScale = Vector3.zero;
        popupPanel.DOScale(Vector3.one, 0.2f).OnComplete(() => OnComplete(true));
    }

    public void ClosePanel()
    {
        background.DOFade(0, 0.5f);
        popupPanel.DOScale(Vector3.zero, 0.2f).OnComplete(() => OnComplete(false));
        if (!wasRewardedAdShown)
        {
            AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 30);
        }
        wasRewardedAdShown = false;
    }

    private void OpenDifficultyMenu()
    {
        TaskManager.Instance.EnableDifficultyMenu(true);
        background.DOFade(0, 0.5f);
        popupPanel.DOScale(Vector3.zero, 0.2f).OnComplete(() => gameObject.SetActive(false));
        AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 30);
    }

    private void OnComplete(bool isOpened)
    {
        if (isOpened)
        {
            popupPanel.DOShakeScale(0.2f, scaleTo, 5, 30);
        }
        else
        {
            gameObject.SetActive(false);
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
}
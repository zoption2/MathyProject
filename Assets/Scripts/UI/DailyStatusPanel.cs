using DG.Tweening;
using Mathy;
using Mathy.Data;
using Mathy.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class DailyStatusPanel : StaticInstance<DailyStatusPanel>
{
    #region FIELDS
    [Inject] private IDataService dataService;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;

    [Header("Components:")]
    [SerializeField] private Image awardImage;
    [SerializeField] private Image topTextImage;
    [SerializeField] private Image bottomTextImage;

    [Header("References:")]
    [SerializeField] private List<Sprite> awardMedals;
    [SerializeField] private List<Sprite> topTextImages;
    [SerializeField] private List<Sprite> bottomTextImages;

    private RectTransform rTransform;
    private bool allModesDone = false;
    //public static UnityEvent OnAllModesDone = new UnityEvent();

    public bool AllModesDone
    {
        get
        {
            return allModesDone;
        }
        set
        {
            allModesDone = value;
            //OnAllModesDone.Invoke();
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        rTransform = (RectTransform)transform;
        vfx.Stop();
    }

    private async void OnEnable()
    {
        LocalizeTextImages();
        LocalizationManager.OnLanguageChanged.AddListener(LocalizeTextImages);
        var today = System.DateTime.UtcNow;
        var dayResult = await dataService.TaskData.GetDayResult(today);
        if (dayResult.IsCompleted)
        {
            Debug.LogFormat("Day completed! Your reward is {0}", dayResult.Reward);
            var rewardIndex = GetAwardIndex(dayResult.Reward);
            if (rewardIndex == -1)
            {
                successPanel.SetActive(false);
                failPanel.SetActive(true);
            }
            else
            {
                successPanel.SetActive(true);
                failPanel.SetActive(false);
                SetAwardImage(rewardIndex);
            }

            OpenPanel();
        }
    }

    private void OnDisable()
    {
        rTransform.anchoredPosition = new Vector2(rTransform.anchoredPosition.x, 1625);
        LocalizationManager.OnLanguageChanged.RemoveListener(LocalizeTextImages);
    }

    //private void AllModesDoneReward()
    //{
    //    Debug.Log("AllModesDoneReward Called!");
    //    if (!DataManager.Instance.WasTodayAwardGot)
    //    {
    //        PlayerDataManager.Instance.AllModesDoneReward();
    //        DataManager.Instance.WasTodayAwardGot = true;
    //        Debug.Log("GoldenAmount was ++");
    //    }
    //}

    public void OpenPanel()
    {
        vfx.Play();
        rTransform.DOAnchorPosY(193, 0.5f).SetEase(Ease.InOutQuad);
    }

    public void ClosePanel()
    {
        if (allModesDone)
        {
            //rTransform.anchoredPosition = new Vector2(36, -755);
            rTransform.DOAnchorPosY(0, 0.2f).SetEase(Ease.InOutQuad);
        }
    }

    public void LocalizeTextImages()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        switch (localeCode)
        {
            case "en":
                topTextImage.sprite = topTextImages[0];
                bottomTextImage.sprite = bottomTextImages[0];
                break;
            case "ru":
                topTextImage.sprite = topTextImages[1];
                bottomTextImage.sprite = bottomTextImages[1];
                break;
            case "uk":
                topTextImage.sprite = topTextImages[2];
                bottomTextImage.sprite = bottomTextImages[2];
                break;
            default:
                goto case "en";
        }
    }

    private void SetAwardImage(int resultIndex)
    {
        awardImage.sprite = awardMedals[resultIndex];
    }

    private int GetAwardIndex(Achievements reward)
    {
        switch (reward)
        {
            case Achievements.GoldMedal: return 0;
            case Achievements.SilverMedal: return 1;
            case Achievements.BronzeMedal:return 2;

            default:
                return -1;
        }
    }
}

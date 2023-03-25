using DG.Tweening;
using Mathy.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class DailyStatusPanel : StaticInstance<DailyStatusPanel>
{
    #region FIELDS

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
    public static UnityEvent OnAllModesDone = new UnityEvent();

    public bool AllModesDone
    {
        get
        {
            return allModesDone;
        }
        set
        {
            allModesDone = value;
            OnAllModesDone.Invoke();
        }
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        rTransform = (RectTransform)transform;
        LocalizationManager.OnLanguageChanged.AddListener(LocalizeTextImages);
        OnAllModesDone.AddListener(AllModesDoneReward);
    }

    private void AllModesDoneReward()
    {
        Debug.Log("AllModesDoneReward Called!");
        if (!DataManager.Instance.WasTodayAwardGot)
        {
            PlayerDataManager.Instance.AllModesDoneReward();
            DataManager.Instance.WasTodayAwardGot = true;
            Debug.Log("GoldenAmount was ++");
        }
    }

    public void OpenPanel()
    {
        if (allModesDone)
        {
            //rTransform.anchoredPosition = new Vector2(36, 0);
            rTransform.DOAnchorPosY(-755, 0.25f).SetEase(Ease.InOutQuad);
        }
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

    public void SetAwardImage(int resultIndex)
    {
        awardImage.sprite = awardMedals[resultIndex];
    }
}

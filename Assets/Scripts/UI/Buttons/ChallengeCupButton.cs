using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Mathy.Data;
using Mathy.Core;
using UnityEngine.Localization.Components;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;
using System;
using System.Collections;

public class ChallengeCupButton : ButtonFX
{
    #region FIELDS

    [SerializeField] private List<ScriptableTask> challenges;

    [Header("Components:")]

    [SerializeField] protected Image playText;
    [SerializeField] private Image starBG;
    [SerializeField] private Image ribbon;
    [SerializeField] private Transform cup;
    [SerializeField] private ParticleSystem winFX;
    [SerializeField] protected List<Sprite> playTextImages;

    private Transform playTransform;
    private bool isCompleted = false;

    #endregion

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged.AddListener(UpdatePlayTextImage);
        UpdatePlayTextImage();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged.RemoveListener(UpdatePlayTextImage);
    }

    protected override void Initialization()
    {
        base.Initialization();
        playTransform = playText.transform;
        //UpdateChallengeStatus();
    }

    public async void UpdateChallengeStatus()
    {
        isCompleted = await DataManager.Instance.TodayChallengeStatus();
        UpdateDisplayStyle();
    }

    private void UpdateDisplayStyle()
    {
        UpdatePlayTextImage();
        starBG.enabled = isCompleted;
        ribbon.enabled = isCompleted;
        if (isCompleted)
        {
            //StartCoroutine(PlayFX(winFX));
        }
        else
        {
            winFX.Stop();
        }
        button.interactable = !isCompleted;
    }

    public void UpdatePlayTextImage()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        Sprite localizedImage;
        switch (localeCode)
        {
            case "en":
                localizedImage = playTextImages[0];
                break;
            case "ru":
                localizedImage = playTextImages[1];
                break;
            case "uk":
                localizedImage = playTextImages[2];
                break;
            default:
                goto case "en";
        }
        playText.sprite = localizedImage;
    }

    protected override void DoTween()
    {
        var sequence = DOTween.Sequence();

        if (isScaling)
        {
            sequence.Join(cup.DOPunchScale(scaleTo, tweenDuration).SetEase(Ease.InOutQuad));
            sequence.Join(playTransform.DOShakeScale(tweenDuration, scaleTo).SetEase(Ease.InOutQuad));
        }
        if (isRotating)
        {
            sequence.Join(rotatingElement.DOShakeRotation(tweenDuration, rotateTo).SetEase(Ease.InOutQuad));
        }
        sequence.OnComplete(() => OnTweenComplete());
    }

    private IEnumerator PlayFX(ParticleSystem fx)
    {
        yield return new WaitForSeconds(0.5f);
        fx.Stop();
        fx.Play();
    }

    public void RunChallenge()
    {
        int randomIndex = UnityEngine.Random.Range(0, challenges.Count);
        ScenesManager.Instance.CreateChallenge(challenges[randomIndex], false);
    }
}
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ExperiencePanel : HeaderBar
{
    #region Fields

    [SerializeField] Slider progressBar;

    #endregion

    protected override async UniTask AsyncUpdateText()
    {
        await UniTask.WaitUntil(() => PlayerDataManager.Instance != null);
        int experience = PlayerDataManager.Instance.PlayerExperience;
        SetText(experience);
    }

    public override void SetText(int newValue)
    {
        string currentExp = newValue.ToString();
        string nextLevelExp = PlayerDataManager.Instance.NextLevelExp.ToString();
        string value = currentExp + "/" + nextLevelExp;
        if (title.text != value)
        {
            SetText(value);
            AnimateProgressBar();
        }
    }

    private void AnimateProgressBar()
    {
        float currentExp = PlayerDataManager.Instance.PlayerExperience;
        float prevLevelExp = PlayerDataManager.Instance.PrevLevelExp;
        float nextLevelExp = PlayerDataManager.Instance.NextLevelExp;
        float endValue = (currentExp - prevLevelExp) / (nextLevelExp - prevLevelExp);
        var sequence = DOTween.Sequence();
        sequence.Join(progressBar.DOValue(endValue, 1f).SetEase(Ease.InOutQuad));
        sequence.Append(title.transform.DOPunchScale(new Vector2(0.25f, -0.1f), 0.5f).SetEase(Ease.InOutQuad))
            .OnComplete(() => title.transform.localScale = Vector2.one);
    }
}
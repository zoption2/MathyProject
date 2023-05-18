using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;
using Mathy.Services;
using Mathy;

public class ExperiencePanel : HeaderBar
{
    private const string kLastShowedKeyFormat = "{0}LastShowedMainMenu";

    [Inject] private IDataService _dataService;
    #region Fields

    [SerializeField] Slider progressBar;

    #endregion

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }

    protected override async UniTask AsyncUpdateText()
    {
        int experience = await _dataService.PlayerData.Progress.GetPlayerExperienceAsync();
        SetText(experience);
    }

    public override async void SetText(int newValue)
    {
        string currentExp = newValue.ToString();
        int rank = await _dataService.PlayerData.Progress.GetRankAsynk();
        string nextLevelExp = PointsHelper.GetMaxExperienceOfRank(rank).ToString();
        string value = currentExp + "/" + nextLevelExp;
        if (title.text != value)
        {
            SetText(value);
            AnimateProgressBar(newValue, rank);
        }
    }

    private async void AnimateProgressBar(int currentExp, int rank)
    {
        var lastShowedExpKey = string.Format(kLastShowedKeyFormat, KeyValueIntegerKeys.Experience);
        int prevLevelExp = await _dataService.KeyValueStorage.GetIntValue(lastShowedExpKey);
        await _dataService.KeyValueStorage.SaveIntValue(lastShowedExpKey, currentExp);

        var lastShowedRankKey = string.Format(kLastShowedKeyFormat, KeyValueIntegerKeys.PlayerRank);
        var lastShowedRank = await _dataService.KeyValueStorage.GetIntValue(lastShowedRankKey);
        await _dataService.KeyValueStorage.SaveIntValue(lastShowedRankKey, rank);

        List<int> ranksToProcess = new List<int>();
        for (int i = lastShowedRank; i < rank; i++)
        {
            ranksToProcess.Add(i);
        }
        ranksToProcess.Add(rank);

        int currentProgress = prevLevelExp;
        for (int i = 0, j = ranksToProcess.Count; i < j; i++)
        {
            var selectedRank = ranksToProcess[i];
            var rankExpLimit = PointsHelper.GetMaxExperienceOfRank(selectedRank);
            progressBar.maxValue = rankExpLimit;
            progressBar.value = currentProgress;
            var targetProgress = currentExp > rankExpLimit
                ? rankExpLimit
                : currentExp;

            await AnimateProgress(currentProgress, targetProgress);
            currentProgress = 0;
        }
    }

    private async UniTask AnimateProgress(int startValue, int endValue)
    {
        var tcs = new UniTaskCompletionSource<bool>();

        progressBar.value = startValue;
        var sequence = DOTween.Sequence();
        sequence.Join(progressBar.DOValue(endValue, 1f).SetEase(Ease.InOutQuad).SetId(transform));
        sequence.Append(title.transform.DOPunchScale(new Vector2(0.25f, -0.1f), 0.5f).SetEase(Ease.InOutQuad).SetId(transform))
            .OnComplete(() =>
            {
                title.transform.localScale = Vector2.one;
                tcs.TrySetResult(true);
            });

        await tcs.Task;
    }
}
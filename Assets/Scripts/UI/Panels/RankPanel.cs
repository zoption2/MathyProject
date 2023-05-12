using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Mathy.Services;
using DG.Tweening;
using Mathy;
using Cysharp.Threading.Tasks;

public class RankPanel : MonoBehaviour
{
    private const string kLastShowedKeyFormat = "{0}LastShowedMainMenu";
    [Inject] private IPlayerDataService _playerDataService;
    [Inject] private IDataService _dataService;
    [Header("Components:")]

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Slider progressBar;
    [SerializeField] protected TMP_Text progressText;
    private RectTransform rectTransform;


    private void Awake()
    {
        Initialization();
    }

    private void OnEnable()
    {
        UpdateDisplayStyle();
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }

    private void Initialization()
    {
        rectTransform = icon.GetComponent<RectTransform>();
    }

    public async void UpdateDisplayStyle()
    {
        //int index = await _playerDataService.Progress.GetRankAsynk();
        //rankText.text = index.ToString();
        await CompletedMethod();
    }

    private async UniTask CompletedMethod()
    {
        int experience = await _playerDataService.Progress.GetPlayerExperienceAsync();
        var lastShowedExpKey = string.Format(kLastShowedKeyFormat, KeyValueIntegerKeys.Experience);
        int lastShowedExp = await _dataService.KeyValueStorage.GetIntValue(lastShowedExpKey);

        int rank = await _playerDataService.Progress.GetRankAsynk();

        if (experience == lastShowedExp)
        {
            ShowComplexProgress(rank, experience);
            int previousRank = 0;
            if (rank > 0)
            {
                previousRank = rank - 1;
                var previousLimit = PointsHelper.GetMaxExperienceOfRank(previousRank);
                progressBar.minValue = previousLimit;
            }
            var rankExpLimit = PointsHelper.GetMaxExperienceOfRank(rank);
            progressBar.maxValue = rankExpLimit;
            progressBar.value = experience;
            return;
        }

        await _dataService.KeyValueStorage.SaveIntValue(lastShowedExpKey, experience);

        var lastShowedRankKey = string.Format(kLastShowedKeyFormat, KeyValueIntegerKeys.PlayerRank);
        var lastShowedRank = await _dataService.KeyValueStorage.GetIntValue(lastShowedRankKey);
        await _dataService.KeyValueStorage.SaveIntValue(lastShowedRankKey, rank);

        AnimateProgressBar(experience, lastShowedExp, rank, lastShowedRank);
    }

    private void SetRankText(string rank)
    {
        rankText.text = rank;
    }

    private void SetProgressText(string progress)
    {
        progressText.text = progress;
    }

    private void ShowComplexProgress(int rank, int exp)
    {
        var rankText = rank.ToString();
        SetRankText(rankText);

        string nextLevelExp = PointsHelper.GetMaxExperienceOfRank(rank).ToString();
        string progressValue = string.Format("{0}/{1}", exp, nextLevelExp);
        SetProgressText(progressValue);
    }

    //private async UniTask UpdateProgressAsync()
    //{
    //    //await UniTask.WaitUntil(() => PlayerDataManager.Instance != null);
    //    //int experience = PlayerDataManager.Instance.PlayerExperience;
    //    int experience = await _playerDataService.Progress.GetPlayerExperienceAsync();
    //    UpdateProgress(experience);
    //}

    //private async void UpdateProgress(int newValue)
    //{
    //    string currentExp = newValue.ToString();
    //    //string nextLevelExp = PlayerDataManager.Instance.NextLevelExp.ToString();
    //    int rank = await _playerDataService.Progress.GetRankAsynk();
    //    //string nextLevelExp = PlayerDataManager.Instance.NextLevelExp.ToString();
    //    string nextLevelExp = PointsHelper.GetMaxExperienceOfRank(rank).ToString();
    //    string value = currentExp + "/" + nextLevelExp;
    //    if (progressText.text != value)
    //    {
    //        progressText.text = value;
    //        AnimateProgressBar(newValue, rank);
    //    }
    //}

    private async void AnimateProgressBar(int currentExp, int lastExp, int currentRank, int lastRank)
    {
        ShowComplexProgress(lastRank, currentExp);

        List<int> ranksToProcess = new List<int>();
        for (int i = lastRank; i < currentRank; i++)
        {
            ranksToProcess.Add(i);
        }
        ranksToProcess.Add(currentRank);

        int currentProgress = lastExp;
        for (int i = 0, j = ranksToProcess.Count; i < j; i++)
        {
            var rank = ranksToProcess[i];
            ShowComplexProgress(rank, currentExp);
            var selectedRank = ranksToProcess[i];
            var rankExpLimit = PointsHelper.GetMaxExperienceOfRank(selectedRank);
            if (rank > 0)
            {
                var previousRank = rank - 1;
                var previousLimit = PointsHelper.GetMaxExperienceOfRank(previousRank);
                progressBar.minValue = previousLimit;
            }
            progressBar.maxValue = rankExpLimit;
            progressBar.value = currentProgress;
            var targetProgress = currentExp > rankExpLimit
                ? rankExpLimit
                : currentExp;

            await AnimateProgress(currentProgress, targetProgress);
            currentProgress = 0;
        }
    }

    //private async void AnimateProgressBar(int currentExp, int rank)
    //{
    //    var lastShowedExpKey = string.Format(kLastShowedKeyFormat, KeyValueIntegerKeys.Experience);
    //    int prevLevelExp = await _dataService.KeyValueStorage.GetIntValue(lastShowedExpKey);
    //    await _dataService.KeyValueStorage.SaveIntValue(lastShowedExpKey, currentExp);
    //    //float currentExp = PlayerDataManager.Instance.PlayerExperience;
    //    //float prevLevelExp = PlayerDataManager.Instance.PrevLevelExp;

    //    var lastShowedRankKey = string.Format(kLastShowedKeyFormat, KeyValueIntegerKeys.PlayerRank);
    //    var lastShowedRank = await _dataService.KeyValueStorage.GetIntValue(lastShowedRankKey);
    //    await _dataService.KeyValueStorage.SaveIntValue(lastShowedRankKey, rank);

    //    List<int> ranksToProcess = new List<int>();
    //    for (int i = lastShowedRank; i < rank; i++)
    //    {
    //        ranksToProcess.Add(i);
    //    }
    //    ranksToProcess.Add(rank);

    //    int currentProgress = prevLevelExp;
    //    for (int i = 0, j = ranksToProcess.Count; i < j; i++)
    //    {
    //        var selectedRank = ranksToProcess[i];
    //        var rankExpLimit = PointsHelper.GetMaxExperienceOfRank(selectedRank);
    //        progressBar.maxValue = rankExpLimit;
    //        progressBar.value = currentProgress;
    //        var targetProgress = currentExp > rankExpLimit
    //            ? rankExpLimit
    //            : currentExp;

    //        await AnimateProgress(currentProgress, targetProgress);
    //        currentProgress = 0;
    //    }
    //}

    private async UniTask AnimateProgress(int startValue, int endValue)
    {
        var tcs = new UniTaskCompletionSource<bool>();

        progressBar.value = startValue;
        var sequence = DOTween.Sequence();
        sequence.Join(progressBar.DOValue(endValue, 1f).SetEase(Ease.InOutQuad).SetId(transform));
        sequence.Append(progressText.transform.DOPunchScale(new Vector2(0.25f, -0.1f), 0.5f).SetEase(Ease.InOutQuad).SetId(transform))
            .OnComplete(() =>
            {
                progressText.transform.localScale = Vector2.one;
                tcs.TrySetResult(true);
            });

        await tcs.Task;
    }
}

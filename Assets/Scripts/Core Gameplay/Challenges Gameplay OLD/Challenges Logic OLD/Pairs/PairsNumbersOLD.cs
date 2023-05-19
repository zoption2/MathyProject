using System.Collections;
using System.Collections.Generic;
using Fallencake.Tools;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Mathy.Core;
using Mathy.Data;
using System;
using Mathy.Services;
using Zenject;

public class PairsNumbersOLD : ChallengeOLD
{
    #region Fields

    public override TaskType TaskType { get; } = TaskType.PairsNumbers;
    private List<AnswerVariantOLD> selectedVariants = new List<AnswerVariantOLD>();
    private const string bestTimeKey = "PairsNumbersBestTime";

    #endregion
    [Inject] private IDataService dataService;

    public override void RunTask(bool isPractice)
    {
        _isPractice = isPractice;
        Initialization();
        UpdateDisplayStyle();
        GenerateElements();
        GenerateVariants();
        SubscribeOnVariants();
    }

    protected override void UpdateDisplayStyle()
    {
        ShowTaskPanel(false);
        ShowTimer(true, RightPanel);
        ShowLivesPanel(false);
        ShowBGImage(false);
        ShowVariantsPanel(true, 12, new Vector2(10, 10));
    }

    protected override void GenerateElements()
    {
        ClearPanels();
    }

    private async void CorrectVariant(List<AnswerVariantOLD> selectedVariants)
    {
        VibrationManager.Instance.TapPeekVibrate();
        var tasks = new List<System.Threading.Tasks.Task>();

        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetInteractable(false);
            tasks.Add(variant.selectionTask);
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);

        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetAsCorrect(0.5f, true);
            variants.Remove(variant);
        }
        AudioManager.Instance.CorrectVariantSound();
        ResultCheck();
    }

    private async void WrongVariant(List<AnswerVariantOLD> selectedVariants)
    {
        VibrationManager.Instance.TapNopeVibrate();
        var tasks = new List<System.Threading.Tasks.Task>();
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetInteractable(false);
            tasks.Add(variant.selectionTask);
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);
        AudioManager.Instance.WrongVariantSound();
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetAsWrong(0.5f);
        }

        tasks = new List<System.Threading.Tasks.Task>();
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            tasks.Add(variant.tweenTask);
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SelectTween(false);
            variant.SetInteractable(true);
        }
    }

    private void ResultCheck()
    {
        if (variants.Count == 0)
        {
            ChallengeData data = new ChallengeData();
            data.Mode = _isPractice ? TaskMode.Practic : TaskMode.Challenge;
            data.Seed = 0;
            data.TaskType = TaskType.PairsNumbers;
            data.Duration = TimeSpan.FromSeconds(StopTimer(true));
            data.MaxNumber = 20;
            data.IsDone = true;
            data.CorrectRate = this.GetCorrectRate();

            var modeData = new DailyModeData();
            modeData.Mode = _isPractice ? TaskMode.Practic : TaskMode.Challenge;
            modeData.Date = DateTime.UtcNow;
            modeData.IsComplete = true;
            modeData.PlayedCount = 1;
            modeData.CorrectAnswers = 1;
            modeData.CorrectRate = 100;
            var duration = TimeSpan.FromSeconds(StopTimer(true));
            modeData.Duration = duration.TotalMilliseconds;
            modeData.TotalTasks = 1;
            modeData.TasksIds.Add(0);

            dataService.TaskData.UpdateDailyMode(modeData);

            //ChallengesManager.Instance.SaveTaskData(data);
            ChallengesManager.Instance.ShowResult(true);
            //Debug.LogError("SAVE HERE!!");
        }
    }

    public override float GetCorrectRate()
    {
        float correctRate = 100f;
        return correctRate;
    }

    protected override void SetVariantsValues()
    {
        var numberList = Enumerable.Range(1, variants.Count / 2).ToList();
        List<int> variantsValues = numberList.Concat(numberList).ToList();
        variantsValues.FCShuffle();

        for (int i = 0; i < variants.Count; i++)
        {
            variants[i].SetText(variantsValues[i].ToString());
        }

        StartCoroutine(HideAllVariants());
    }

    private IEnumerator HideAllVariants()
    {
        foreach (AnswerVariantOLD variant in variants)
        {
            variant.SelectTween(true);
            variant.SetInteractable(false);
        }

        yield return new WaitForSeconds(5f);

        StartTimer();

        foreach (AnswerVariantOLD variant in variants)
        {
            variant.SelectTween(false);
            variant.SetInteractable(true);
        }
    }

    private void StartTimer()
    {
        timerPanel.bestTimeKey = bestTimeKey;
        timerPanel.StartTimer();
    }

    private float StopTimer(bool isComplete)
    {
        return timerPanel.StopTimer(isComplete);
    }

    private void SubscribeOnVariants()
    {
        foreach (AnswerVariantOLD variant in variants)
        {
            variant.GetComponent<Button>().onClick.AddListener(delegate { SelectVariant(variant); });
        }
    }

    private void SelectVariant(AnswerVariantOLD selectedVariant)
    {
        selectedVariant.SelectTween(true);
        if (selectedVariants.Count == 1)
        {
            selectedVariants.Add(selectedVariant);
            selectedVariant.SetInteractable(false);

            if (selectedVariants[0].GetValueInt() == selectedVariants[1].GetValueInt())
            {
                CorrectVariant(selectedVariants);
            }
            else
            {
                WrongVariant(selectedVariants);
            }

            selectedVariants = new List<AnswerVariantOLD>();
        }
        else
        {
            selectedVariants.Add(selectedVariant);
            selectedVariant.SetInteractable(false);
        }
    }
}

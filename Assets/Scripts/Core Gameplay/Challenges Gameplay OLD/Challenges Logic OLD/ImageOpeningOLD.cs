using System.Collections.Generic;
using Fallencake.Tools;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Core;
using Mathy.Data;
using System;
using Mathy.Services;
using Zenject;

public class ImageOpeningOLD : ChallengeOLD
{
    #region Fields

    public override TaskType TaskType { get; } = TaskType.ImageOpening;
    private const string bestTimeKey = "ImageOpeningBestTime";

    #endregion
    [Inject] private IDataService dataService;

    public override void RunTask()
    {
        Initialization();
        UpdateDisplayStyle();
        CheckDifficultyMode();
        GenerateElements();
        CalculateExpression();
        GenerateVariants();
        //UpdateTextStyle();
        SubscribeOnVariants();
        SetLives();
        StartTimer();
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

    private void CheckDifficultyMode()
    {
        /*
        switch (GameManager.Instance.DifficultyMode)
        {
            case TaskDifficulty.Beginner:
                maxLives = 3;
                break;
            case TaskDifficulty.Expert:
                maxLives = 5;
                break;
            default:
                goto case 0;
        }
        */
    }

    private void CorrectVariant(AnswerVariantOLD variant)
    {
        variant.SetAsCorrect(0.5f, true);
        variants.Remove(variant);
        ResultCheck();
        AudioManager.Instance.CorrectVariantSound();
        VibrationManager.Instance.TapPeekVibrate();
    }

    private void WrongVariant(AnswerVariantOLD variant)
    {
        variant.SetAsWrong(1.5f);
        LivesPanel.SetDamage(1);
        if (LivesPanel.Lives <= 0)
        {
            StopTimer(false);
            ChallengesManager.Instance.ShowResult(true);
        }
        AudioManager.Instance.WrongVariantSound();
        VibrationManager.Instance.TapNopeVibrate();
    }

    private void GenirateNextTask()
    {
        GenerateChalengeElements();
        CalculateChalengeExpression();
        SetVariantsValues();
    }

    private void ResultCheck()
    {
        ChallengeData data = new ChallengeData();
        data.Mode = TaskMode.Challenge;
        data.Seed = 0;
        data.TaskType = TaskType.ImageOpening;
        data.IsDone = true;
        data.MaxNumber = 20;
        data.CorrectRate = this.GetCorrectRate();

        var modeData = new DailyModeData();
        modeData.Mode = TaskMode.Challenge;
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

        if (variants.Count > 0)
        {
            GenirateNextTask();
        }
        else
        {
            data.Duration = TimeSpan.FromSeconds(StopTimer(true));
            //ChallengesManager.Instance.SaveTaskData(data);
            ChallengesManager.Instance.ShowResult(true);
            //Debug.LogError("SAVE HERE!!");
        }
    }

    private void GenerateChalengeElements()
    {
        ClearElements();
        InstantiateElements();
        SetElementsValues();
        SetOperatorsValues();
        SetAnswer();
    }

    private void ClearElements()
    {
        elements = new List<TaskElementOLD>();
        operators = new List<MathOperatorOLD>();
        TaskPanel.DestroyChildren();
    }

    private void CalculateChalengeExpression()
    {
        string expression = Expression();
        Answer = (int)Evaluate(expression);

        if (Answer > MaxNumber || (onlyPositive && Answer < 0))
        {
            GenerateChalengeElements();
            CalculateChalengeExpression();
        }
    }
    protected override void SetVariantsValues()
    {
        List<int> variantsValues = new List<int>() { Answer };
        int variantIndex = UnityEngine.Random.Range(0, variants.Count);
        var randomVariant = variants[variantIndex];
        randomVariant.SetText(Answer.ToString());
        correctVariant = randomVariant;

        foreach (AnswerVariantOLD variant in variants)
        {
            if (variant != correctVariant)
            {
                int randomInt = variantsValues.UniqueRandom(0, MaxNumber);
                variantsValues.Add(randomInt);
                variant.SetText(randomInt.ToString());
            }
        }
    }

    private void SubscribeOnVariants()
    {
        foreach (AnswerVariantOLD variant in variants)
        {
            //int index = variants.FindIndex(v => v == variant);
            variant.GetComponent<Button>().onClick.AddListener(delegate { SelectVariant(variant); });
        }
    }

    private void SelectVariant(AnswerVariantOLD selectedVariant)
    {
        if (selectedVariant.GetValueInt() == Answer)
        {
            CorrectVariant(selectedVariant);
        }
        else
        {
            WrongVariant(selectedVariant);
        }
    }
}
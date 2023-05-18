using Cysharp.Threading.Tasks;
using Mathy.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class StatisticsPanel : MonoBehaviour
{
    [Inject] private IDataService _dataService;
    #region Fields

    [Header("DAILY MODE BARS:")]
    [SerializeField] private TweenedProgressBar sModeBar;
    [SerializeField] private TweenedProgressBar mModeBar;
    [SerializeField] private TweenedProgressBar lModeBar;

    [Header("SKILL BARS:")]
    [SerializeField] private TweenedProgressBar countBar;
    [SerializeField] private TweenedProgressBar addSubBar;
    [SerializeField] private TweenedProgressBar comparisonBar;

    [Header("DAILY MODE TEXT:")]
    [SerializeField] private TMP_Text sModeLabel;
    [SerializeField] private TMP_Text mModeLabel;
    [SerializeField] private TMP_Text lModeLabel;

    [Header("SKILLS TEXT:")]
    [SerializeField] private TMP_Text countLabel;
    [SerializeField] private TMP_Text addSubLabel;
    [SerializeField] private TMP_Text comparisonLabel;

    [Header("AWARDS TEXT:")]
    [SerializeField] private TMP_Text goldenLabel;
    [SerializeField] private TMP_Text silverLabel;
    [SerializeField] private TMP_Text bronzeLabel;
    [SerializeField] private TMP_Text challengeModeLabel;

    [Header("COMPONENTS:")]
    [SerializeField] private GameObject loadingIndicator;

    private int sModeCorrectRate;
    private int mModeCorrectRate;
    private int lModeCorrectRate;

    private int countCorrectRate;
    private int addSubCorrectRate;
    private int comparisonCorrectRate;

    private PlayerDataManager playerData { get => PlayerDataManager.Instance; }

    #endregion
    private void OnEnable()
    {
        _ = UpdateStatistics();
    }

    private async UniTask UpdateStatistics()
    {
		loadingIndicator.SetActive(true);

		UpdateAwardsText();
		
        sModeCorrectRate = await playerData.GetPercentageOfCompletedTaskOfMode(TaskMode.Small);
        mModeCorrectRate = await playerData.GetPercentageOfCompletedTaskOfMode(TaskMode.Medium);
        lModeCorrectRate = await playerData.GetPercentageOfCompletedTaskOfMode(TaskMode.Large);

		UpdateDailyModesText();
		UpdateModeBars();
		
        countCorrectRate = await playerData.GetCorrectRateOfPercentageTaskType(TaskType.MissingNumber);
        addSubCorrectRate = await playerData.GetCorrectRateOfPercentageTaskType(TaskType.Addition);
        comparisonCorrectRate = await playerData.GetCorrectRateOfPercentageTaskType(TaskType.Comparison);

        UpdateSkillsText();
        UpdateSkillBars();

        loadingIndicator.SetActive(false);
    }

    private void UpdateSkillBars()
    {
        countBar.Value = countCorrectRate;
        addSubBar.Value = addSubCorrectRate;
        comparisonBar.Value = comparisonCorrectRate;
    }

    private void UpdateModeBars()
    {
        sModeBar.Value = sModeCorrectRate;
        mModeBar.Value = mModeCorrectRate;
        lModeBar.Value = lModeCorrectRate;
    }

    private void UpdateDailyModesText()
    {
        sModeLabel.text = CalculateGrade(sModeCorrectRate);
        mModeLabel.text = CalculateGrade(mModeCorrectRate);
        lModeLabel.text = CalculateGrade(lModeCorrectRate);
    }

    private void UpdateSkillsText()
    {
        countLabel.text = CalculateGrade(countCorrectRate);
        addSubLabel.text = CalculateGrade(addSubCorrectRate);
        comparisonLabel.text = CalculateGrade(comparisonCorrectRate);
    }

    private async void UpdateAwardsText()
    {
        var goldCount = await _dataService.PlayerData.Achievements.GetGoldMedals();
        var silverCount = await _dataService.PlayerData.Achievements.GetSilverMedals();
        var bronzeCount = await _dataService.PlayerData.Achievements.GetBronzeMedals();
        var cupsCount = await _dataService.PlayerData.Achievements.GetChallengeCups();
        goldenLabel.text = goldCount.ToString();
        silverLabel.text = silverCount.ToString();
        bronzeLabel.text = bronzeCount.ToString();
        challengeModeLabel.text = cupsCount.ToString();
    }

    private string CalculateGrade(int correctRate)
    {
        string grade = "";
        if (correctRate <= 59)
        {
            grade = "F";
        }
        else if (correctRate > 59 && correctRate <= 69)
        {
            grade = "D";
        }
        else if (correctRate > 69 && correctRate <= 79)
        {
            grade = "C";
        }
        else if (correctRate > 79 && correctRate <= 89)
        {
            grade = "B";
        }
        else if (correctRate > 89)
        {
            grade = "A";
        }
        return grade + " (" + correctRate.ToString() + "%)";
    }
}

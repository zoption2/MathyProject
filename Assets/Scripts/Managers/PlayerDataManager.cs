using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using Mathy.Data;
using System;
using Mathy.Core.Tasks;

public class PlayerDataManager : StaticInstance<PlayerDataManager>
{
    #region FIELDS

    [Header("GUI Components:")]

    [SerializeField] GradePanel gradePanel;
    [SerializeField] RankPanel rankPanel;

    public int PlayerStars { get; private set; }
    public int PlayerRank { get; private set; }
    public int PlayerExperience { get; private set; }
    public int PrevLevelExp { get; private set; } = 0;
    public int NextLevelExp { get; private set; } = 0;

    [SerializeField] List<int> levelUpValues = new List<int>
    { 500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000,
    10000, 11000, 12000, 13000, 14000, 15000, 16000, 17000, 18000, 19000};

    public static UnityEvent OnPlayerStatsUpdated = new UnityEvent();

    public int ChallengesDoneAmount
    {
        get => PlayerPrefs.GetInt(challengesKey);
        set => PlayerPrefs.SetInt(challengesKey, value);
    }
    public int GoldenAmount
    {
        get => PlayerPrefs.GetInt(goldenKey);
        set => PlayerPrefs.SetInt(goldenKey, value);
    }
    public int SilverAmount
    {
        get => PlayerPrefs.GetInt(silverKey);
        set => PlayerPrefs.SetInt(silverKey, value);
    }
    public int BronzeAmount
    {
        get => PlayerPrefs.GetInt(bronzeKey);
        set => PlayerPrefs.SetInt(bronzeKey, value);
    }

    const string starsKey = "PlayerStars";
    const string expKey = "PlayerExperience";
    const string challengesKey = "ChallengesDoneAmount";
    const string goldenKey = "GoldenAmount";
    const string silverKey = "SilverAmount";
    const string bronzeKey = "BronzeAmount";

    #endregion

    #region SAVE & LOAD

    private static void SavePlayerStats(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    private static void SavePlayerStats(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public void LoadPlayerData()
    {
        PlayerStars = PlayerPrefs.GetInt(starsKey, 0);
        PlayerExperience = PlayerPrefs.GetInt(expKey, 0);
        UpdateNextLevelUpValue();
        OnPlayerStatsUpdated.Invoke();
    }

    public void ResetToDefault()
    {
        PlayerExperience = 0;
        SavePlayerStats(expKey, PlayerExperience);
        UpdateNextLevelUpValue();
        OnPlayerStatsUpdated.Invoke();

        ChallengesDoneAmount = 0;
        GoldenAmount = 0;
        SilverAmount = 0;
        BronzeAmount = 0;
    }

    private void UpdateNextLevelUpValue()
    {
        for (int i = 0; i < levelUpValues.Count; i++)
        {
            if (levelUpValues[i] > PlayerExperience)
            {
                PrevLevelExp = i > 0 ? levelUpValues[i - 1] : 0;
                NextLevelExp = levelUpValues[i];
                PlayerRank = i;
                rankPanel.UpdateDisplayStyle();
                break;
            }
        }
    }
    #endregion

    #region ADD METHODS

    public void AddExperience(int value)
    {
        PlayerExperience += value;
        SavePlayerStats(expKey, PlayerExperience);
        OnPlayerStatsUpdated.Invoke();
        UpdateNextLevelUpValue();
    }
    public void DoubleExperience(int value)
    {
        PlayerExperience += value;
        SavePlayerStats(expKey, PlayerExperience);
        OnPlayerStatsUpdated.Invoke();
        UpdateNextLevelUpValue();
    }

    public void AddStars(int value)
    {
        PlayerStars += value;
        SavePlayerStats(starsKey, PlayerStars);
        OnPlayerStatsUpdated.Invoke();
    }

    #endregion

	
    public void AllModesDoneReward()
    {
        GoldenAmount++;
    }
	
    public async System.Threading.Tasks.Task<int> GetPercentageOfCompletedTaskOfMode(TaskMode mode)
    {
        double rate = await DataManager.Instance.GetCorrectRateOfMode(mode);
        return (int)(rate * 100);
    }

    public async System.Threading.Tasks.Task<int> GetCorrectRateOfPercentageTaskType(TaskType taskType)
    {
        double rate = await DataManager.Instance.GetCorrectRateOfTaskType(taskType);
        return (int)(rate * 100);
    }
}

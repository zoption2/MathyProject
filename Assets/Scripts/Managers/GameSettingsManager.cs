using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Data;
using Mathy.Core;
using UnityEngine.SceneManagement;
using Mathy.Core.Tasks;
using Randoms.DailyReward;
using Zenject;
using Mathy.Services;

public class GameSettingsManager : StaticInstance<GameSettingsManager>
{
    #region FIELDS
    [Inject] private IDataService dataService;

    [Header("GUI Components:")]

    [SerializeField] SettingsPanel settingsPanel;

    public static UnityEvent<bool> OnSoundsEnabled = new UnityEvent<bool>();
    public static UnityEvent<bool> OnMusicEnabled = new UnityEvent<bool>();
    public static UnityEvent<bool> OnVibrationEnabled = new UnityEvent<bool>();

    const string musicKey = "isMusicEnabled";
    const string soundsKey = "isSoundsEnabled";
    const string vibrationKey = "isVibrationEnabled";
    const string languageKey = "en";
    const string maxNumberKey = "GameModeMaxNumber";

    public bool isSoundsEnabled { get; private set; } = true;
    public bool isMusicEnabled { get; private set; } = true;
    public bool isVibrationEnabled { get; private set; } = true;

    private int gameModeMaxNumber = 20;
    public int MaxNumber
    {
        get => gameModeMaxNumber;
        set
        {
            gameModeMaxNumber = value;
            PlayerPrefs.SetInt(maxNumberKey, value);
            //_ = TaskManager.Instance.GenerateAllTasks();
        }
    }

    #endregion

    #region SAVE & LOAD

    private static void SaveSettings(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value.ToInt());
    }

    private static void SaveSettings(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public void LoadSettings()
    {
        Application.targetFrameRate = 60;

        gameModeMaxNumber = PlayerPrefs.GetInt(maxNumberKey, 20);

        isMusicEnabled = PlayerPrefs.GetInt(musicKey, 1).ToBool();
        isSoundsEnabled = PlayerPrefs.GetInt(soundsKey, 1).ToBool();
        isVibrationEnabled = PlayerPrefs.GetInt(vibrationKey, 1).ToBool();
        string language = PlayerPrefs.GetString(languageKey, "en");

        _ = SetLoadedSettings(language);
    }

    private async UniTask SetLoadedSettings(string language)
    {
        settingsPanel.soundsToggle.isOn = isSoundsEnabled;
        settingsPanel.musicToggle.isOn = isMusicEnabled;
        settingsPanel.vibrationToggle.isOn = isVibrationEnabled;

        settingsPanel.maxNumberSlider.value = gameModeMaxNumber/10;

        await UniTask.WaitUntil(() => LocalizationSettings.InitializationOperation.IsDone);
        LocalizationManager.Instance.SetLanguage(language);
    }

    #endregion

    #region SOUNDS METHODS

    public static void HandleSoundsEnabled(bool isEnabled)
    {
        OnSoundsEnabled.Invoke(isEnabled);
        SaveSettings(soundsKey, isEnabled);
    }

    public static void HandleSoundsEnabled(Toggle toggle)
    {
        bool isEnabled = toggle.isOn;
        OnSoundsEnabled.Invoke(isEnabled);
        SaveSettings(soundsKey, isEnabled);
    }

    #endregion

    #region MUSIC METHODS

    public static void HandleMusicEnabled(bool isEnabled)
    {
        OnMusicEnabled.Invoke(isEnabled);
        SaveSettings(musicKey, isEnabled);
    }

    public static void HandleMusicEnabled(Toggle toggle)
    {
        bool isEnabled = toggle.isOn;
        OnMusicEnabled.Invoke(isEnabled);
        SaveSettings(musicKey, isEnabled);
    }

    #endregion

    #region LANGUAGE METHODS

    public void SaveLanguageSettings(string newValue)
    {
        SaveSettings(languageKey, newValue);
    }

    #endregion

    public async void ResetProgress()
    {
        //DailyRewardManager.Instance.ResetToDefault();
        //DataManager.Instance.WasTodayAwardGot = false;
        //DataManager.Instance.ResetAllBestScores();
        //DailyStatusPanel.Instance.AllModesDone = false;
        //await DataManager.Instance.ResetSaveFile();
        //GameManager.Instance.ChangeState(GameState.MainMenu);
        //CalendarManager.Instance.ResetToDefault();
        //PlayerDataManager.Instance.ResetToDefault();
        //IAPManager.Instance.ResetToDefault();
        ////Here in the reset process we need to show a modal window to the user and ask him if he really wants to delete all his progress
        //await SceneManager.LoadSceneAsync("LoadingScreen");

        //DailyStatusPanel.Instance.AllModesDone = false;
        PlayerPrefs.DeleteAll();
        await dataService.ResetProgress();
        GameManager.Instance.ChangeState(GameState.MainMenu);
        CalendarManager.Instance.ResetToDefault();
        //PlayerDataManager.Instance.ResetToDefault();
        IAPManager.Instance.ResetToDefault();
        //Here in the reset process we need to show a modal window to the user and ask him if he really wants to delete all his progress
        await SceneManager.LoadSceneAsync("LoadingScreen");
    }



    public static void HandleVibrationEnabled(Toggle toggle)
    {
        bool isEnabled = toggle.isOn;
        OnVibrationEnabled.Invoke(isEnabled);
        Instance.isVibrationEnabled = isEnabled;
        SaveSettings(vibrationKey, isEnabled);
    }
}

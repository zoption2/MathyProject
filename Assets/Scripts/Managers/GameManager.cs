using Mathy.Core.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mathy.Core
{
    public class GameManager : StaticInstance<GameManager>
    {
        #region FIELDS

        [Header("GUI Components:")]

        [SerializeField] HeaderPanel header;
        [SerializeField] DatePanel datePanel;
        [SerializeField] PlayButtonPanel playPanel;
        [SerializeField] ChallengeCupButton challengeButton;
        [SerializeField] List<PopupPanel> popupPanels;

        [SerializeField] private PopupPanel skillPlanWindow;

        public bool IsFirstLaunch 
        {
            get
            {
                int IsFirst = PlayerPrefs.GetInt("IsFirstLaunch");
                //Debug.LogError("IsFirstLaunch: " + IsFirst);
                if (IsFirst == 0)
                {
                    PlayerPrefs.SetInt("IsFirstLaunch", 1);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static event Action<GameState> OnBeforeStateChanged;
        public static event Action<GameState> OnAfterStateChanged;
        public static UnityEvent<int> OnDifficultyModeChanged = new UnityEvent<int>();
        public static UnityEvent OnGeneratingTasks = new UnityEvent();

        public GameState State { get; private set; }

        #endregion

        private void Start()
        {
            ChangeState(GameState.Starting);

            if (IsFirstLaunch)
            {
                skillPlanWindow.gameObject.SetActive(true);
            }
        }

        public void ChangeState(GameState newState)
        {
            OnBeforeStateChanged?.Invoke(newState);

            State = newState;
            switch (newState)
            {
                case GameState.Starting:
                    HandleStarting();
                    break;
                case GameState.GeneratingTasks:
                    HandleGeneratingTasks();
                    break;
                case GameState.MainMenu:
                    HandleMainMenu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            OnAfterStateChanged?.Invoke(newState);
        }

        private async void HandleStarting()
        {
            await PlayerDataManager.Instance.LoadPlayerData(); //Load from PlayerPrefs values of the player's experience and stars
            GameSettingsManager.Instance.LoadSettings();
            AudioManager.Instance.PlayMusic();
            InitializePanels();
            ChangeState(GameState.GeneratingTasks);
        }

        private void InitializePanels()
        {
            foreach (PopupPanel panel in popupPanels)
            {
                panel.Initialization();
            }
        }

        private void HandleGeneratingTasks()
        {
            //Temp off in new vers
            OnGeneratingTasks.Invoke();
            playPanel.UpdateModeIndicators();
        }

        private void HandleMainMenu()
        {
            // Do something after tasks generation
            AudioManager.Instance.PlayMusic();
            ScenesManager.Instance.MainMenuScene.IsActive = true;
            TaskManager.Instance.ResetToDefault();
            //Very bad thing, need to refactor, all of it should be handled by SceneManager
            ScenesManager.Instance.TaskScene.IsActive = false;
            ScenesManager.Instance.TaskResultScene.IsActive = false;
            ScenesManager.Instance.ChallengesScene.IsActive = false;

            challengeButton.UpdateChallengeStatus();
            playPanel.UpdateModeIndicators();
        }
    }

    //Future Todo: remake game states 
    [Serializable]
    public enum GameState
    {
        Starting = 0,
        GeneratingTasks = 1,
        PlayingDailyTask = 2,
        MainMenu = 3,
        DoneDaylyTask = 4
    }
}
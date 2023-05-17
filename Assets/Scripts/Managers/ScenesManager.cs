using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks;
using Mathy.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Mathy.Core
{
    public class ScenesManager : PersistentSingleton<ScenesManager>
    {
        [Inject] private IGameplayService gameplayService;
        public SceneGFXContainer MainMenuScene { get; set; }
        public SceneGFXContainer TaskScene { get; set; }
        public SceneGFXContainer ChallengesScene { get; set; }
        public SceneGFXContainer TaskResultScene { get; set; }

        protected override void Awake()
        {
            base.Awake();
        }

        public void DisableTaskScene()
        {
            TaskScene.IsActive = false;
        }

        public async UniTask SetGameplaySceneActive()
        {
            LoadingManager.Instance.OpenPanel();
            await UniTask.WaitUntil(() => LoadingManager.Instance.isLoadingScreenOpened);

            TaskScene.IsActive = true;
            TaskManager.Instance.IsPractice = false;
            TaskManager.Instance.EnableDifficultyMenu(false);
            MainMenuScene.IsActive = false;
        }

        public void CreateChallenge(ScriptableTask challenge, bool isPractice)
        {
            AudioSystem.Instance.FadeMusic(0, 1f, true);
            ChallengesScene.IsActive = true;
            ChallengesManager.Instance.IsPractice = isPractice;
            MainMenuScene.IsActive = false;
            ChallengesManager.Instance.CreateTaskList(challenge, 1);
        }
    }
}

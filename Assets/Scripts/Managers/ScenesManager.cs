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

        //private string[] AllScenes()
        //{
        //    int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        //    string[] scenes = new string[sceneCount];
        //    for (int i = 0; i < sceneCount; i++)
        //    {
        //        scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        //    }
        //    return scenes;
        //}

        //public void SceneUnloading(string sceneName)
        //{
        //    SceneManager.UnloadSceneAsync(sceneName);
        //}

        //Why all create tasks here, it's not scene manager responsibility

        //public void CreateTasks(ScriptableTask task, int amount, bool isChallenge)
        //{
        //    AudioSystem.Instance.FadeMusic(0, 1f, true);
        //    TaskScene.IsActive = true;
        //    MainMenuScene.IsActive = false;
        //}

        //public void CreateTasks(List<ScriptableTask> tasks, int amount, bool isChallenge)
        //{
        //    AudioSystem.Instance.FadeMusic(0, 1f, true);
        //    TaskScene.IsActive = true;
        //    MainMenuScene.IsActive = false;
        //}

        public async UniTask SetGameplaySceneActive()
        {
            LoadingManager.Instance.OpenPanel();
            await UniTask.WaitUntil(() => LoadingManager.Instance.isLoadingScreenOpened);

            TaskScene.IsActive = true;
            TaskManager.Instance.IsPractice = false;
            TaskManager.Instance.EnableDifficultyMenu(false);
            MainMenuScene.IsActive = false;
        }

        //public async UniTask CreateTasks(int amount)
        //{
        //    LoadingManager.Instance.OpenPanel();
        //    await UniTask.WaitUntil(() => LoadingManager.Instance.isLoadingScreenOpened);

        //    TaskScene.IsActive = true;
        //    TaskManager.Instance.IsPractice = false;
        //    TaskManager.Instance.EnableDifficultyMenu(false);
        //    MainMenuScene.IsActive = false;

        //    TaskMode mode;
        //    switch (amount)
        //    {
        //        case 10:
        //            mode = TaskMode.Small;
        //            break;
        //        case 20:
        //            mode = TaskMode.Medium;
        //            break;
        //        case 30:
        //            mode = TaskMode.Large;
        //            break;
        //        default:
        //            goto case 10;
        //    }

        //    TaskManager.Instance.CreateTaskList(mode);
        //}

        //public void StartTaskPractice(List<ScriptableTask> taskParams, int amount)
        //{
        //    //Debug.LogError("Tesdt: "+ taskParams.name);
        //    AudioSystem.Instance.FadeMusic(0, 1f, true);
        //    TaskScene.IsActive = true;
        //    TaskManager.Instance.IsPractice = true;
        //    TaskManager.Instance.EnableDifficultyMenu(false);
        //    MainMenuScene.IsActive = false;

        //    if (taskParams.Count == 1)
        //    {
        //        TaskManager.Instance.StartTaskPractice(taskParams[0], amount);
        //    }
        //    else
        //    {
        //        //Debug.LogError("Multiple tasks");
        //        TaskManager.Instance.StartTaskPractice(taskParams, amount);
        //    }

        //    TaskManager.Instance.ActivatePracticeMode(false);
        //}

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

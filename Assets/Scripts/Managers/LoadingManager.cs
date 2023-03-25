using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks;
using TMPro;

namespace Mathy.Core
{
    public class LoadingManager : StaticInstance<LoadingManager>
    {
        #region FIELDS

        [SerializeField] private Transform tweenedTransform;
        [SerializeField] private CanvasGroup background;
        [SerializeField] private RawImage bgImage;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Camera loadingCamera;
        [SerializeField] private Transform loadingSpin;
        public TMP_Text DebugInApp;

        public bool isLoadingScreenOpened { get; private set; }

        #endregion

        #region INITIALIZATION AND MONO

        protected override async void Awake()
        {
            base.Awake();
            await LoadAllScenesAsync();
        }

        #endregion

        #region LOADING

        private async UniTask LoadAllScenesAsync()
        {
            await UnloadAllScenesExcept("LoadingScreen");
            AsyncOperation asyncLoadMainMenu = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => asyncLoadMainMenu.isDone).ContinueWith(LoadGameplayScenes);
            await UniTask.WaitUntil(() => IAPManager.Instance != null).ContinueWith(() =>
            IAPManager.Instance.DebugText = DebugInApp);
        }

        private async UniTask UnloadAllScenesExcept(string sceneName)
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != sceneName)
                {
                    await UniTask.WaitUntil(() => !SceneManager.UnloadSceneAsync(scene).isDone);
                }
            }
        }

        private async UniTask LoadGameplayScenes()
        {
            _ = SceneManager.LoadSceneAsync("Task Gameplay New", LoadSceneMode.Additive);
            _ = SceneManager.LoadSceneAsync("Challenges Gameplay", LoadSceneMode.Additive);
            _ = SceneManager.LoadSceneAsync("Task Result", LoadSceneMode.Additive);

            await UniTask.WaitUntil(() => ScenesManager.Instance.TaskScene != null);
            ScenesManager.Instance.TaskScene.IsActive = false;
            await UniTask.WaitUntil(() => ScenesManager.Instance.ChallengesScene != null);
            ScenesManager.Instance.ChallengesScene.IsActive = false;
            await UniTask.WaitUntil(() => ScenesManager.Instance.TaskResultScene != null);
            ScenesManager.Instance.TaskResultScene.IsActive = false;

            //await UniTask.WaitUntil(() => GradeManager.Instance != null && 
            //GradeManager.Instance.IsInitialized);

            //await UniTask.WaitUntil(() => TaskManager.Instance != null);
            //await TaskManager.Instance.GenerateAllTasks();
            await UniTask.Delay(1000);
            ClosePanel();
        }

        #endregion

        #region TWEENS

        public void OpenPanel()
        {
            loadingCamera.gameObject.SetActive(true);
            loadingScreen.SetActive(true);

            background.alpha = 0;
            tweenedTransform.localScale = Vector3.zero;

            background.DOFade(1, 0.25f);
            tweenedTransform.DOScale(Vector3.one, 0.2f).OnComplete(() => OnTweenComplete(true));
        }

        public void ClosePanel()
        {
            background.DOFade(0, 0.25f);
            tweenedTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => OnTweenComplete(false));
        }

        private void OnTweenComplete(bool isOpened)
        {
            if (!isOpened)
            {
                loadingScreen.SetActive(false);
                loadingCamera.gameObject.SetActive(false);
            }
            isLoadingScreenOpened = isOpened;
        }

        #endregion
    }
}
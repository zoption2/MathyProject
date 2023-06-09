using System.Collections.Generic;
using UnityEngine;
using Mathy.Core;
using Mathy.UI;
using System;
using DG.Tweening;
using System.Linq;
using Zenject;
using Mathy.Services;
using Cysharp.Threading.Tasks;

public class TaskPracriceButton : TweenedButton
{
    #region FIELDS

    [Header("TASK SETTINGS:")]
    [SerializeField] private int tasksAmount;
    [SerializeField] private bool isChallenge = false;
    [SerializeField] private List<ScriptableTask> taskConfigs;
    [SerializeField] private List<Transform> panelPages;

    private bool isActive = true;
    public bool IsActive 
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            button.interactable = value;
            SetActiveTween();
        } 
    }

    #endregion
    [Inject] private IGameplayService gameplayService;

    private void OnEnable()
    {
        if (!isActive) IsActive = true;
        button.onClick.AddListener(OnPress);
    }

    private async void RunTask()
    {
        if (taskConfigs.Count > 0)
        {
            if (isChallenge)
            {
                ScenesManager.Instance.CreateChallenge(taskConfigs[0], true);
            }
            else
            {
                AudioSystem.Instance.FadeMusic(0, 1f, true);
                _ = ScenesManager.Instance.SetGameplaySceneActive();
                gameplayService.StartGame(TaskMode.Practic, taskConfigs);
                await UniTask.Delay(1000);
                LoadingManager.Instance.ClosePanel();
            }
        }
        else
        {
            throw new Exception("Need to specify any 'ScriptableTask' in the 'taskConfigs' list!");
        }
    }

    private void SetActiveTween()
    {
        Vector2 endSize = isActive ? Vector2.one : Vector2.zero;
        var sequence = DOTween.Sequence().
            SetEase(Ease.InOutBack).
            OnComplete(() => OnActivationComplete());
        sequence.Join(tweenTransform.DOScale(endSize, tweenDuration));
    }

    private void OnActivationComplete()
    {
        gameObject.SetActive(isActive);
    }

    protected override void OnTweenComplete()
    {
        base.OnTweenComplete();
        RunTask();
    }
}

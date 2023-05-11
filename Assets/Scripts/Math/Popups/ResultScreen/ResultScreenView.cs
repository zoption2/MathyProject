using Cysharp.Threading.Tasks;
using Mathy.Core;
using Mathy.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Services
{
    public interface IResultScreenView : IView
    {
        event Action ON_CLOSE_CLICK;
        void Init(Camera camera);
        IResultScreenSkillsPanelView SkillView { get; }
        IResultScreenAchievementsView AchievementView { get; }
        IResultScreenRewardView RewardView { get; }
    }


    public class ResultScreenView : MonoBehaviour, IResultScreenView
    {
        public event Action ON_CLOSE_CLICK;

        [SerializeField] private Button _closeButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ResultScreenSkillsPanelView _skillView;
        [SerializeField] private ResultScreenAchievementsView _achievementView;
        [SerializeField] private ResultScreenRewardView _rewardView;
        [SerializeField] private BaseViewAnimator _animator;

        public IResultScreenSkillsPanelView SkillView => _skillView;
        public IResultScreenAchievementsView AchievementView => _achievementView;
        public IResultScreenRewardView RewardView => _rewardView;

        public void Init(Camera camera)
        {
            _canvas.worldCamera = camera;
        }

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            _closeButton.onClick.AddListener(OnCloseClick);
            _animator.AnimateShowing(onShow);
        }

        public void Hide(Action onHide)
        {
            _closeButton.onClick.RemoveListener(OnCloseClick);

            _animator.AnimateHiding(()=>
            {
                gameObject.SetActive(false);
                onHide?.Invoke();
            });
        }

        public void Release()
        {
            SkillView.Release();
            Destroy(gameObject);
        }

        private void OnCloseClick()
        {
            ON_CLOSE_CLICK?.Invoke();
        }
    }
}



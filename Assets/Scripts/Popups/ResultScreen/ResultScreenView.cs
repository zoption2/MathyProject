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
        IResultScreenSkillsPanelView SkillResults { get; }
        IResultScreenAchievementsView AchievementResults { get; }
    }


    public class ResultScreenView : MonoBehaviour, IResultScreenView
    {
        public event Action ON_CLOSE_CLICK;

        [SerializeField] private Button _closeButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ResultScreenSkillsPanelView _skillResults;
        [SerializeField] private ResultScreenAchievementsView _achievementResults;
        [SerializeField] private BaseViewAnimator _animator;

        public IResultScreenSkillsPanelView SkillResults => _skillResults;
        public IResultScreenAchievementsView AchievementResults => _achievementResults;

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
            SkillResults.Release();
            Destroy(gameObject);
        }

        private void OnCloseClick()
        {
            ON_CLOSE_CLICK?.Invoke();
        }
    }
}



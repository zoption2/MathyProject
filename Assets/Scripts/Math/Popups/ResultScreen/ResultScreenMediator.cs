using Mathy.Services.UI;
using System;
using UnityEngine;
using Mathy.UI;

namespace Mathy.Services
{
    public interface IResultScreenMediator : IPopupView
    {
        event Action ON_CLOSE_CLICK;
    }


    public class ResultScreenMediator : IResultScreenMediator
    {
        public event Action ON_CLOSE_CLICK;

        private const string kResultScreenTable = "ResultScreen";

        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;
        private readonly IResultScreenSkillsController _skillController;
        private readonly IResultScreenAchievementsController _achievementController;
        private readonly IResultScreenRewardController _rewardController;
        private IResultScreenView _view;

        public ResultScreenMediator(IAddressableRefsHolder refsHolder
            , IResultScreenSkillsController skillController
            , IResultScreenAchievementsController achievementController
            , IResultScreenRewardController rewardController
            , IUIManager uIManager)
        {
            _refsHolder = refsHolder;
            _skillController = skillController;
            _achievementController = achievementController;
            _rewardController = rewardController;
            _uiManager = uIManager;
        }

        public async void Init(Camera camera, Transform parent, Action onComplete)
        {
            _view = await _refsHolder.PopupsProvider.InstantiateFromReference<IResultScreenView>(Popups.ResultScreen, parent);
            _view.ON_CLOSE_CLICK += DoOnCloseClick;
            _view.Init(camera);
            InitSkillController();
            InitAchievementController();
            InitRewardController();
            _view.Show(onComplete);
        }

        public void Show(Action onShow)
        {
            _uiManager.OpenView(this, UIBehaviour.Disposable, onShow);
        }

        private void InitSkillController()
        {
            IResultScreenSkillsPanelView view = _view.SkillView;
            _skillController.Init(view);
        }

        private void InitAchievementController()
        {
            IResultScreenAchievementsView view = _view.AchievementView;
            _achievementController.Init(view);
        }

        private void InitRewardController()
        {
            IResultScreenRewardView view = _view.RewardView;
            _rewardController.Init(view);
        }

        public void Hide(Action onHide)
        {
            _view.Hide(onHide);
        }

        public void Release()
        {
            _view.Release();
        }

        private void DoOnCloseClick()
        {
            _view.ON_CLOSE_CLICK -= DoOnCloseClick;
            ON_CLOSE_CLICK?.Invoke();
            _uiManager.CloseView(this);
        }
    }
}



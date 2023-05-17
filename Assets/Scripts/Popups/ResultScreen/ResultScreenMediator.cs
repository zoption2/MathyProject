using Cysharp.Threading.Tasks;
using Mathy.Services.UI;
using System;
using UnityEngine;


namespace Mathy.UI
{
    public interface IResultScreenMediator : IPopupMediator
    {
        public event Action ON_CLOSE_CLICK;
    }


    public class ResultScreenMediator : IResultScreenMediator, IPopupView
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

        public void CreatePopup(Action onComplete = null)
        {
            _uiManager.OpenView(this, viewBehaviour: UIBehaviour.StayWithNew, onShow: onComplete);
        }

        public async UniTask InitPopup(Camera camera, Transform parent, int priority = 0)
        {
            _view = await _refsHolder.PopupsProvider.InstantiateFromReference<IResultScreenView>(Popups.ResultScreen, parent);
            _view.ON_CLOSE_CLICK += DoOnCloseClick;
            _view.Init(camera, priority);
            InitSkillController();
            InitAchievementController();
            InitRewardController();
        }

        public void Show(Action onShow)
        {
            _view.Show(onShow);
        }

        public void ClosePopup(Action onComplete = null)
        {
            _uiManager.CloseView(this, onComplete);
            onComplete?.Invoke();
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
            ClosePopup();
        }
    }
}



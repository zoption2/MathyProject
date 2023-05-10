using Mathy.Services.UI;
using System;
using UnityEngine;
using Mathy.UI;

namespace Mathy.Services
{
    public interface IResultScreenMediator : IPopupView
    {
        
    }


    public class ResultScreenMediator : IResultScreenMediator
    {
        private const string kResultScreenTable = "ResultScreen";

        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;
        private readonly IResultScreenSkillsController _skillController;
        private readonly IResultScreenAchievementsController _achievementController;
        private IResultScreenView _view;

        public ResultScreenMediator(IAddressableRefsHolder refsHolder
            , IResultScreenSkillsController skillController
            , IResultScreenAchievementsController achievementController
            , IUIManager uIManager)
        {
            _refsHolder = refsHolder;
            _skillController = skillController;
            _achievementController = achievementController;
            _uiManager = uIManager;
        }

        public async void Init(Camera camera, Transform parent, Action onComplete)
        {
            _view = await _refsHolder.PopupsProvider.InstantiateFromReference<IResultScreenView>(Popups.ResultScreen, parent);
            _view.ON_CLOSE_CLICK += DoOnCloseClick;
            _view.Init(camera);
            InitSkillResults();
            InitAchievementResults();
            _view.Show(onComplete);
        }

        public void Show(Action onShow)
        {
            _uiManager.OpenView(this, UIBehaviour.Disposable, onShow);
        }

        private void InitSkillResults()
        {
            IResultScreenSkillsPanelView view = _view.SkillResults;
            _skillController.Init(view);
        }

        private void InitAchievementResults()
        {
            IResultScreenAchievementsView view = _view.AchievementResults;
            _achievementController.Init(view);
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
            _uiManager.CloseView(this);
        }
    }
}



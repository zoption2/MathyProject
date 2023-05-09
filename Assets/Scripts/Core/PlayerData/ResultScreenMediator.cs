using Cysharp.Threading.Tasks;
using Mathy.Services.UI;
using System;
using UnityEngine;

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
        private readonly IResultScreenSkillsController _skillResults;
        private IResultScreenView _view;

        public ResultScreenMediator(IAddressableRefsHolder refsHolder
            , IResultScreenSkillsController skillResults
            , IUIManager uIManager)
        {
            _refsHolder = refsHolder;
            _skillResults = skillResults;
            _uiManager = uIManager;
        }

        public async void Init(Camera camera, Transform parent, Action onComplete)
        {
            _view = await _refsHolder.PopupsProvider.InstantiateFromReference<IResultScreenView>(Popups.ResultScreen, parent);
            _view.ON_CLOSE_CLICK += DoOnCloseClick;
            _view.Init(camera);
            InitSkillResults();
            _view.Show(onComplete);
        }

        public void Show(Action onShow)
        {
            _uiManager.OpenView(this, UIBehaviour.Disposable, onShow);
        }

        public void InitSkillResults()
        {
            IResultScreenSkillsPanelView view = _view.SkillResults;
            _skillResults.Init(view);
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



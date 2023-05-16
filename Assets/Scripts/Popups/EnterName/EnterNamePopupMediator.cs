using Cysharp.Threading.Tasks;
using Mathy.Services;
using Mathy.Services.UI;
using System;
using UnityEngine;

namespace Mathy.UI
{
    public interface IEnterNamePopupMediator : IPopupView
    {
       
    }


    public class EnterNamePopupMediator : IEnterNamePopupMediator
    {
        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IUIManager _uiManager;
        private readonly IEnterNamePopupController _controller;

        public EnterNamePopupMediator(IAddressableRefsHolder refsHolder
            , IUIManager uiManager
            , IEnterNamePopupController controller)
        {
            _refsHolder = refsHolder;
            _uiManager = uiManager;
            _controller = controller;
        }

        public void CreatePopup(Action onComplete)
        {
            _uiManager.OpenView(this, UIBehaviour.HideOnNew, onComplete);
        }

        public void Show(Action onShow)
        {
            _controller.Show(onShow);
        }

        public void Hide(Action onHide)
        {
            _controller.Hide(onHide);
        }

        public async void InitPopup(Camera camera, Transform parent, Action onComplete, int orderLayer = 0)
        {
            var model = new EnterNamePopupModel();
            var view = await _refsHolder.PopupsProvider.InstantiateFromReference<IEnterNamePopupView>(Popups.EnterName, parent);
            view.Init(camera, orderLayer);
            _controller.Init(view);
        }

        public void Release()
        {
            _controller.Rel
        }

        public void Close()
        {

        }
    }

    public class EnterNamePopupModel : IModel
    { 
        public string LocalizedTitle { get; set; }
        public string LocalizedEnterNameText { get; set; }
        public string LocalizedDefaultPlayerName { get; set; }
        public string LocalizedSaveText { get; set; }
    }


    public interface IEnterNamePopupController : IBaseMediatedController, IView
    {
        public event Action<string> ON_CLOSE_CLICK;
        public event Action<string> ON_SAVE_CLICK;
    }

    public class EnterNamePopupController
        : BaseMediatedController<IEnterNamePopupView, EnterNamePopupModel>
        , IEnterNamePopupController

    {
        public event Action<string> ON_CLOSE_CLICK;
        public event Action<string> ON_SAVE_CLICK;

        private const string kEnterNameTable = "EnterNamePopup";
        private const string kTitleKey = "TitleKey";
        private const string kEnterNameKey = "EnterNameKey";
        private const string kDefaultPlayerKey = "DefaultPlayerKey";
        private const string kSaveKey = "SaveKey";
        private readonly IPlayerDataService _playerService;

        public EnterNamePopupController(IPlayerDataService playerService)
        {
            _playerService = playerService;
        }

        public void Show(Action onShow)
        {
            _view.Show(onShow);
        }

        public void Hide(Action onHide)
        {
            _view.Hide(onHide);
        }

        public void Release()
        {
            _view.Release();
        }

        protected override UniTask<EnterNamePopupModel> BuildModel()
        {
            _model = new EnterNamePopupModel();
            _model.LocalizedTitle = LocalizationManager.GetLocalizedString(kEnterNameTable, kTitleKey);
            _model.LocalizedEnterNameText = LocalizationManager.GetLocalizedString(kEnterNameTable, kEnterNameKey);
            _model.LocalizedDefaultPlayerName = LocalizationManager.GetLocalizedString(kEnterNameTable, kDefaultPlayerKey);
            _model.LocalizedSaveText = LocalizationManager.GetLocalizedString(kEnterNameTable, kSaveKey);

            return UniTask.FromResult(_model);
        }

        protected override void DoOnInit(IEnterNamePopupView view)
        {
            _view.SetTitle(_model.LocalizedTitle);
            _view.SetEnterNameText(_model.LocalizedEnterNameText);
            _view.SetDefaultPlayerName(_model.LocalizedDefaultPlayerName);
            _view.SetSaveText(_model.LocalizedSaveText);

            _view.ON_CLOSE_CLICK += DoOnCloseClick;
            _view.ON_SAVE_CLICK += DoOnSaveClick;
        }

        private async void DoOnSaveClick(string name)
        {
            _view.ON_SAVE_CLICK -= DoOnSaveClick;
            await _playerService.Account.SetPlayerName(name);
        }

        private async void DoOnCloseClick(string defaultName)
        {
            _view.ON_CLOSE_CLICK -= DoOnCloseClick;
            await _playerService.Account.SetPlayerName(defaultName);
        }


    }
}



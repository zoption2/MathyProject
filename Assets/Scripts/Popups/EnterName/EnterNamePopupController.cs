using Cysharp.Threading.Tasks;
using Mathy.Services;
using System;

namespace Mathy.UI
{
    public interface IEnterNamePopupController : IBaseMediatedController, IView
    {
        public event Action<string> ON_NAME_CHANGED;
    }


    public class EnterNamePopupController
        : BaseMediatedController<IEnterNamePopupView, EnterNamePopupModel>
        , IEnterNamePopupController

    {
        public event Action<string> ON_NAME_CHANGED;

        private const string kEnterNameTable = "EnterNamePopup";
        private const string kTitleKey = "TitleKey";
        private const string kEnterNameKey = "EnterNameKey";
        private const string kDefaultPlayerKey = "DefaultPlayerKey";
        private const string kSaveKey = "SaveKey";


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

        private void DoOnSaveClick(string name)
        {
            _view.ON_SAVE_CLICK -= DoOnSaveClick;
            ON_NAME_CHANGED?.Invoke(name);
        }

        private void DoOnCloseClick(string defaultName)
        {
            _view.ON_CLOSE_CLICK -= DoOnCloseClick;
            ON_NAME_CHANGED?.Invoke(defaultName);
        }
    }
}



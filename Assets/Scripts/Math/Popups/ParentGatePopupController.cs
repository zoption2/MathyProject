using UnityEngine;
using Cysharp.Threading.Tasks;
using Mathy.Services;
using Mathy.Services.UI;
using System;

namespace Mathy.UI
{
    public interface IParentGatePopupMediator : IPopupView
    {
        public event Action ON_CANCEL;
        public event Action ON_COMPLETE;
        public void Close(Action callback = null);
    }

    public class ParentGatePopupMediator : IParentGatePopupMediator
    {
        private const int kPopupPriority = 100;

        public event Action ON_CANCEL;
        public event Action ON_COMPLETE;

        private IAddressableRefsHolder _refsHolder;
        private IParentGatePopupController _controller;
        private IUIManager _uiManager;

        public ParentGatePopupMediator(IAddressableRefsHolder refsHolder
            , IParentGatePopupController controller
            , IUIManager uIManager)
        {
            _refsHolder = refsHolder;
            _controller = controller;
            _uiManager = uIManager;
        }

        public void Show(Action onShow)
        {
            _uiManager.OpenView(this, UIBehaviour.StayWithNew, onShow, kPopupPriority);
        }

        public void Hide(Action onHide)
        {
            _controller.Hide(onHide);
        }

        public void Close(Action callback = null)
        {
            _uiManager.CloseView(this, callback);
        }

        public async void Init(Camera camera, Transform parent, Action onComplete, int orderLayer = 0)
        {
            var model = new ParentGatePopupModel();
            var view = await _refsHolder.PopupsProvider.InstantiateFromReference<IParentGatePopupView>(Popups.ParentGate, parent);
            view.Init(camera, orderLayer);
            _controller.ON_CLOSE_CLICK += DoOnClose;
            _controller.ON_COMPLETE += DoOnComplete;
            await _controller.Init(model, view);
        }

        public void Release()
        {
            _controller.Release();
        }

        private void DoOnClose()
        {
            _controller.ON_CLOSE_CLICK -= DoOnClose;
            ON_CANCEL?.Invoke();
            //_parentGateService.Cancel();
        }

        private void DoOnComplete()
        {
            _controller.ON_COMPLETE -= DoOnComplete;
            //_parentGateService.Complete();
            ON_COMPLETE?.Invoke();
        }
    }


    public interface IParentGatePopupController : IBaseController
    {
        public event Action ON_CLOSE_CLICK;
        public event Action ON_COMPLETE;
    }


    public class ParentGatePopupController : BaseController<IParentGatePopupView, ParentGatePopupModel>, IParentGatePopupController
    {
        public event Action ON_CLOSE_CLICK;
        public event Action ON_COMPLETE;

        private const string kTableKey = "ParentGateTable";
        private const string kCapchaFormat = "{0} {1}";
        private const string kTensSuffix = "tens";
        private const string kUnitsSuffix = "units";

        private string _capchaValue;


        protected override async UniTask DoOnInit()
        {
            var localizedLabel = LocalizationManager.GetLocalizedString(kTableKey, Model.LabelKey);
            View.SetLabelText(localizedLabel);

            var localizedOkText = LocalizationManager.GetLocalizedString(kTableKey, Model.OkKey);
            View.SetOkText(localizedOkText);

            var localizedCancelText = LocalizationManager.GetLocalizedString(kTableKey, Model.CancelKey);
            View.SetCancelText(localizedCancelText);

            _capchaValue = Model.GetCapchaKey();
            var localizedCapchaText = BuildLocalizedCapcha(_capchaValue);
            View.SetCapchaText(localizedCapchaText);

            View.ON_OK_CLICK += DoOnOkButtonClick;
            View.ON_CANCEL_CLICK += DoOnCancelButtonClick;

            var completionSource = new UniTaskCompletionSource();
            View.Show(() =>
            {
                completionSource.TrySetResult();
            });
            await completionSource.Task;
        }

        public override void Release()
        {
            View.ON_OK_CLICK -= DoOnOkButtonClick;
            View.ON_CANCEL_CLICK -= DoOnCancelButtonClick;
            base.Release();
        }

        private void DoOnOkButtonClick(string value)
        {
            if (!value.Equals(_capchaValue))
            {
                View.ResetInputField();
                return;
            }

            ON_COMPLETE?.Invoke();
        }

        private void DoOnCancelButtonClick()
        {
            ON_CLOSE_CLICK?.Invoke();
        }

        private string BuildLocalizedCapcha(string capchaKey)
        {
            var chars = capchaKey.ToCharArray();
            var numberKeys = new string[chars.Length];
            numberKeys[0] = (chars[0] + kTensSuffix).ToString();
            numberKeys[1] = (chars[1] + kUnitsSuffix).ToString();

            var localizedTens = LocalizationManager.GetLocalizedString(kTableKey, numberKeys[0]);
            var localizedUnits = LocalizationManager.GetLocalizedString(kTableKey, numberKeys[1]);

            var result = string.Format(kCapchaFormat, localizedTens, localizedUnits);
            return result;
        }
    }
}


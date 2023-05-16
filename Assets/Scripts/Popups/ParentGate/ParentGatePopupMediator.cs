using UnityEngine;
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

        public void CreatePopup(Action onComplete = null)
        {
            _uiManager.OpenView(this, UIBehaviour.StayWithNew, onComplete, kPopupPriority);
        }

        public void Show(Action onShow)
        {
            _controller.Show(onShow);
        }

        public void Hide(Action onHide)
        {
            _controller.Hide(onHide);
        }

        public void Close(Action callback = null)
        {
            _uiManager.CloseView(this, callback);
        }

        public async void InitPopup(Camera camera, Transform parent, Action onComplete, int orderLayer = 0)
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
}


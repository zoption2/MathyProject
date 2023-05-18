using Cysharp.Threading.Tasks;
using Mathy.Services.UI;
using System;
using UnityEngine;

namespace Mathy.UI
{
    public interface IEnterNamePopupMediator : IPopupMediator
    {
        event Action<string> ON_COMPLETE;
    }


    public class EnterNamePopupMediator : IEnterNamePopupMediator, IPopupView
    {
        public event Action<string> ON_COMPLETE;

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

        public void CreatePopup(Action onComplete = null)
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

        public async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            var view = await _refsHolder.PopupsProvider.InstantiateFromReference<IEnterNamePopupView>(Popups.EnterName, parent);
            view.Init(camera, orderLayer);
            _controller.Init(view);
            _controller.ON_NAME_CHANGED += DoOnNameChanged;
        }

        public void Release()
        {
            _controller.Release();
        }

        public void ClosePopup(Action onComplete = null)
        {
            _uiManager.CloseView(this, onComplete);
        }

        private void DoOnNameChanged(string name)
        {
            _controller.ON_NAME_CHANGED -= DoOnNameChanged;
            ON_COMPLETE?.Invoke(name);
        }
    }
}



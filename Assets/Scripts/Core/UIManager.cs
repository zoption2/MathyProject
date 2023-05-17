using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mathy.Services.UI
{
    public enum UIBehaviour
    {
        StayWithNew = 1,
        HideOnNew = 2,
        CloseOnNew = 3,
    }

    public interface IUIManager
    {
        void OpenView(IPopupView view, UIBehaviour viewBehaviour = UIBehaviour.StayWithNew, Action onShow = null, int manualPriopity = -1);
        void CloseView(IPopupView view, Action onHide = null);
    }


    public class UIManager : IUIManager
    {
        private const string kGOName = "UIManager";
        private Transform _popupsHolder;
        private List<ViewInfo> _popups;

        private int _priority => _popups.Count;

        private Camera _camera => Camera.main;

        private Transform Holder
        {
            get
            {
                if (_popupsHolder == null)
                {
                    var go = new GameObject(kGOName);
                    _popupsHolder = go.transform;
                    GameObject.DontDestroyOnLoad(go);
                }
                return _popupsHolder;
            }
        }

        public UIManager()
        {
            _popups = new List<ViewInfo>();
        }


        public async void OpenView(IPopupView view
            , UIBehaviour viewBehaviour = UIBehaviour.StayWithNew
            , Action onShow = null
            , int manualPriority = -1)
        {
            var viewInfo = new ViewInfo();
            viewInfo.View = view;
            viewInfo.Behaviour = viewBehaviour;

            var popups = _popups.Count;
            if (popups > 0)
            {
                var previousViewInfo = _popups[popups - 1];
                var previousViewBehaviour = previousViewInfo.Behaviour;
                var previousView = previousViewInfo.View;
                switch (previousViewBehaviour)
                {
                    case UIBehaviour.HideOnNew:
                        previousView.Hide(null);
                        break;

                    case UIBehaviour.CloseOnNew:
                        _popups.Remove(previousViewInfo);
                        previousView.Hide(() =>
                        {
                            previousView.Release();
                        });
                        break;

                    default:
                        break;
                }
            }

            var priority = manualPriority == -1
                ? _priority
                : manualPriority;

            if (!_popups.Contains(viewInfo))
            {
                _popups.Add(viewInfo);
                await view.InitPopup(_camera, Holder, priority);
                view.Show(onShow);
            }
        }

        public void CloseView(IPopupView view, Action onHide = null)
        {
            var selectedViewInfo = _popups.FirstOrDefault(x => x.View == view);
            var selectedView = selectedViewInfo.View;
            selectedView.Hide(() =>
            {
                _popups.Remove(selectedViewInfo);
                selectedView.Release();
                onHide?.Invoke();
                OnHide();
            });

            void OnHide()
            {
                var popups = _popups.Count;
                if (popups > 0)
                {
                    var previousViewInfo = _popups[popups - 1];
                    var previousViewBehaviour = previousViewInfo.Behaviour;
                    var previousView = previousViewInfo.View;
                    switch (previousViewBehaviour)
                    {
                        case UIBehaviour.HideOnNew:
                            previousView.Show(null);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private class ViewInfo
        {
            public IPopupView View { get; set; }
            public UIBehaviour Behaviour { get; set; }
        }
    }
}


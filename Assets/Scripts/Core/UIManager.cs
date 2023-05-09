using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Mathy.Services.UI
{
    public enum UIBehaviour
    {
        Disposable,
        Reusable,
    }

    public interface IUIManager
    {
        void OpenView(IPopupView view, UIBehaviour viewBehaviour = UIBehaviour.Disposable, Action onShow = null);
        void CloseView(IPopupView view, Action onHide = null);
    }


    public class UIManager : IUIManager
    {
        private const string kGOName = "UIManager";
        private Transform _popupsHolder;
        private List<IPopupView> _popups;
        private IPopupView _disposableView;

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
            _popups = new List<IPopupView>();
        }


        public void OpenView(IPopupView view
            , UIBehaviour viewBehaviour = UIBehaviour.Disposable
            , Action onShow = null)
        {
            if (viewBehaviour == UIBehaviour.Disposable)
            {
                _disposableView = view;
                _disposableView.Init(_camera, Holder, onShow);
            }
            else
            {
                if (!_popups.Contains(view))
                {
                    _popups.Add(view);
                }
                var selectedView = _popups.FirstOrDefault(x => x == view);
                selectedView.Init(_camera, Holder, onShow);
            }
        }

        public void CloseView(IPopupView view, Action onHide = null)
        {
            if (_disposableView == view)
            {
                _disposableView.Hide(() =>
                {
                    _disposableView.Release();
                    _disposableView = null;
                    onHide?.Invoke();
                });
            }
            else
            {
                if (_popups.Contains(view))
                {
                    var selectedView = _popups.FirstOrDefault(x => x == view);
                    _popups.Remove(view);
                    selectedView.Hide(() =>
                    {
                        selectedView.Release();
                        onHide?.Invoke();
                    });
                }
            }
        }
    }
}


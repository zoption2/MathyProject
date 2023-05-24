using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IView
{
    void Show(Action onShow);
    void Hide(Action onHide);
    void Release();
}

public interface IPopupView : IView
{
    UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0);
}

public interface IPopupMediator
{
    public event Action ON_CLOSE_CLICK;
    void CreatePopup(Action onComplete = null);
    void ClosePopup(Action onComplete = null);
}


public interface IModel
{

}




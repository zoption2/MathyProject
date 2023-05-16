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
    void CreatePopup(Action onComplete = null);
    void InitPopup(Camera camera, Transform parent, int orderLayer = 0);
    void ClosePopup(Action onComplete = null);
}

public interface IModel
{

}




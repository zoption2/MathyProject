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
    void Init(Camera camera, Transform parent, Action onComplete, int orderLayer = 0);
}

public interface IModel
{

}




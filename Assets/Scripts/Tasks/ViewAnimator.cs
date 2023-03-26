using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface IViewAnimator
    {
        public void AnimateShowing(Action onComplete);
        public void AnimateHiding(Action onComplete);
    }

    public abstract class BaseViewAnimator : MonoBehaviour, IViewAnimator
    {
        public abstract void AnimateHiding(Action onComplete);
        public abstract void AnimateShowing(Action onComplete);
    }
}


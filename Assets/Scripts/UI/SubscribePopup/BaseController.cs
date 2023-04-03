using System;
using Cysharp.Threading.Tasks;

namespace Mathy.UI
{
    public interface IBaseController : IView
    {
        public UniTask Init(IModel model, IView view);
    }


    public abstract class BaseController<TView, TModel> : IBaseController where TView : IView where TModel : IModel
    {
        public TModel Model { get; private set; }
        public TView View { get; private set; }

        protected abstract UniTask DoOnInit();

        public async UniTask Init(IModel model, IView view)
        {
            Model = (TModel)model;
            View = (TView)view;

            await DoOnInit();
        }

        public void Show(Action onShow)
        {
            View.Show(() =>
            {
                onShow?.Invoke();
            });
        }

        public void Hide(Action onHide)
        {
            View.Hide(() =>
            {
                onHide?.Invoke();
            });
        }

        public virtual void Release()
        {
            View.Release();
        }
    }
}



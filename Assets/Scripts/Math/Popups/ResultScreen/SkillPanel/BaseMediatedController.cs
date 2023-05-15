using Cysharp.Threading.Tasks;


namespace Mathy.UI
{
    public interface IBaseMediatedController
    {
        void Init(IView view);
    }


    public abstract class BaseMediatedController<TView, TModel> : IBaseMediatedController where TView : IView where TModel : IModel
    {
        protected TView _view;
        protected TModel _model;

        public async void Init(IView view)
        {
            _view = (TView)view;
            _model = await BuildModel();
            DoOnInit(_view);
        }

        protected abstract void DoOnInit(TView view);
        protected abstract UniTask<TModel> BuildModel();
    }
}



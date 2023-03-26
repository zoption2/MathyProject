using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using TASK = Mathy.Core.Tasks.Task;
using ValueTask = System.Threading.Tasks.ValueTask;
using Mathy.Data;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskController
    {
        public event Action<ITaskController> ON_COMPLETE;
        public event Action ON_FORCE_EXIT;
        public Transform ViewParent { get; set; }
        public void Init(IModel model, IView view);
        void StartTask();
        TaskData GetResults();
        void HideAndRelease(Action callback);
        void ReleaseImmediate();
    }

    public abstract class BaseTaskController<TView, TModel> : ITaskController where TView : ITaskView where TModel: ITaskModel
    {
        public event Action<ITaskController> ON_COMPLETE;
        public event Action ON_FORCE_EXIT;

        protected ITaskViewComponentsProvider componentsFactory;
        protected ITaskBackgroundSevice backgroundSevice;
        protected TaskData taskData;
        private DateTime timer;

        protected virtual string LocalizationTableKey { get; } = "Game Names";
        protected virtual bool UseRandomBackground { get; } = false;
        public TModel Model { get; private set; }
        public TView View { get; private set; }
        public Transform ViewParent { get; set; }
        protected double TotalPlayingTime { get; private set; }


        public BaseTaskController(ITaskViewComponentsProvider componentsFactory
            , ITaskBackgroundSevice backgroundSevice)
        {
            this.componentsFactory = componentsFactory;
            this.backgroundSevice = backgroundSevice;
        }

        public void StartTask()
        {
            View.Show(() =>
            {
                StartTimer();
            });
        }

        public void Init(IModel model, IView view)
        {
            Model = (TModel)model;
            View = (TView)view;

            _ = DoOnInit();

            taskData = Model.GetResult();
            var title = LocalizationManager.GetLocalizedString(LocalizationTableKey, Model.TitleKey);
            View.SetTitle(title);

            View.ON_EXIT_CLICK += ExitButtonClick;
        }

        protected abstract UniTask DoOnInit();

        public abstract TaskData GetResults();

        public void HideAndRelease(Action callback)
        {
            View.ON_EXIT_CLICK -= ExitButtonClick;
            Model = default;
            View.Hide(() =>
            {
                ReleaseImmediate();
                callback?.Invoke();
            });
        }

        public void ReleaseImmediate()
        {
            DoOnRelease();
            View.Release();
        }

        private void StartTimer()
        {
            timer = DateTime.UtcNow;
        }

        protected void StopTimer()
        {
            var difference = DateTime.UtcNow - timer;
            TotalPlayingTime += difference.TotalMilliseconds;
        }

        protected void CompleteTask()
        {
            ON_COMPLETE?.Invoke(this);
        }

        private void ExitButtonClick()
        {
            OnExitButtonClick();
            ON_FORCE_EXIT?.Invoke();
        }

        protected virtual void OnExitButtonClick()
        {

        }

        protected virtual void DoOnRelease()
        {

        }
    }
}


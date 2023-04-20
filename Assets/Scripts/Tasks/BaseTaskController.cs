using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using TASK = Mathy.Core.Tasks.Task;
using ValueTask = System.Threading.Tasks.ValueTask;
using Mathy.Data;
using Random = System.Random;
using System.Collections.Generic;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskController
    {
        public event Action<ITaskController> ON_COMPLETE;
        public event Action ON_FORCE_EXIT;
        public Transform ViewParent { get; set; }
        public UniTask Init(IModel model, IView view);
        void Prepare();
        void StartTask();
        TaskResultData GetResults();
        void HideAndRelease(Action callback);
        void ReleaseImmediate();
    }

    public abstract class BaseTaskController<TView, TModel> : ITaskController where TView : ITaskView where TModel: ITaskModel
    {
        public event Action<ITaskController> ON_COMPLETE;
        public event Action ON_FORCE_EXIT;

        protected ITaskBackgroundSevice backgroundSevice;
        protected IAddressableRefsHolder refsHolder;
        protected Random random;
        protected TaskResultData taskData;
        private DateTime timer;
        private double totalPlayingTime;

        protected abstract bool IsAnswerCorrect { get; set; }
        protected abstract List<int> SelectedAnswerIndexes { get; set; }

        protected virtual string LocalizationTableKey { get; } = "Game Names";
        protected virtual bool UseRandomBackground { get; } = false;
        public TModel Model { get; private set; }
        public TView View { get; private set; }
        public Transform ViewParent { get; set; }


        public BaseTaskController(IAddressableRefsHolder refsHolder
            , ITaskBackgroundSevice backgroundSevice)
        {
            this.backgroundSevice = backgroundSevice;
            this.refsHolder = refsHolder;
            random = new Random();
            SelectedAnswerIndexes = new List<int>();
        }

        public void Prepare()
        {
            DoOnTaskPrepare();
        }

        public void StartTask()
        {
            View.Show(() =>
            {
                OnTaskShowed();
                StartTimer();
            });
        }

        protected virtual void OnTaskShowed()
        {

        }

        protected virtual void DoOnTaskPrepare()
        {

        }

        public async UniTask Init(IModel model, IView view)
        {
            Model = (TModel)model;
            View = (TView)view;

            await DoOnInit();

            taskData = Model.GetResult();
            var title = GetLocalizedTitle();
            View.SetTitle(title);

            View.ON_EXIT_CLICK += ExitButtonClick;
        }

        protected virtual string GetLocalizedTitle()
        {
            return LocalizationManager.GetLocalizedString(LocalizationTableKey, Model.TitleKey);
        }

        protected abstract UniTask DoOnInit();

        public TaskResultData GetResults()
        {
            taskData.Duration = totalPlayingTime;
            taskData.IsAnswerCorrect = IsAnswerCorrect;
            taskData.SelectedAnswerIndexes = SelectedAnswerIndexes;
            return taskData;
        }

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

        private void StopTimer()
        {
            var difference = DateTime.UtcNow - timer;
            totalPlayingTime += difference.TotalMilliseconds;
        }

        protected void CompleteTask()
        {
            StopTimer();
            ON_COMPLETE?.Invoke(this);
        }

        private void ExitButtonClick()
        {
            OnExitButtonClick();
            ON_FORCE_EXIT?.Invoke();
        }

        protected virtual UIComponentType GetElementViewByType(TaskElementType type)
        {
            switch (type)
            {
                case TaskElementType.Value:
                    return UIComponentType.DefaultElement;

                case TaskElementType.Operator:
                    return UIComponentType.DefaultOperator;

                default:
                    throw new ArgumentException(
                        string.Format("{0} type of element not found", type)
                        );
            }
        }

        protected virtual void OnExitButtonClick()
        {
            VibrationManager.Instance.TapVibrateCustom();
            AudioManager.Instance.ButtonClickSound();
        }

        protected virtual void DoOnRelease()
        {

        }
    }
}


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mathy.UI.Tasks;
using DG.Tweening;
using System;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskViewComponent
    {
        event Action<TaskElementState> ON_STATE_CHANGE;
        public TaskElementState State { get; }
        public int Index { get; }
        public string Value { get; }
        void Init(int index, string value, TaskElementState initedState = TaskElementState.Default);
        void ChangeValue(string value);
        void ChangeState(TaskElementState state);
        void Release();
    }

    public class TaskElementView : MonoBehaviour, ITaskViewComponent
    {
        public event Action<TaskElementState> ON_STATE_CHANGE;

        private const int kDefaultSpriteIndex = 0;
        private const int kCorrectSpriteIndex = 1;
        private const int kWrongSpriteIndex = 2;
        private const int kUnknownSpriteIndex = 3;

        [SerializeField] protected TMP_Text valueText;
        [SerializeField] private Image stateImage;
        [SerializeField] private Sprite[] stateSprites;
        private TaskElementState state;
        private int index;

        public int Index => index;
        private Transform tweenID => transform;
        public string Value => valueText.text;
        public TaskElementState State => state;

        public virtual void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            this.index = index; 
            valueText.text = value;
            state = initedState;
            stateImage.sprite = stateSprites[(int)initedState];

            ON_STATE_CHANGE?.Invoke(initedState);
        }

        public void ChangeValue(string value)
        {
            valueText.text = value;
            AnimateText();
        }

        public void ChangeState(TaskElementState state)
        {
            if (this.state != state)
            {
                this.state = state;
                stateImage.sprite = stateSprites[(int)state];
                AnimatePress();
                DoOnStateChanged(state);
                ON_STATE_CHANGE?.Invoke(state);
            }
        }

        protected virtual void DoOnStateChanged(TaskElementState state)
        {

        }

        public virtual void Release()
        {
            gameObject.SetActive(false);
            DOTween.Kill(tweenID);
            Destroy(gameObject);
        }

        protected virtual void AnimateText()
        {
            DOTween.Complete(tweenID);
            valueText.transform.DOShakeRotation(UnityEngine.Random.Range(0.5f, 1f), new Vector2(20, 60))
                .SetId(tweenID)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    valueText.transform.eulerAngles = Vector3.zero;
                });
        }

        protected virtual void AnimatePress()
        {
            DOTween.Complete(tweenID);
            stateImage.transform.DOPunchScale(new Vector2(-0.1f, 0.1f), 0.5f)
                .SetId(tweenID)
                .SetEase(Ease.InOutQuad);
        }
    }
}


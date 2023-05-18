using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mathy.UI.Tasks;
using DG.Tweening;
using System;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskElementFrame : ITaskViewComponent
    {
        void Init(int index, string value, Sprite image, TaskElementState initedState = TaskElementState.Default);
        void ChangeValue(string value, bool isCorrect);
    }


    public class TaskElementFrame : MonoBehaviour, ITaskElementFrame
    {
        public event Action<TaskElementState> ON_STATE_CHANGE;

        private const int kDefaultColorIndex = 0;
        private const int kCorrectColorIndex = 1;
        private const int kWrongColorIndex = 2;
        private const int kUnknownColorIndex = 3;
        private const float kScalingTime = 0.5f;

        private readonly Vector2 kStartObjectSize = Vector2.zero;
        private readonly Vector2 kEndObjectSize = Vector2.one;

        [SerializeField] private Image stateImage;
        [SerializeField] private Image objectImage;
        [SerializeField] private TMP_Text objectValue;
        [SerializeField] private RectTransform objectHolder;
        [SerializeField] private Color[] stateColors;
        private TaskElementState state;
        private int index;
        private string value;


        private Transform tweenID => transform;
        public int Index => index;
        public string Value => Value;

        public void Init(int index, string value, Sprite image, TaskElementState initedState = TaskElementState.Default)
        {
            SetObjectImage(image);
            Init(index, value, initedState);
        }

        public void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            this.index = index;
            this.value = value;
            state = initedState;
            stateImage.color = stateColors[(int)initedState];

            DoOnStateChanged(initedState);
        }

        public void ChangeState(TaskElementState state)
        {
            if (this.state != state)
            {
                stateImage.color = stateColors[(int)state];
                DoOnStateChanged(state);
                AnimateObjectHolder();
                AnimatePress();
            }
        }

        public void ChangeValue(string value, bool isCorrect)
        {
            ChangeValue(value);
            if(isCorrect)
            {
                objectValue.color = stateColors[kCorrectColorIndex];
            }
        }

        public void ChangeValue(string value)
        {
            this.value = value;
            objectValue.text = value;
        }

        public void Release()
        {
            gameObject.SetActive(false);
            DOTween.Kill(tweenID);
            Destroy(gameObject);
        }

        private void SetObjectImage(Sprite sprite)
        {
            objectImage.sprite = sprite;
        }

        private void DoOnStateChanged(TaskElementState state)
        {
            switch (state)
            {
                case TaskElementState.Default:
                    objectHolder.sizeDelta = kEndObjectSize;
                    objectImage.gameObject.SetActive(true);
                    objectValue.gameObject.SetActive(false);
                    break;

                case TaskElementState.Correct:
                    objectImage.gameObject.SetActive(true);
                    objectValue.gameObject.SetActive(false);
                    break;

                case TaskElementState.Wrong:
                    objectImage.gameObject.SetActive(false);
                    objectValue.gameObject.SetActive(true);

                    break;
                case TaskElementState.Unknown:
                    objectHolder.localScale = kStartObjectSize;
                    break;
                default:
                    throw new NotImplementedException(
                        string.Format("State {0} not inplemented for {1}", state, typeof(TaskElementFrame))
                        );
            }

            ON_STATE_CHANGE?.Invoke(state);
        }

        private void AnimateObjectHolder()
        {
            objectHolder.DOScale(kEndObjectSize, kScalingTime).SetEase(Ease.OutBack).SetId(tweenID);
        }

        private void AnimatePress()
        {
            DOTween.Complete(tweenID);
            stateImage.transform.DOPunchScale(new Vector2(-0.01f, 0.01f), 0.5f)
                .SetId(tweenID)
                .SetEase(Ease.InOutQuad);
        }
    }
}


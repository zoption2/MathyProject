using UnityEngine;
using UnityEngine.UI;
using Mathy.UI.Tasks;
using System;
using DG.Tweening;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class TaskButtonVariantClickable : MonoBehaviour, ITaskViewComponentClickable
    {
        public event Action<ITaskViewComponent> ON_CLICK;

        private const int kDefaultColorIndex = 0;
        private const int kCorrectColorIndex = 1;
        private const int kWrongColorIndex = 2;
        private const int kUnknownColorIndex = 3;

        [SerializeField] private Button button;
        [SerializeField] private Image stateImage;
        [SerializeField] private Color[] stateColors;
        private TaskElementState state;
        private int index;
        private string value;

        public int Index => index;
        private Transform tweenID => transform;
        public string Value => value;


        public virtual void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            this.index = index;
            this.value = value;
            state = initedState;
            stateImage.color = stateColors[(int)initedState];
            button.onClick.AddListener(DoOnClick);
        }

        public void ChangeValue(string value)
        {
            this.value = value;
        }

        public void ChangeState(TaskElementState state)
        {
            if (this.state != state)
            {
                stateImage.color = stateColors[(int)state];
                AnimatePress();
            }
        }

        public virtual void Release()
        {
            button.onClick.RemoveListener(DoOnClick);
            gameObject.SetActive(false);
            DOTween.Kill(tweenID);
            Destroy(gameObject);
        }

        private void AnimatePress()
        {
            DOTween.Complete(tweenID);
            stateImage.transform.DOPunchScale(new Vector2(-0.01f, 0.01f), 0.5f)
                .SetId(tweenID)
                .SetEase(Ease.InOutQuad);
        }

        private void DoOnClick()
        {
            ON_CLICK?.Invoke(this);
        }
    }
}


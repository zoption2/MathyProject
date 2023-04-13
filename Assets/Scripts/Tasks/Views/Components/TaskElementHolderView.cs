using UnityEngine;
using UnityEngine.UI;
using Mathy.UI.Tasks;
using DG.Tweening;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskElementHolderView
    {
        public int Index { get; }
        Transform Parent { get; }
        void Init(int index, TaskElementState initedState = TaskElementState.Default);
        void ChangeState(TaskElementState state);
        void Release();
    }


    public class TaskElementHolderView : MonoBehaviour, ITaskElementHolderView
    {
        private const int kDefaultColorIndex = 0;
        private const int kCorrectColorIndex = 1;
        private const int kWrongColorIndex = 2;
        private const int kUnknownColorIndex = 3;

        [SerializeField] private Image stateImage;
        [SerializeField] private float shakePower = 0.01f;
        [SerializeField] private Color[] stateSprites;
        private TaskElementState state;
        private int index;

        public int Index => index;
        public Transform Parent => transform;
        private Transform tweenID => transform;


        public virtual void Init(int index, TaskElementState initedState = TaskElementState.Default)
        {
            this.index = index;
            state = initedState;
            stateImage.color = stateSprites[(int)initedState];
        }

        public void ChangeState(TaskElementState state)
        {
            if (this.state != state)
            {
                stateImage.color = stateSprites[(int)state];
                AnimatePress();
            }
        }

        public virtual void Release()
        {
            gameObject.SetActive(false);
            DOTween.Kill(tweenID);
            Destroy(gameObject);
        }

        protected virtual void AnimatePress()
        {
            DOTween.Complete(tweenID);
            stateImage.transform.DOPunchScale(new Vector2(-shakePower, shakePower), 0.5f)
                .SetId(tweenID)
                .SetEase(Ease.InOutQuad);
        }
    }
}


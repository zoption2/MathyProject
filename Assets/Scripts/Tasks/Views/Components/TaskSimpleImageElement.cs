using Mathy.UI.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskSimpleImageElement : ITaskViewComponent
    {
        void Init(int index, string value, Sprite image, TaskElementState initedState = TaskElementState.Default);
    }


    public class TaskSimpleImageElement : MonoBehaviour, ITaskSimpleImageElement
    {
        public event Action<TaskElementState> ON_STATE_CHANGE;

        [SerializeField] private Image objectImage;
        [SerializeField] private RectTransform objectHolder;
        private TaskElementState state;
        private int index;
        private string value;

        public int Index => index;
        public string Value => value;


        public void Init(int index, string value, Sprite image, TaskElementState initedState = TaskElementState.Default)
        {
            SetObjectImage(image);
            Init(index, value, initedState);
        }

        public void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            this.index = index;
            this.value = value;
            ChangeState(initedState);
        }

        public void ChangeState(TaskElementState state)
        {
            ON_STATE_CHANGE?.Invoke(state);
        }

        public void ChangeValue(string value)
        {
            this.value = value;
        }

        public void Release()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        private void SetObjectImage(Sprite sprite)
        {
            objectImage.sprite = sprite;
        }
    }
}


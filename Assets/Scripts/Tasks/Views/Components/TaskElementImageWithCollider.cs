using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskElementImageWithCollider
    {
        void Init(int index, Sprite sprite);
        void Release();
    }

    public class TaskElementImageWithCollider : MonoBehaviour, ITaskElementImageWithCollider
    {
        [SerializeField] private Image image;

        private int index;
        public int Index => index;


        public void Init(int index, Sprite sprite)
        {
            this.index = index;
            image.sprite = sprite;
        }

        public void Release()
        {
            Destroy(gameObject);
        }
    }
}


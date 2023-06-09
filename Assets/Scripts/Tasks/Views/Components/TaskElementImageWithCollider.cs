using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskElementImageWithCollider
    {
        void Init(int index, Sprite sprite);
        void SetPosition(Vector2 position);
        void EnableColliders(bool isEnable = true);
        void SetSize(float sizePixels);
        void Release();
    }

    public class TaskElementImageWithCollider : MonoBehaviour, ITaskElementImageWithCollider
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CircleCollider2D elementCollider;

        private int index;
        public int Index => index;

        public void Init(int index, Sprite sprite)
        {
            this.index = index;
            image.sprite = sprite;
            EnableColliders(false);
        }

        public void EnableColliders(bool isEnable = true)
        {
            elementCollider.enabled = isEnable;
        }

        public void SetSize(float sizePixels)
        {
            var size = rectTransform.sizeDelta;
            size.x = sizePixels;
            size.y = sizePixels;
            rectTransform.sizeDelta = size;
            elementCollider.radius = sizePixels / 2;
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.localPosition = position;
        }

        public void Release()
        {
            Destroy(gameObject);
        }
    }
}


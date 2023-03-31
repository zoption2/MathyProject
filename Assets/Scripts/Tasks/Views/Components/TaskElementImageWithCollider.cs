using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskElementImageWithCollider
    {
        void Init(int index, TaskCountedImageElementType imageType);
        void Init(int index, Sprite sprite);
        void SetPosition(Vector2 position);
        void EnableColliders(bool isEnable = true);
        void Release();
    }

    public class TaskElementImageWithCollider : MonoBehaviour, ITaskElementImageWithCollider
    {
        private const int kBeanStartIndex = 0;
        private const int kBeanEndIndex = 2;
        private const int kCheeseIndex = 3;
        private const int kCoinIndex = 4;
        private const int kFlowerIndex = 5;
        private const int kLeafIndex = 6;
        private const int kCandyStartIndex = 7;
        private const int kCandyEndIndex = 9;
        private const int kStrawberryIndex = 10;

        [SerializeField] private Image image;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Collider2D elementCollider;
        [SerializeField] private Sprite[] sprites;

        private int index;
        public int Index => index;


        public void Init(int index, TaskCountedImageElementType imageType)
        {
            this.index = index;
            image.sprite = GetSpriteByType(imageType);
            EnableColliders(false);
        }

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

        public void SetPosition(Vector2 position)
        {
            rectTransform.localPosition = position;
        }

        public void Release()
        {
            Destroy(gameObject);
        }

        private Sprite GetSpriteByType(TaskCountedImageElementType imageType)
        {
            switch (imageType)
            {
                case TaskCountedImageElementType.GreenBean:
                case TaskCountedImageElementType.OrangeBean:
                case TaskCountedImageElementType.VioletBean:
                    return sprites[Random.Range(kBeanStartIndex, kBeanEndIndex + 1)];

                case TaskCountedImageElementType.Cheese:
                    return sprites[kCheeseIndex];

                case TaskCountedImageElementType.Coin:
                    return sprites[kCoinIndex];

                case TaskCountedImageElementType.Flower:
                    return sprites[kFlowerIndex];

                case TaskCountedImageElementType.Leaf:
                    return sprites[kLeafIndex];

                case TaskCountedImageElementType.BlueCandy:
                case TaskCountedImageElementType.RedCandy:
                case TaskCountedImageElementType.VioletCandy:
                    return sprites[Random.Range(kCandyStartIndex, kCandyEndIndex + 1)];

                case TaskCountedImageElementType.Strawberry:
                    return sprites[kStrawberryIndex];
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}


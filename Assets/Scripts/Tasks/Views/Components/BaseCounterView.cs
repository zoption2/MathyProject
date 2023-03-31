using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


namespace Mathy.UI
{
    public interface ITaskCounter
    {
        GameObject gameObject { get; }
        void Init(int total, List<bool> existingAnswers = null);
        void ChangeStatusByIndex(int index, TaskStatus status);
        void SetCurrentCount(int currentIndex);
    }


    public class BaseCounterView : MonoBehaviour, ITaskCounter
    {
        private const int kPendingSpriteIndex = 0;
        private const int kInProgressSpriteIndex = 1;
        private const int kCorrectSpriteIndex = 2;
        private const int kWrongSpriteIndex = 3;
        private const string kCounterFormat = "{0}/{1}";

        [SerializeField] private TMP_Text counterText;
        [SerializeField] private Image[] indicators;
        [SerializeField] private Sprite[] statusSprites;

        private int totalIndicators;
        private bool isInited;

        public void ChangeStatusByIndex(int index, TaskStatus status)
        {
            if (isInited)
            {
                indicators[index].sprite = GetStatusSprite(status);
            }
        }

        public virtual void Init(int total, List<bool> existingAnswers = null)
        {
            totalIndicators = total;
            for (int i = 0, j = indicators.Length; i < j; i++)
            {
                bool isActive = i < total;
                indicators[i].gameObject.SetActive(isActive);
                if (isActive)
                {
                    indicators[i].sprite = statusSprites[kPendingSpriteIndex];
                }
            }

            for (int i = 0, j = existingAnswers.Count; i < j; i++)
            {
                bool isCorrect = existingAnswers[i];
                indicators[i].sprite = isCorrect
                    ? statusSprites[kCorrectSpriteIndex]
                    : statusSprites[kWrongSpriteIndex];
            }

            isInited = true;
        }

        public void SetCurrentCount(int currentIndex)
        {
            if (isInited)
            {
                counterText.text = string.Format(kCounterFormat, currentIndex, totalIndicators);
            }
        }

        private Sprite GetStatusSprite(TaskStatus status)
        {
            Sprite selectedSprite;
            switch (status)
            {
                case TaskStatus.Pending: return selectedSprite = statusSprites[kPendingSpriteIndex];
                case TaskStatus.InProgress: return selectedSprite = statusSprites[kInProgressSpriteIndex];
                case TaskStatus.Right: return selectedSprite = statusSprites[kCorrectSpriteIndex];
                case TaskStatus.Wrong: return selectedSprite = statusSprites[kWrongSpriteIndex];

                default:
                    throw new System.ArgumentException(
                        string.Format("There is no implementation of CounterStatus for {0} status, at {1}"
                        , status, typeof(DefaultTaskCounterView))
                        );
            }
        }
    }
}
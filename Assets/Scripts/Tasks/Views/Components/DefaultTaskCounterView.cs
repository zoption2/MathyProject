using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEditor.Tilemaps;


namespace Mathy.UI
{
    public interface ITaskCounter
    {
        GameObject gameObject { get; }
        void Init(int total, List<bool> existingAnswers = null);
        void ChangeStatusByIndex(int index, TaskStatus status);
        void SetCurrentCount(int currentIndex);
    }


    public class DefaultTaskCounterView : MonoBehaviour, ITaskCounter
    {
        private const int kPendingSpriteIndex = 0;
        private const int kInProgressSpriteIndex = 1;
        private const int kCorrectSpriteIndex = 2;
        private const int kWrongSpriteIndex = 3;
        private const string kCounterFormat = "{0}/{1}";
        private const float kTweenDuration = 0.5f;
        private const float kSmallOffset = 160;
        private const float kMediumOffset = 250;
        private const float kLargeOffset = 340;
        private readonly Vector3 scaleTo = new Vector3(0.05f, 0.1f, 0);

        [SerializeField] private TMP_Text counterText;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform movableRect;
        [SerializeField] private FlexibleGridLayout indicatorPanel;
        [SerializeField] private List<AudioClip> panelSounds;
        [SerializeField] private Image[] indicators;
        [SerializeField] private Sprite[] statusSprites;

        private bool isInited;
        private bool isOpened;
        private int totalIndicators;
        private float offsetY;

        private Transform TweenID => transform;
        private bool IsTweening => DOTween.IsTweening(TweenID);

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClick);
            DOTween.Kill(TweenID);
        }

        public void Init(int total, List<bool> existingAnswers = null)
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

            offsetY = GetOffsetForMode(total);
            isInited = true;
        }

        public void SetCurrentCount(int currentIndex)
        {
            if (isInited)
            {
                counterText.text = string.Format(kCounterFormat, currentIndex, totalIndicators);
            }
        }

        public void ChangeStatusByIndex(int index, TaskStatus status)
        {
            if (isInited)
            {
                indicators[index].sprite = GetStatusSprite(status);
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

        private void OnButtonClick()
        {
            if (!IsTweening)
            {
                DoMove();
            }
        }

        private void DoMove()
        {
            DOTween.Kill(TweenID);
            var sequence = DOTween.Sequence();

            if (isOpened)
            {
                sequence.Append(movableRect.DOShakeScale(kTweenDuration, scaleTo, 10, 60f)).SetId(TweenID);
                sequence.Join(movableRect.DOAnchorPos(new Vector2(0, 0), kTweenDuration).SetEase(Ease.OutBack)).SetId(TweenID);
                AudioSystem.Instance.PlaySound(panelSounds[0], UnityEngine.Random.Range(0.9f, 1.1f));
            }
            else
            {
                sequence.Append(movableRect.DOShakeScale(kTweenDuration, scaleTo, 10, 60f)).SetId(TweenID);
                sequence.Join(movableRect.DOAnchorPos(new Vector2(0, offsetY), kTweenDuration).SetEase(Ease.OutBack)).SetId(TweenID);
                AudioSystem.Instance.PlaySound(panelSounds[1], 0.5f, UnityEngine.Random.Range(0.9f, 1.1f));
            }
            isOpened = !isOpened;
        }

        private float GetOffsetForMode(int amount)
        {
            float result = kSmallOffset;
            switch (amount)
            {
                case 10:
                    indicatorPanel.padding.bottom = 390;
                    result = kSmallOffset;
                    break;
                case 20:
                    indicatorPanel.padding.bottom = 304;
                    result = kMediumOffset;
                    break;
                case 30:
                    indicatorPanel.padding.bottom = 218;
                    result = kLargeOffset;
                    break;
            }
            return result;
        }
    }
}
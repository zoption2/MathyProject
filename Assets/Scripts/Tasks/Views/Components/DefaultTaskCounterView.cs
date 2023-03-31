using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;


namespace Mathy.UI
{
    public class DefaultTaskCounterView : BaseCounterView, ITaskCounter
    {
        private const float kTweenDuration = 0.5f;
        private const float kSmallOffset = 160;
        private const float kMediumOffset = 250;
        private const float kLargeOffset = 340;
        private readonly Vector3 scaleTo = new Vector3(0.05f, 0.1f, 0);

        [SerializeField] private Button button;
        [SerializeField] private RectTransform movableRect;
        [SerializeField] private FlexibleGridLayout indicatorPanel;
        [SerializeField] private List<AudioClip> panelSounds;

        private bool isOpened;
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

        public override void Init(int total, List<bool> existingAnswers = null)
        {
            base.Init(total, existingAnswers);
            offsetY = GetOffsetForMode(total);
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
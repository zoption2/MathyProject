using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Mathy.UI
{
    public class PlayModeIndicator : TweenedButton
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private Image shiningImage;
        [SerializeField] private Image pendingIndicator;
        [SerializeField] private CanvasGroup canvasGroup;        
        [SerializeField] private ParticleSystem doneFX;

        [Header("REFERENCES:")]
        [SerializeField] private List<Sprite> statusImages;

        protected DailyModeState state = 0;
        public DailyModeState State
        {
            get => state;
            set
            {
                state = value;
                UpdateDisplayStyle();
            }
        }
        public override bool IsInteractable
        {
            get => button.interactable;
            set
            {
                button.interactable = value;
            }
        }
        public bool isDone { get; private set; } = false;

        #endregion

        /// <summary>
        /// Display completion status of the daily tasks mode.
        /// Set state 0 to hide status icon, 1 for not finished status and to show Notify icon
        /// and 2 and more for finished status to play tween and hide the indicator.
        /// </summary>
        private void UpdateDisplayStyle()
        {
            bool isPlayed = (int)state > 0;
            pendingIndicator.enabled = isPlayed;

            switch (state)
            {
                case DailyModeState.Default:
                    pendingIndicator.enabled = false;
                    button.interactable = true;
                    isDone = false;
                    break;
                case DailyModeState.Pending:
                    pendingIndicator.enabled = true;
                    button.interactable = true;
                    isDone = false;
                    break;
                case DailyModeState.Done:
                    pendingIndicator.enabled = true;
                    button.interactable = false;
                    isDone = true;
                    _ = DoneTween();
                    break;
                default:
                    goto case DailyModeState.Default;
            }
        }

        private async UniTask DoneTween()
        {
            PlayButtonPanel.Instance.SelectedNextMode();
            var sequence = DOTween.Sequence();
            sequence.Join(tweenTransform.DOScale(new Vector2(0.75f, 0.75f), 0.5f).SetEase(Ease.InOutQuad));
            sequence.Append(tweenTransform.DOScale(new Vector2(1.2f, 1.2f), 0.3f).SetEase(Ease.InOutQuad));
            sequence.Join(canvasGroup.DOFade(0, 0.3f));
            await UniTask.Delay(500);
            PlayDoneFX();
            await UniTask.Delay(1000);
            gameObject.SetActive(false);
        }

        public void Select(bool isSelected)
        {
            float endAlpha, duration;
            shiningImage.enabled = true;

            if (isSelected)
            {
                endAlpha = 1;
                duration = 0.5f;
                buttonImage.sprite = statusImages[0];
            }
            else
            {
                endAlpha = 0;
                duration = 0.25f;
                buttonImage.sprite = statusImages[1];
            }

            var sequence = DOTween.Sequence();
            sequence.Join(shiningImage.DOFade(endAlpha, duration));
            sequence.OnComplete(() => shiningImage.enabled = isSelected);
        }

        public void PlayDoneFX()
        {
            doneFX.Stop();
            doneFX.Play();
        }
    }

    public enum DailyModeState
    {
        Default = 0,
        Pending = 1,
        Done = 2
    }
}
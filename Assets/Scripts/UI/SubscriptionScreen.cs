using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections;

namespace Mathy.UI
{
    public interface ISubscriptionPopupStub
    {
        UniTask CheckSubscription();
    }

    public class SubscriptionScreen : StaticInstance<SubscriptionScreen>, ISubscriptionPopupStub
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private CanvasGroup gfxCanvasGroup;
        [SerializeField] private RectTransform popupWindow;
        [SerializeField] private TMP_Text freeTrialPeriodText;
        [SerializeField] private TMP_Text subscriptionPrice;

        [Header("CONFIG:")]
        [SerializeField] private string policyLink = "https://www.fivesysdev.com/Policy";

        private UniTaskCompletionSource _tcs;

        public bool IsOpened { get; private set; }

        #endregion

        public async UniTask CheckSubscription()
        {
            _tcs = new UniTaskCompletionSource();
            UpdateSubscriptionInfo();
            var result = CheckSubAsync();
            if (!result)
            {
                gameObject.SetActive(true);
                IAPManager.Instance.ON_PURCHASE_COMPLETE += DoOnPurchaseComplete;
                IAPManager.Instance.ON_PURCHASE_RESTORED += DoOnPurchaseRestored;

                await _tcs.Task;
                SetGFXActive(false);
                IAPManager.Instance.ON_PURCHASE_COMPLETE -= DoOnPurchaseComplete;
                IAPManager.Instance.ON_PURCHASE_RESTORED -= DoOnPurchaseRestored;
            }
        }

        private void DoOnPurchaseComplete()
        {
            IAPManager.Instance.ON_PURCHASE_COMPLETE -= DoOnPurchaseComplete;
            _tcs.TrySetResult();
        }

        private void DoOnPurchaseRestored()
        {
            IAPManager.Instance.ON_PURCHASE_RESTORED -= DoOnPurchaseRestored;
            _tcs.TrySetResult();
        }

        //private void Start()
        //{
        //    _ = CheckSubscriptionStatusAsync();
        //}

        //private void Update()
        //{
        //    if (gfxCanvasGroup.gameObject.activeInHierarchy)
        //    {
        //        UpdateSubscriptionInfo();
        //    }
        //}

        private async UniTask CheckSubscriptionStatusAsync()
        {
            while (true)
            {
                if (await IAPManager.Instance.IsSubscribed())
                {
                    SetGFXActive(false);
                    return;
                }
                await UniTask.Delay(1000);
            }
        }

        //private async UniTask<bool> CheckSubAsync()
        //{
        //    bool result;
        //    int attempts = 5;
        //    for (int i = attempts; i > 0; i--)
        //    {
        //        Debug.Log("Attempts to get subscription info left: " + attempts.ToString());

        //        result = await IAPManager.Instance.IsSubscribed();
        //        if (result)
        //        {
        //            Debug.Log("User is subscribed");
        //            return true;
        //        }
        //    }
        //    gameObject.SetActive(true);
        //    SetGFXActive(true);
        //    Debug.Log("User is NOT subscribed");
        //    return false;
        //}

        private bool CheckSubAsync()
        {
            var result = IAPManager.Instance.HasSubscription();
            if (result)
            {
                Debug.Log("User is subscribed");
                return true;
            }
            
            gameObject.SetActive(true);
            SetGFXActive(true);
            Debug.Log("User is NOT subscribed");
            return false;
        }

        public void UpdateSubscriptionInfo()
        {
            subscriptionPrice.text = IAPManager.Instance.GetSubscriptionPrice() +
                LocalizationSettings.StringDatabase.GetLocalizedString("In-App Purchasing", "SubscriptionApple_YearPrice");
            freeTrialPeriodText.text = IAPManager.Instance.GetSubscriptionFreeTrialPeriod() +
                LocalizationSettings.StringDatabase.GetLocalizedString("In-App Purchasing", "SubscriptionApple_FreePeriod");
        }

        public void SetGFXActive(bool isActive)
        {
            if (!IsOpened)
                gfxCanvasGroup.gameObject.SetActive(true);

            gfxCanvasGroup.alpha = isActive ? 0 : 1;
            gfxCanvasGroup.gameObject.SetActive(true);
            popupWindow.localScale = isActive ? Vector2.zero : Vector2.one;

            Sequence sequence = DOTween.Sequence();
            if (isActive)
            {
                sequence.Join(gfxCanvasGroup.DOFade(1, 0.25f).SetEase(Ease.InOutQuad));
                sequence.Append(popupWindow.DOScale(Vector2.one, 0.25f).SetEase(Ease.InOutQuad));
            }
            else
            {
                sequence.Join(popupWindow.DOScale(Vector2.zero, 0.25f).SetEase(Ease.InOutQuad));
                sequence.Append(gfxCanvasGroup.DOFade(0, 0.25f).SetEase(Ease.InOutQuad));
            }
            sequence.OnComplete(() => OnTweenComplete(isActive));
        }

        private void OnTweenComplete(bool isActive)
        {
            gfxCanvasGroup.gameObject.SetActive(isActive);
            gfxCanvasGroup.interactable = isActive;
            gfxCanvasGroup.blocksRaycasts = isActive;
            IsOpened = isActive;
        }

        public void OpenPolicyLink()
        {
            Application.OpenURL(policyLink);
        }
    }
}

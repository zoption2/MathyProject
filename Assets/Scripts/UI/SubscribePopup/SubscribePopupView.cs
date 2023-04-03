using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cysharp.Threading.Tasks;

namespace Mathy.UI
{
    public interface ISubscribePopupView : IView
    {
        public event Action ON_MONTH_SUBSCRIBE_CLICK;
        public event Action ON_YEAR_SUBSCRIBE_CLICK;
        void SetDescriptionText(string text);
        void SetYearPriceText(string text);
        void SetMonthPriceText(string text);
        void SetFreeTrialText(string text);
        void ShowFreeTrialText(bool isShow);
        void ShowSaleLabel(bool isShow);
    }


    public class SubscribePopupView : MonoBehaviour
    {
        public event Action ON_MONTH_SUBSCRIBE_CLICK;
        public event Action ON_YEAR_SUBSCRIBE_CLICK;

        [SerializeField] private Button yearSubscibeButton;
        [SerializeField] private Button monthSubscibeButton;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text monthPriceText;
        [SerializeField] private TMP_Text yearPriceText;
        [SerializeField] private TMP_Text[] tryForFreeTexts;
        [SerializeField] private GameObject saleLabel;

        private void OnEnable()
        {
            monthSubscibeButton.onClick.AddListener(DoOnMonthButtonClick);
            yearSubscibeButton.onClick.AddListener(DoOnYearButtonClick);
        }

        private void OnDisable()
        {
            monthSubscibeButton.onClick.RemoveListener(DoOnMonthButtonClick);
            yearSubscibeButton.onClick.RemoveListener(DoOnYearButtonClick);
        }

        public void SetDescriptionText(string text)
        {
            descriptionText.text = text;
        }

        public void SetYearPriceText(string text)
        {
            yearPriceText.text = text;
        }

        public void SetMonthPriceText(string text)
        {
            monthPriceText.text = text;
        }

        public void SetFreeTrialText(string text)
        {
            for (int i = 0, j = tryForFreeTexts.Length; i < j; i++)
            {
                tryForFreeTexts[i].text = text;
            }
        }

        public void ShowFreeTrialText(bool isShow)
        {
            for (int i = 0, j = tryForFreeTexts.Length; i < j; i++)
            {
                tryForFreeTexts[i].gameObject.SetActive(isShow);
            }
        }

        public void ShowSaleLabel(bool isShow)
        {
            saleLabel.SetActive(isShow);
        }

        private void DoOnYearButtonClick()
        {
            ON_YEAR_SUBSCRIBE_CLICK?.Invoke();
        }

        private void DoOnMonthButtonClick()
        {
            ON_MONTH_SUBSCRIBE_CLICK?.Invoke();
        }
    }

    public class SubscribePopupModel : IModel
    {
        public ReactiveProperty<bool> HasFreeTrial;
        public float YearPrice;
        public float MonthPrice;
        public string CurrencyType;
        public int FreeDays;
    }


    public class SubscribePopupController : BaseController<ISubscribePopupView, SubscribePopupModel>
    {
        private const string kPriceFormat = "{0} {1}/{2}";

        protected override async UniTask DoOnInit()
        {
            View.ON_MONTH_SUBSCRIBE_CLICK += OnMonthSubcribeClick;
            View.ON_YEAR_SUBSCRIBE_CLICK += OnYearSubcribeClick;
            await UniTask.CompletedTask;
        }

        private void OnMonthSubcribeClick()
        {

        }

        private void OnYearSubcribeClick()
        {

        }

        public override void Release()
        {
            View.ON_MONTH_SUBSCRIBE_CLICK -= OnMonthSubcribeClick;
            View.ON_YEAR_SUBSCRIBE_CLICK -= OnYearSubcribeClick;
            base.Release();
        }
    }
}





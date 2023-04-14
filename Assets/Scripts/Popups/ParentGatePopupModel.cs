using ModestTree;
using Random = System.Random;

namespace Mathy.UI
{
    public class ParentGatePopupModel : IModel
    {
        private const string kLabelKey = "LabelKey";
        private const string kOkKey = "OkKey";
        private const string kCancelKey = "CancelKey";
        private const int kMinCapchaValue = 21;
        private const int kMaxCapchaValue = 99;

        private Random random;
        private string capchaValue = string.Empty;

        public string LabelKey => kLabelKey;
        public string OkKey => kOkKey;
        public string CancelKey => kCancelKey;


        public ParentGatePopupModel()
        {
            random = new Random();
        }

        public string GetCapchaKey()
        {
            if (capchaValue.IsEmpty())
            {
                var randomValue = random.Next(kMinCapchaValue, kMaxCapchaValue + 1);
                capchaValue = randomValue.ToString();
            }
            return capchaValue;
        }
    }
}


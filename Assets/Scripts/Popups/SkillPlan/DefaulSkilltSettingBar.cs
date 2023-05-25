using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public class DefaulSkilltSettingBar : BaseSkillSettingView
    {
        private const int kActiveImageIndex = 0;
        private const int kInactiveImageIndex = 1;

        [SerializeField] private Image _sliderBaseImage;
        [SerializeField] private Sprite[] _sliderStatusImages;

        protected override void DoOnToggleChanged(bool isActive)
        {
            base.DoOnToggleChanged(isActive);
            _sliderBaseImage.sprite = isActive
                ? _sliderStatusImages[kActiveImageIndex]
                : _sliderStatusImages[kInactiveImageIndex];
        }
    }
}



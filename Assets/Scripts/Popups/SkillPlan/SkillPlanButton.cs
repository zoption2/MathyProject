using UnityEngine;
using Zenject;
using UnityEngine.UI;

namespace Mathy.UI.Helpers
{
    public class SkillPlanButton : MonoBehaviour
    {
        [Inject] private ISkillPlanPopupMediator _mediator;
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        public void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            _mediator.CreatePopup();
        }
    }
}



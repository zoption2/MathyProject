using UnityEngine;
using Zenject;
using Mathy.Services;

namespace Mathy
{
    public class ParentGateEnter : MonoBehaviour
    {
        [Inject] private IParentGateService _service;

        private void Start ()
        {
#if UNITY_IOS || UNITY_EDITOR
            _= _service.CheckAccess();
#endif
        }


        [ContextMenu("ResetParentGate")]
        private void ResetParentGate()
        {
            if (PlayerPrefs.GetInt("ParentGateAccess", 0) == 1)
            {
                PlayerPrefs.DeleteKey("ParentGateAccess");
            }
        }
    }
}


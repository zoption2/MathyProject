using UnityEngine;
using Zenject;
using Mathy.Services;

namespace Mathy
{
    public class ParentGateEnter : MonoBehaviour
    {
        [Inject] private DiContainer _container;
        private IParentGateService _service;
        private IDataService _dataService;

        private void Start ()
        {
            _service = _container.Resolve<IParentGateService>();
            _dataService = _container.Resolve<IDataService>();
#if UNITY_IOS || UNITY_EDITOR
            _= _service.CheckAccess();
#endif
        }


        [ContextMenu("ResetParentGate")]
        private async void ResetParentGate()
        {
            var value = await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.ParentGate);
            if (value == 1)
            {
                await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.ParentGate, 0);
            }
        }
    }
}


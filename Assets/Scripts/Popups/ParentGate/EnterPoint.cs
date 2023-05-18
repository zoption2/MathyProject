﻿using UnityEngine;
using Zenject;
using Mathy.Services;
using Mathy.UI;
using Cysharp.Threading.Tasks;

namespace Mathy
{
    public class EnterPoint : MonoBehaviour
    {
        [SerializeField] private SkillsPanel _skillPanel;
        [SerializeField] private SubscriptionScreen _subscriptionScreen;

        [Inject] private DiContainer _container;
        private IAccountService _accountService;
        private IDataService _dataService;

        private async void Start ()
        {
            _accountService = _container.Resolve<IAccountService>();
            _dataService = _container.Resolve<IDataService>();

            _accountService.SetSkillPlanStub(_skillPanel);
            _accountService.SetSubscriptionScreenStub(_subscriptionScreen);

            await UniTask.Delay(500);
            await _accountService.CheckAllAsync();
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


using Cysharp.Threading.Tasks;
using Mathy.UI;
using System.Diagnostics;

namespace Mathy.Services
{
    public interface IAccountService
    {
        UniTask CheckAllAsync();
        UniTask CheckPlayerNameAsync();
        UniTask CheckSkillPanelAsync();
        UniTask CheckSubscriptionAsync();

        void SetSubscriptionScreenStub(ISubscriptionPopupStub panel);
    }

    public class AccountService : IAccountService
    {
        private const string kCheckSkillPlan = "SkillPlanControl";
        private const string kCheckNameKey = "UserNameControl";

        private readonly IDataService _dataService;
        private readonly IEnterNamePopupMediator _enterNamePopup;
        private readonly ISkillPlanPopupMediator _skillPlanMediator;
        private readonly IParentGateService _parentGateService;

        private ISubscriptionPopupStub _subscriptionPopup;

        public AccountService(IDataService dataService
            , IEnterNamePopupMediator enterNamePopup
            , ISkillPlanPopupMediator skillPlanPopup
            , IParentGateService parentGateService)
        {
            _dataService = dataService;
            _enterNamePopup = enterNamePopup;
            _skillPlanMediator = skillPlanPopup;
            _parentGateService = parentGateService;
        }

        public void SetSubscriptionScreenStub(ISubscriptionPopupStub panel)
        {
            _subscriptionPopup = panel;
        }

        public async UniTask CheckAllAsync()
        {
#if UNITY_IOS || UNITY_EDITOR
            await CheckParentGate();
            await CheckSubscriptionAsync();
#endif
            await CheckPlayerNameAsync();
            await CheckSkillPanelAsync();
        }

        public async UniTask CheckParentGate()
        {
            UnityEngine.Debug.Log("Entered to " + nameof(CheckParentGate));
            await _parentGateService.CheckAccess();
            UnityEngine.Debug.Log("Exit from " + nameof(CheckParentGate));
        }

        public async UniTask CheckPlayerNameAsync()
        {
            UnityEngine.Debug.Log("Entered to " + nameof(CheckPlayerNameAsync));
            var tcs = new UniTaskCompletionSource();
            var isNameChecked = await _dataService.KeyValueStorage.GetIntValueAsync(kCheckNameKey);
            if (isNameChecked == 0)
            {
                _enterNamePopup.CreatePopup();
                _enterNamePopup.ON_COMPLETE += OnNameChoosed;
            }
            else
            {
                tcs.TrySetResult();
            }

            async void OnNameChoosed(string name)
            {
                _enterNamePopup.ON_COMPLETE -= OnNameChoosed;
                await _dataService.PlayerData.Account.SetPlayerName(name);
                await _dataService.KeyValueStorage.SaveIntValueAsync(kCheckNameKey, 1);
                _enterNamePopup.ClosePopup();
                tcs.TrySetResult();
            }

            await tcs.Task;
            UnityEngine.Debug.Log("Exit from " + nameof(CheckPlayerNameAsync));
        }

        public async UniTask CheckSkillPanelAsync()
        {
            UnityEngine.Debug.Log("Entered to " + nameof(CheckSkillPanelAsync));
            var tcs = new UniTaskCompletionSource();
            var isChecked = await _dataService.KeyValueStorage.GetIntValueAsync(kCheckSkillPlan);
            if (isChecked == 0)
            {
                _skillPlanMediator.CreatePopup();
                _skillPlanMediator.ON_CLOSE_CLICK += OnClose;
            }
            else
            {
                tcs.TrySetResult();
            }

            async void OnClose()
            {
                _skillPlanMediator.ON_CLOSE_CLICK -= OnClose;
                await _dataService.KeyValueStorage.SaveIntValueAsync(kCheckSkillPlan, 1);
                tcs.TrySetResult();
            }

            await tcs.Task;
            UnityEngine.Debug.Log("Exit from " + nameof(CheckSkillPanelAsync));
        }

        public async UniTask CheckSubscriptionAsync()
        {
            UnityEngine.Debug.Log("Entered to " + nameof(CheckSubscriptionAsync));
            await _subscriptionPopup.CheckSubscription();
            UnityEngine.Debug.Log("Exit from " + nameof(CheckSubscriptionAsync));
        }
    }
}


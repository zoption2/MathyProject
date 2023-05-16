using Cysharp.Threading.Tasks;
using Mathy.UI;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Text;


namespace Mathy.Services
{
    public interface IParentGateService
    {
        public UniTask CheckAccess();
        public void Complete();
        public void Cancel();
    }

    public class ParentGateService : IParentGateService
    {
        private const string isSubscribedKey = "isSubscriptionBought";

        private UniTaskCompletionSource tcs = new();
        private CancellationTokenSource cancelTokenSource = new();
        private IDataService _dataService;
        private IParentGatePopupMediator _mediator;

        public ParentGateService(IDataService dataService, IParentGatePopupMediator mediator)
        {
            _dataService = dataService;
            _mediator = mediator;
        }

        public async UniTask CheckAccess()
        {
            var value = await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.ParentGate);
            var isSub = PlayerPrefs.GetInt(isSubscribedKey, 0);
            if (value == 1 && isSub == 1)
            {
                tcs.TrySetResult();
            }
            else
            {
                _mediator.Show(null);
                _mediator.ON_COMPLETE += Complete;
                _mediator.ON_CANCEL += Cancel;
            }

            await tcs.Task;
        }

        public async void Complete()
        {
            _mediator.ON_COMPLETE -= Complete;
            _mediator.Close();
            await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.ParentGate, 1);
            tcs.TrySetResult();
        }

        public void Cancel()
        {
            _mediator.ON_CANCEL -= Cancel;
            cancelTokenSource.Cancel();
            _mediator.Close();
            Application.Quit();
        }
    }






    public class AskToBuyListener //: IAppleExtensionsListener
    {
        private readonly IStoreController storeController;
        private readonly IExtensionProvider extensionProvider;

        public AskToBuyListener(IStoreController storeController, IExtensionProvider extensionProvider)
        {
            this.storeController = storeController;
            this.extensionProvider = extensionProvider;
        }

        public void OnPurchaseDeferred(Product product)
        {
            // Get the receipt from the product
            var receipt = GetReceipt(product);

            // Validate the receipt with the App Store
            ValidateReceipt(receipt).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError($"Failed to validate receipt: {t.Exception}");
                    return;
                }

                // Check if the receipt is for a parental approval purchase
                var receiptData = t.Result;
                var isAskToBuy = receiptData.TryGetValue("is-retryable", out var value) && value.ToString() == "1";

                if (isAskToBuy)
                {
                    // Prompt the user to get parental approval
                    PromptForParentalApproval(product);
                }
            });
        }

        private string GetReceipt(Product product)
        {
            var receipt = (product.receipt != null) ? product.receipt : string.Empty;
            var resultBytes = Convert.FromBase64String(receipt);
            var result = Encoding.UTF8.GetString(resultBytes);
            return result;
        }

        private async Task<Dictionary<string, object>> ValidateReceipt(string receipt)
        {
            throw new NotImplementedException();
        }

        private void PromptForParentalApproval(Product product)
        {
            // Show a message to the user to inform them that parental approval is required
            Debug.Log($"Product {product.definition.id} requires parental approval");

            // Use the IAppleExtensions.GetTransactionReceipt method to get the original transaction receipt
            extensionProvider.GetExtension<IAppleExtensions>().GetTransactionReceiptForProduct(product);
        }
    }

    public class MyStoreListener : IStoreListener
    {
        private const string googleFullVersionProductId = "full_version_upgrade";
        private const string appleSubscriptionProductId = "five.systems.development.Mathy.year.sub";
        private const string adsRemovedKey = "adsRemoved";
        private const string isSubscribedKey = "isSubscriptionBought";

        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        private void InitializePurchasing()
        {
            if (IsInitialized())
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct(googleFullVersionProductId, ProductType.NonConsumable);
            builder.AddProduct(appleSubscriptionProductId, ProductType.Subscription);

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;

            // Register the AskToBuyListener listener
            var askToBuyListener = new AskToBuyListener(storeController, extensionProvider);
            extensionProvider.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(askToBuyListener.OnPurchaseDeferred);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            throw new NotImplementedException();
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new NotImplementedException();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            throw new NotImplementedException();
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            throw new NotImplementedException();
        }

        private bool IsInitialized()
        {
            return storeController != null && extensionProvider != null;
        }
    }

}


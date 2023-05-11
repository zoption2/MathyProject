using Cysharp.Threading.Tasks;
using Mathy.UI;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using System.Text;
using Zenject;

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
        private const string kAccessKey = "ParentGateAccess";
        private DiContainer container;
        private UniTaskCompletionSource tcs = new();
        private CancellationTokenSource cancelTokenSource = new();
        private IAddressableRefsHolder refsHolder;
        private ParentGatePopupController controller;

        public ParentGateService(DiContainer container, IAddressableRefsHolder refsHolder)
        {
            this.container = container;
            this.refsHolder = refsHolder;
        }

        public async UniTask CheckAccess()
        {
            //tcs.TrySetCanceled(cancelTokenSource.Token);

            if(PlayerPrefs.GetInt(kAccessKey, 0) == 1)
            {
                tcs.TrySetResult();
            }
            else
            {
                var model = new ParentGatePopupModel();
                var view = await refsHolder.PopupsProvider.InstantiateFromReference<IParentGatePopupView>(Popups.ParentGate, null);
                controller = container.Resolve<ParentGatePopupController>();
                await controller.Init(model, view);
            }

            await tcs.Task;
        }

        public void Complete()
        {
            PlayerPrefs.SetInt(kAccessKey, 1);
            controller.Hide(() =>
            {
                controller.Release();
            });
            tcs.TrySetResult();
        }

        public void Cancel()
        {
            cancelTokenSource.Cancel();
            controller.Release();
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


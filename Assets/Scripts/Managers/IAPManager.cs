using UnityEngine.Purchasing.Security;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Mathy.Core;
using Mathy.UI;
using System;
using TMPro;
#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public class IAPManager : StaticInstance<IAPManager>, IStoreListener
{
    public event Action ON_PURCHASE_COMPLETE;
    public event Action ON_PURCHASE_RESTORED;
    #region FIELDS

    [Header("BUTTONS:")]
    [SerializeField] private TweenedButton noAdsButton;
    [SerializeField] private TweenedButton noAdsSettingsButton;
    [SerializeField] private TweenedButton subscribeButton;
    [SerializeField] private TweenedButton restoreButton;

    //[Header("PANELS:")]
    //[SerializeField] private GameObject subscriptionPanel;

    [Header("DEBUG:")]
    public TMP_Text DebugText;
    [SerializeField] string connectionCheckAddress = "https://www.fivesysdev.com/";

    // Product IDs and static keys
    private const string googleFullVersionProductId = "full_version_upgrade";
    private const string appleSubscriptionProductId = "five.systems.development.Mathy.year.sub";
    private const string adsRemovedKey = "adsRemoved";
    private const string isSubscribedKey = "isSubscriptionBought";

    // Unity IAP
    private static IStoreController storeController;          // The Unity Purchasing system.
    private static IExtensionProvider storeExtensionProvider; // The store-specific Purchasing subsystems.
    private IAppleExtensions appleExtensions;
    //private IGooglePlayStoreExtensions googleExtensions;

    private bool isSubscribed()
    {
        bool isSubscribed;

        var subscriptionProduct = storeController.products.WithID(appleSubscriptionProductId);

        try
        {
            isSubscribed = IsSubscribedTo(subscriptionProduct);
            return isSubscribed;
        }
        catch (StoreSubscriptionInfoNotSupportedException)
        {
            var receipt = (Dictionary<string, object>)MiniJson.JsonDecode(subscriptionProduct.receipt);
            var store = receipt["Store"];
            Log(
                "Couldn't retrieve subscription information because your current store is not supported.\n" +
                $"Your store: \"{store}\"\n\n" +
                "You must use the App Store, Google Play Store or Amazon Store to be able to retrieve subscription information.\n\n" +
                "For more information, see README.md");
            isSubscribed = PlayerPrefs.GetInt(isSubscribedKey, 0).ToBool();
            return isSubscribed;
        }
    }

    public bool HasSubscription()
    {
        bool isConnection = CheckInternetConnection();
        if (isConnection)
        {
            Log("INTERNET CONNECTION IS AVAILABLE");
            Log("CHECKING SUBSCRIPTION STATUS ONLINE CONNECTING THE STORE");
            return isSubscribed();
        }
        else
        {
            Log("INTERNET CONNECTION IS NOT AVAILABLE");
            Log("CHECKING SUBSCRIPTION STATUS LOCAL USING PLAYER PREFS");
            return PlayerPrefs.GetInt(isSubscribedKey, 0).ToBool();
        }
    }

    public async UniTask<bool> IsSubscribed()
    {
        if (await IsInternetConnectedAsync())
        {
            Log("INTERNET CONNECTION IS AVAILABLE");
            Log("CHECKING SUBSCRIPTION STATUS ONLINE CONNECTING THE STORE");
            return isSubscribed();
        }
        else
        {
            Log("INTERNET CONNECTION IS NOT AVAILABLE");
            Log("CHECKING SUBSCRIPTION STATUS LOCAL USING PLAYER PREFS");
            return PlayerPrefs.GetInt(isSubscribedKey, 0).ToBool();
        }
    }

    private SubscriptionManager subManager;
    private SubscriptionInfo appleSubscriptionInfo;

    #endregion

    #region MONO AND INITIALIZATION

    //protected override void Awake()
    //{
    //    base.Awake();
    //    if (storeController == null)
    //    {
    //        // Begin to configure our connection to Purchasing, can use button click instead
    //        TryInitializeStoreManager();
    //    }
    //}

    private async void Start()
    {
        DebugText = LoadingManager.Instance.DebugInApp;

        // If we haven't set up the Unity Purchasing reference
        //if (storeController == null)
        //{
        //    // Begin to configure our connection to Purchasing, can use button click instead
        //    InitializePurchasing();
        //}
        await TryInitializeStoreManager();
        CheckForFullVersion();

        // Display GUI Elements depending on the platform
        UpdateGUIOnPlatform();
        SetButtonEvents();
    }

    private async UniTask TryInitializeStoreManager()
    {
        int attempts = 1;
        for (int i = 0; i < 6; i++)
        {
            InitializePurchasing();
            await UniTask.Delay(1000);
            if (IsPurchasingInitialized())
            {
                Debug.LogFormat("Store Controller initialized with {0} attempt!", attempts);
                return;
            }
            attempts++;
        }
        Debug.LogFormat("Store Controller was not initialized with 5 attempts!");
    }

    private void InitializePurchasing()
    {
        if (IsPurchasingInitialized())
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(googleFullVersionProductId, ProductType.NonConsumable);
        builder.AddProduct(appleSubscriptionProductId, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsPurchasingInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    private bool CheckInternetConnection()
    {
        //Output the network reachability to the console window

        //Check if the device cannot reach the internet
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Change the Text
            Log("Internet : Not Reachable.");
            return false;
        }
        //Check if the device can reach the internet via a carrier data network
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            Log("Internet : Reachable via carrier data network.");
            return true;
        }
        //Check if the device can reach the internet via a LAN
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            Log("Internet : Reachable via Local Area Network.");
            return true;
        }
        return false;
    }

    //private async Task<bool>CheckInternetConnection()
    //{
    //    const string echoServer = "http://google.com";
    //    bool connectionResult;
    //    using (var request = UnityWebRequest.Head(echoServer))
    //    {
    //        request.timeout = 10;
    //        await new Task(() => request.SendWebRequest());
    //        connectionResult = request.result != UnityWebRequest.Result.ConnectionError && 
    //            request.result != UnityWebRequest.Result.ProtocolError && 
    //            request.responseCode == 200;
    //    }
    //    Log("Unity Web Connection result is " +
    //        (connectionResult ? "SUCCESS!" : "FAILED!"));
    //    return connectionResult;        
    //}

    private void SetButtonEvents()
    {
        noAdsButton.Button.onClick.AddListener(BuyFullVersion);
        noAdsSettingsButton.Button.onClick.AddListener(BuyFullVersion);
        subscribeButton.Button.onClick.AddListener(BuySubscription);
        restoreButton.Button.onClick.AddListener(RestorePurchases);
    }

    private void UpdateGUIOnPlatform()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        UpdateRemoveAdsButton();
#endif
#if UNITY_IOS
        noAdsButton.gameObject.SetActive(false);
        noAdsSettingsButton.gameObject.SetActive(false);
#endif
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;
        appleExtensions = extensions.GetExtension<IAppleExtensions>();
        //googleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

        Dictionary<string, string> dict = appleExtensions.GetIntroductoryPriceDictionary();

        foreach (Product item in controller.products.all)
        {
            if (item.receipt != null)
            {
                string intro_json = (dict == null || !dict.ContainsKey(item.definition.storeSpecificId)) ? null : dict[item.definition.storeSpecificId];

                if (item.definition.type == ProductType.Subscription)
                {
                    subManager = new SubscriptionManager(item, intro_json);
                    SubscriptionInfo info = subManager.getSubscriptionInfo();
                    Log("SubInfo: " + info.getProductId().ToString());
                    Log("isSubscribed: " + info.isSubscribed().ToString());
                    Log("isFreeTrial: " + info.isFreeTrial().ToString());

                    if (item.definition.id == appleSubscriptionProductId)
                        appleSubscriptionInfo = info;
                }
            }
        }

        Log("[In-App Purchase Manager] >>>>>> OnInitialized: SUCCESSFUL");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Log("[In-App Purchase Manager] >>>>>> OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Log("[In-App Purchase Manager] >>>>>> OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void ResetToDefault()
    {
        PlayerPrefs.SetInt(adsRemovedKey, 0);
        PlayerPrefs.SetInt(isSubscribedKey, 0);
    }

    #endregion

    #region HELPERS

    public void Log(string debug)
    {
        Debug.Log(debug);
        if (DebugText != null)
        {
            DebugText.text += "\r\n" + debug;
        }
        else
        {
            Debug.LogWarning("Need to set TMP_Text component as debugText to display logging text on screen!");
        }
    }

    public async Task<bool> IsInternetConnectedAsync()
    {
        WWW request = new WWW(connectionCheckAddress);
        float elapsedTime = 0f;

        //check for 5 seconds, return fail after 5 seconds
        while (!request.isDone)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 5f && request.progress <= 0.5f)
                break;
            await Task.Delay(1);
        }

        if (!request.isDone || !string.IsNullOrEmpty(request.error))
            return false;
        else
            return true;
    }

    #endregion

    #region UNITY IAP (TRANSACTION CONTROL)

    // Automatically Called by Unity IAP when
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var validPurchase = true;
#if !UNITY_EDITOR && RECEIPT_VALIDATION
        try
        {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            var result = validator.Validate(args.purchasedProduct.receipt);
            Log("Validate = " + result.ToString());

            foreach (IPurchaseReceipt productReceipt in result)
            {
                Log("Valid receipt for " + productReceipt.productID.ToString());
                //Debug.Log($"Purchase Complete - Product: {productReceipt.productID}");
            }
        }
        catch (IAPSecurityException e)
        {
            Log("Error is " + e.Message.ToString());
            validPurchase = false;
        }
#endif

        Log(string.Format("ProcessPurchase: " + args.purchasedProduct.definition.id));
        OnPurchaseCompleted(args.purchasedProduct);
        // We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    private void BuyProductID(string productId)
    {
        if (IsPurchasingInitialized())
        {
            Product product = storeController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Log(string.Format("Purchasing product:" + product.definition.id.ToString()));
                storeController.InitiatePurchase(product);
            }
            else
            {
                Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
            if (result)
            {
                Log("Restore purchases succeeded!");
                ON_PURCHASE_RESTORED?.Invoke();
                //_ = UpdateGUIAsync();
            }
            else
            {
                Log("Restore purchases failed!");
            }
        });
    }

    private async UniTask UpdateGUIAsync()
    {
        SubscriptionScreen.Instance.SetGFXActive(!await IsSubscribed());
    }

#endregion

#region REMOVE ADS

    public void BuyFullVersion()
    {
        BuyProductID(googleFullVersionProductId);
    }

    public void OnPurchaseCompleted(Product product)
    {
        Log("OnPurchaseCompleted called!");
        switch (product.definition.id)
        {
            case googleFullVersionProductId:
                OnRemoveAdsBought();
                break;
            case appleSubscriptionProductId:
                OnSubscriptionBought();
                break;
        }
    }

    private void UpdateRemoveAdsButton()
    {
        noAdsButton.IsInteractable = !IsAdsRemoved();
        noAdsSettingsButton.IsInteractable = !IsAdsRemoved();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Log($"Purchase failed - Product: '{product.definition.id}', " +
            $"PurchaseFailureReason: {failureReason}");
    }

    private void OnRemoveAdsBought()
    {
        Log("Ads Removed!");
        PlayerPrefs.SetInt(adsRemovedKey, 1);
        UpdateRemoveAdsButton();
    }

    public bool IsAdsRemoved()
    {
        bool isAdsRemoved = PlayerPrefs.GetInt(adsRemovedKey, 0).ToBool();
        Debug.LogFormat("Check #2 : IS ADS SHOULD BE SHOWING? - >>{0}<<", !isAdsRemoved);
        return isAdsRemoved;
    }

    private void CheckForFullVersion()
    {
#if UNITY_ANDROID
        try
        {
            if (IsPurchasingInitialized())
            {
                Product product = storeController.products.WithID(googleFullVersionProductId);
                if (product != null && product.hasReceipt)
                {
                    PlayerPrefs.SetInt(adsRemovedKey, 1);
                    Debug.LogFormat("Check #1 : FULL ACCESS HAS BEEN BOUGHT! ADS WILL NOT BE SHOWING");
                }
                else
                {
                    PlayerPrefs.SetInt(adsRemovedKey, 0);
                    Debug.LogFormat("Check #1 : FULL ACCESS HAS NOT BEEN BOUGHT! ADS WILL BE SHOWING");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
#endif
    }

    #endregion

    #region SUBSCRIPTION

    public void BuySubscription()
    {
        BuyProductID(appleSubscriptionProductId);
    }

    private void OnSubscriptionBought()
    {
        PlayerPrefs.SetInt(isSubscribedKey, 1);
        Log($"isSubscribed PlayerPrefs = {PlayerPrefs.GetInt(isSubscribedKey, 0).ToBool()}");
        //SubscriptionScreen.Instance.
        //    SetGFXActive(!PlayerPrefs.GetInt(isSubscribedKey, 0).ToBool());
        Log("Subscription has been BOUGHT!");
        Log("You are SUBSCRIBED");
        ON_PURCHASE_COMPLETE?.Invoke();
    }

    private bool IsSubscribedTo(Product subscription)
    {
        // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
        if (subscription.receipt == null)
        {
            return false;
        }

        //The intro_json parameter is optional and is only used for the App Store to get introductory information.
        var subscriptionManager = new SubscriptionManager(subscription, null);

        // The SubscriptionInfo contains all of the information about the subscription.
        // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
        var info = subscriptionManager.getSubscriptionInfo();

        return info.isSubscribed() == Result.True;
    }

    public string GetSubscriptionPrice()
    {
        string price;
        var product = storeController.products.WithID(appleSubscriptionProductId);

        if (product != null)
        {
            price = product.metadata.localizedPriceString;
        }
        else
        {
            price = "0*";
        }
        return price;
    }

    public string GetSubscriptionFreeTrialPeriod()
    {
        return "7";
    }

#endregion
}
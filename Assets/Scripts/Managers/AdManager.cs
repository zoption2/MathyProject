using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using Zenject;
using Mathy.Services;

public class AdManager : PersistentSingleton<AdManager>
{
    [Inject] private IDataService _dataService;
    #region FIELDS
    //
    private BannerView bannerView;
    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;
    private List<RewardedAd> rewardedAdPool;
    private List<InterstitialAd> interstitialAdPool;

    public bool IsInterstitialReady => interstitialAd != null && interstitialAd.IsLoaded();

    [Header("Ad IDs:")]

    [SerializeField] string bannerAdId = "ca-app-pub-9340983276950968/7743235138";
    private string testBannerAdId = "ca-app-pub-3940256099942544/6300978111";

    [SerializeField] string interstitialAdId = "ca-app-pub-9340983276950968/7918688478";
    private string testInterstitialAdId = "ca-app-pub-3940256099942544/1033173712";

    [SerializeField] string rewardedAdId = "ca-app-pub-9340983276950968/3664103918";
    private string testRewardedAdId = "ca-app-pub-3940256099942544/5224354917";

    [SerializeField] private TextMeshProUGUI debugTextLable;

    [Header("Config:")]
    [SerializeField] private int preloadedAdsAmount = 1;
    [SerializeField] private bool isTesting = true;

    [Header("Ad Status Events:")]

    public UnityEvent OnAdLoadedEvent;
    public UnityEvent OnAdFailedToLoadEvent;
    public UnityEvent OnAdOpeningEvent;
    public UnityEvent OnAdFailedToShowEvent;
    public UnityEvent OnUserEarnedRewardEvent;
    public UnityEvent OnAdClosedEvent;

    private int rewarValue;

    #endregion

    #region INITIALIZATION

    #if UNITY_ANDROID

    private void Start()
    {
        MobileAds.Initialize(initStatus => 
        {
            PreloadAds();
        });
    }

    #endif

    private void PreloadAds()
    {
        AddDebugText("Start Preloading Ads");
        for (int i = 0; i < preloadedAdsAmount; i++)
        {
            RequestInterstitialAd();
            RequestRewardedAd();
        }
        AddDebugText("All Ads have been Preloaded");
    }

    #endregion

    #region HELPERS

    private void AddDebugText(string textToAdd)
    {
        string newText = debugTextLable.text + "\n" + textToAdd;
        debugTextLable.text = newText;
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .AddKeyword("unity-admob-sample")
            .Build();
    }

    public void ShowAdWithProbability(Action ShowAdCallback, int probability = 1)
    {
    #if UNITY_ANDROID || UNITY_EDITOR
        if (!IAPManager.Instance.IsAdsRemoved())
        {
            System.Random random = new System.Random();
            float rnd = random.Next(1, 101);
            Debug.LogFormat("ADS RANDOMIZER! RANDOME VALUE = >>{0}<< SHOULD BE LESS THEN PROBABILITY : {1}", rnd, probability);

            if (rnd <= probability)
            {
                ShowAdCallback?.Invoke();
                Debug.Log("ADS WAS CALLED!");
            }
        }
        else
        {
            Debug.Log("Full Version has been purchased!");
        }
    #endif
    }

    #endregion

    #region INTERSTITIAL ADS

    private void RequestInterstitialAd()
    {
        

        // Clean up interstitial before using it
        //DestroyInterstitialAd();

        if (interstitialAd != null)
        {
            interstitialAd.OnAdLoaded -= (sender, args) => OnAdLoadedEvent.Invoke();
            interstitialAd.OnAdOpening -= (sender, args) => OnAdOpeningEvent.Invoke();
            interstitialAd.OnAdClosed -= (sender, args) => OnAdClosedEvent.Invoke();
            interstitialAd.OnAdFailedToLoad -= Interstitial_OnAdFailedToLoad;
            interstitialAd.OnAdFailedToShow -= Interstitial_OnAdFailedToShow;
            interstitialAd.Destroy();
        }

        //string adUnitId = GetInterstitialKey();
        string adUnitId = interstitialAdId;
        interstitialAd = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
        

        // Add Event Handlers
        interstitialAd.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
        interstitialAd.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
        interstitialAd.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();
        interstitialAd.OnAdFailedToLoad += Interstitial_OnAdFailedToLoad;
        interstitialAd.OnAdFailedToShow += Interstitial_OnAdFailedToShow;

        // Load an interstitial ad
        //interstitialAd.LoadAd(CreateAdRequest());

        // Wait until an interstitial ad will be loaded and add it to the pool
        //_ = AsyncPoolInterstitialAd(interstitialAd);

        if (interstitialAdPool == null)
        {
            interstitialAdPool = new List<InterstitialAd>();
        }

        interstitialAdPool.Add(interstitialAd);
    }

    private void Interstitial_OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        Debug.LogError("ADS SHOWING FAILED");
        OnAdFailedToShowEvent.Invoke();
    }

    private void Interstitial_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.LogError("ADS LOADING FAILED");

        LoadAdError loadAdError = args.LoadAdError;

        // Gets the domain from which the error came.
        string domain = loadAdError.GetDomain();

        // Gets the error code. See
        // https://developers.google.com/android/reference/com/google/android/gms/ads/AdRequest
        // and https://developers.google.com/admob/ios/api/reference/Enums/GADErrorCode
        // for a list of possible codes.
        int code = loadAdError.GetCode();

        // Gets an error message.
        // For example "Account not approved yet". See
        // https://support.google.com/admob/answer/9905175 for explanations of
        // common errors.
        string message = loadAdError.GetMessage();

        // Gets the cause of the error, if available.
        AdError underlyingError = loadAdError.GetCause();

        // All of this information is available via the error's toString() method.
        Debug.Log("Load error string: " + loadAdError.ToString());

        // Get response information, which may include results of mediation requests.
        ResponseInfo responseInfo = loadAdError.GetResponseInfo();
        Debug.Log("Response info: " + responseInfo.ToString());
        OnAdFailedToLoadEvent.Invoke();
    }

    public void ShowInterstitial()
    {
        interstitialAd.Show();
    }

    public void ShowInterstitialAd()
    {
        RequestInterstitialAd();

        if ((interstitialAdPool != null) && interstitialAdPool.Any())
        {
            interstitialAdPool[0].Show();
            interstitialAdPool.RemoveAt(0);
            RequestInterstitialAd();
        }
        else
        {
            _ = AsyncShowInterstitialAd();
        }
    }

    public async UniTask AsyncShowInterstitialAd()
    {
        await UniTask.WaitUntil(() => interstitialAd.IsLoaded());
        interstitialAd.Show();
        interstitialAdPool.RemoveAt(0);
        RequestInterstitialAd();
    }

    public async UniTask AsyncPoolInterstitialAd(InterstitialAd adToPool)
    {
        await UniTask.WaitUntil(() => adToPool.IsLoaded());

        if (interstitialAdPool == null)
        {
            interstitialAdPool = new List<InterstitialAd>();
        }

        interstitialAdPool.Add(adToPool);
    }

    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    #endregion

    #region REWARDED ADS

    public void RequestRewardedAd()
    {
        string adUnitId = isTesting ? testRewardedAdId : rewardedAdId;

        rewardedAd = new RewardedAd(adUnitId);

        // Add Event Handlers
        rewardedAd.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
        rewardedAd.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
        rewardedAd.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
        rewardedAd.OnAdFailedToShow += (sender, args) => OnAdFailedToShowEvent.Invoke();
        rewardedAd.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();
        rewardedAd.OnUserEarnedReward += (sender, args) => OnUserEarnedRewardEvent.Invoke();

        // Create empty ad request
        rewardedAd.LoadAd(CreateAdRequest());

        // Wait until an ad will be loaded and add it to the pool.
        _ = AsyncPoolRewardedAd(rewardedAd);
    }

    public void ShowRewardedAd()
    {
        RequestRewardedAd();

        if ((rewardedAdPool != null) && rewardedAdPool.Any())
        {
            rewardedAdPool[0].Show();
            rewardedAdPool.RemoveAt(0);
            RequestRewardedAd();
            //Debug.Log("Rewarded Ads count: " + rewardedAdPool.Count);
        }
        else
        {
            _ = AsyncShowRewardedAd();
        }
    }

    public void ShowRewardedAd(int newValue)
    {
        rewarValue = newValue;
        ShowRewardedAd();
    }

    public async void AddReward()
    {
        await _dataService.PlayerData.Progress.AddExperienceAsync(rewarValue);
        //PlayerDataManager.Instance.AddExperience(rewarValue);
    }

    public void RewardedAdFailed()
    {
        Debug.Log("Rewarded ad was closed!");
    }

    public async UniTask AsyncShowRewardedAd()
    {
        await UniTask.WaitUntil(() => rewardedAd.IsLoaded());
        rewardedAd.Show();
        rewardedAdPool.RemoveAt(0);
        RequestRewardedAd();
    }

    public async UniTask AsyncPoolRewardedAd(RewardedAd adToPool)
    {
        await UniTask.WaitUntil(() => adToPool.IsLoaded());

        if (rewardedAdPool == null)
        {
            rewardedAdPool = new List<RewardedAd>();
        }

        rewardedAdPool.Add(adToPool);
        AddDebugText("Rewarded ads loaded: " + rewardedAdPool.Count);
    }

    public void DestroyRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }

    #endregion

}

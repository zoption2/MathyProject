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
    [Inject] private IPlayerDataService _playerDataService;
    #region FIELDS
    //
    private BannerView bannerView;
    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;
    private List<RewardedAd> rewardedAdPool;
    private List<InterstitialAd> interstitialAdPool;    

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
        MobileAds.Initialize(initStatus => { });
        PreloadAds();
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
    #if UNITY_ANDROID
        if (!IAPManager.Instance.IsAdsRemoved())
        {
            System.Random random = new System.Random();
            float rnd = random.Next(1, 101);

            if (rnd <= probability)
            {
                ShowAdCallback?.Invoke();
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
        string adUnitId = isTesting ? testInterstitialAdId : interstitialAdId;

        // Clean up interstitial before using it
        //DestroyInterstitialAd();
        interstitialAd = new InterstitialAd(adUnitId);

        // Add Event Handlers
        interstitialAd.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
        interstitialAd.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
        interstitialAd.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
        interstitialAd.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();

        // Load an interstitial ad
        interstitialAd.LoadAd(CreateAdRequest());

        // Wait until an interstitial ad will be loaded and add it to the pool
        _ = AsyncPoolInterstitialAd(interstitialAd);
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
        await _playerDataService.Progress.AddExperienceAsync(rewarValue);
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

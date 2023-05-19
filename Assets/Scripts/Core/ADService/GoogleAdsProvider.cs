using UnityEngine;
using GoogleMobileAds.Api;
using System;
using Cysharp.Threading.Tasks;

namespace Mathy.Services
{
    public interface IGoogleAdsProvider : IAdsProvider
    {
        event Action ON_INTERSTITIAL_FAILED;
        event Action ON_INTERSTITIAL_CLOSED;
        event Action ON_REWARDED_FAILED;
        event Action ON_REWARDED_CLOSED;
    }


    public class GoogleAdsProvider : IGoogleAdsProvider
    {
        public event Action ON_INTERSTITIAL_FAILED;
        public event Action ON_INTERSTITIAL_CLOSED;
        public event Action ON_REWARDED_FAILED;
        public event Action ON_REWARDED_CLOSED;

        private const string kBannerId = "ca-app-pub-9340983276950968/7743235138";
        private const string kTestBannerId = "ca-app-pub-3940256099942544/6300978111";

        private const string kInterstitialId = "ca-app-pub-9340983276950968/7918688478";
        private const string kTestInterstitialId = "ca-app-pub-3940256099942544/1033173712";

        private const string kRewardedId = "ca-app-pub-9340983276950968/3664103918";
        private const string kTestRewardedId = "ca-app-pub-3940256099942544/5224354917";

        private readonly IDataService _dataService;
        private InterstitialAd interstitial;
        private RewardedAd rewarded;

        public bool IsInterstitialLoaded => interstitial != null && interstitial.IsLoaded();
        public bool IsRewardedLoaded => rewarded != null && rewarded.IsLoaded();


        public void Init()
        {
            MobileAds.Initialize(initStatus =>
            {
                LoadGoogleInterstitialAds();
            });
        }

        public void PrepareInterstitial()
        {
            LoadGoogleInterstitialAds();
        }

        public void PrepareRewarded()
        {
            LoadGoogleRewardedAds();
        }

        public void ShowInterstitial()
        {
            interstitial?.Show();
        }

        public void ShowRewarded()
        {
            rewarded?.Show();
        }

        private void LoadGoogleInterstitialAds()
        {
            if (interstitial != null)
            {
                interstitial.OnAdClosed -= Interstitial_OnAdClosed;
                interstitial.OnAdFailedToLoad -= Interstitial_OnAdFailedToLoad;
                interstitial.OnAdFailedToShow -= Interstitial_OnAdFailedToShow;
                interstitial.Destroy();
            }

            string adUnitId = GetInterstitialKey();
            interstitial = new InterstitialAd(adUnitId);
            AdRequest request = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
            interstitial.LoadAd(request);

            interstitial.OnAdClosed += Interstitial_OnAdClosed;
            interstitial.OnAdFailedToLoad += Interstitial_OnAdFailedToLoad;
            interstitial.OnAdFailedToShow += Interstitial_OnAdFailedToShow;
        }

        private void LoadGoogleRewardedAds()
        {
            if (rewarded != null)
            {
                rewarded.OnAdFailedToLoad -= Rewarded_OnAdFailedToLoad;
                rewarded.OnAdClosed -= Rewarded_OnAdClosed;
                rewarded.OnAdFailedToShow -= Rewarded_OnAdFailedToShow;
                rewarded.OnUserEarnedReward -= Rewarded_OnUserEarnedReward;
                rewarded.Destroy();
            }

            var key = GetRewardedKey();
            rewarded = new RewardedAd(key);
            AdRequest request = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
            rewarded.LoadAd(request);

            rewarded.OnAdFailedToLoad += Rewarded_OnAdFailedToLoad;
            rewarded.OnAdClosed += Rewarded_OnAdClosed;
            rewarded.OnAdFailedToShow += Rewarded_OnAdFailedToShow;
            rewarded.OnUserEarnedReward += Rewarded_OnUserEarnedReward;
        }

        private void Interstitial_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogWarningFormat("Interstitial is failed to load!");
        }

        private void Interstitial_OnAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.LogWarningFormat("Interstitial is failed to show!");
        }

        private void Interstitial_OnAdClosed(object sender, EventArgs e)
        {
            interstitial?.Destroy();
            LoadInterstetialWithDelay(2);
        }

        private void Rewarded_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogWarningFormat("Rewarded is failed to load!");
        }

        private void Rewarded_OnAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.LogWarningFormat("Rewarded is failed to show!");
        }

        private void Rewarded_OnAdClosed(object sender, EventArgs args)
        {
            rewarded?.Destroy();
            LoadRewardedWithDelay(2);
        }

        private void Rewarded_OnUserEarnedReward(object sender, Reward reward)
        {
            Debug.Log("Here user should get reward for RewardedAds! It's NULL for now");
        }

        private async void LoadRewardedWithDelay(int delay = 5)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            LoadGoogleRewardedAds();
        }

        private async void LoadInterstetialWithDelay(int delay = 5)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            LoadGoogleInterstitialAds();
        }

        private string GetInterstitialKey()
        {
#if UNITY_EDITOR
            return kTestInterstitialId;
#else
            return kInterstitialId;
#endif
        }

        private string GetRewardedKey()
        {
#if UNITY_EDITOR
            return kTestRewardedId;
#else
            return kRewardedId;
#endif
        }
    }

}



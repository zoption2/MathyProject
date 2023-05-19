using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Analytics;

namespace Mathy.Services
{
    public interface IAdsService
    {
        bool TryShowInterstitialAds(int probability = 100, Action onSuccess = null, Action onFail = null);
        void ShowRewardedAds(Action onSuccess = null, Action onFail = null);
    }


    public class AdsService : IAdsService
    {
        private readonly IGoogleAdsProvider _googleAdsProvider;
        private readonly Random _random;

        private bool _isGoogleInterstitialReady => _googleAdsProvider.IsInterstitialLoaded;
        private bool _isGoogleRewardedReady => _googleAdsProvider.IsRewardedLoaded;

        public AdsService()
        {
            _random = new Random();
            _googleAdsProvider = new GoogleAdsProvider();
            _googleAdsProvider.Init();
        }

        public bool TryShowInterstitialAds(int probability = 100, Action onSuccess = null, Action onFail = null)
        {
            var canShow = CanShow(probability);
            if (!canShow)
            {
                return false;
            }

            if (_isGoogleInterstitialReady)
            {
                _googleAdsProvider.ON_INTERSTITIAL_FAILED += OnFail;
                _googleAdsProvider.ON_INTERSTITIAL_CLOSED += OnSuccess;
                _googleAdsProvider.ShowInterstitial();
                return true;

                void OnSuccess()
                {
                    _googleAdsProvider.ON_INTERSTITIAL_CLOSED -= OnSuccess;
                    _googleAdsProvider.PrepareInterstitial();
                    onSuccess?.Invoke();
                }

                void OnFail()
                {
                    _googleAdsProvider.ON_INTERSTITIAL_FAILED -= OnFail;
                    _googleAdsProvider.PrepareInterstitial();
                    onFail?.Invoke();
                }
            }
            else
            {
                return false;
            }
        }

        public void ShowRewardedAds(Action onSuccess = null, Action onFail = null)
        {
            if (_isGoogleRewardedReady)
            {
                _googleAdsProvider.ON_REWARDED_FAILED += OnFail;
                _googleAdsProvider.ON_REWARDED_CLOSED += OnSuccess;
                _googleAdsProvider.ShowRewarded();

                void OnSuccess()
                {
                    _googleAdsProvider.ON_REWARDED_CLOSED -= OnSuccess;
                    _googleAdsProvider.PrepareInterstitial();
                    onSuccess?.Invoke();
                }

                void OnFail()
                {
                    _googleAdsProvider.ON_REWARDED_FAILED -= OnFail;
                    _googleAdsProvider.PrepareInterstitial();
                    onFail?.Invoke();
                }
            }
        }

        private bool CanShow(int probability)
        {
            float rnd = _random.Next(1, 101);
            UnityEngine.Debug.LogFormat("ADS RANDOMIZER! RANDOME VALUE = >>{0}<< SHOULD BE LESS THEN PROBABILITY : {1}", rnd, probability);

            if (rnd <= probability)
            {
                UnityEngine.Debug.Log("ADS SHOULD BE CALLED!");
                return true;
            }
            return false;
        }
    }
}



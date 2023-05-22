using System;


namespace Mathy.Services
{
    public interface IAdsService
    {
        void Init();
        bool TryShowInterstitialAds(int probability = 100, Action onSuccess = null, Action onFail = null);
        void ShowRewardedAds(Action onSuccess = null, Action onFail = null);
    }


    public class AdsService : IAdsService
    {
        private IGoogleAdsProvider _googleAdsProvider;
        private Random _random;
        private bool _isInited;

        private bool _isGoogleInterstitialReady => _googleAdsProvider.IsInterstitialLoaded;
        private bool _isGoogleRewardedReady => _googleAdsProvider.IsRewardedLoaded;


        public void Init()
        {
            _random = new Random();
            _googleAdsProvider = new GoogleAdsProvider();
            _googleAdsProvider.Init();
            _isInited = false;
            TryInitInternal();
        }

        public bool TryShowInterstitialAds(int probability = 100, Action onSuccess = null, Action onFail = null)
        {
            TryInitInternal();
            var canShow = CanShow(probability);
            if (!canShow)
            {
                return false;
            }

            if (_isGoogleInterstitialReady)
            {
                UnityEngine.Debug.LogFormat("Google interstitial is available");
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
                UnityEngine.Debug.LogFormat("Google interstitial is not available. Trying to load one more ADS. Need to check one more time.");
                _googleAdsProvider.PrepareInterstitial();
                return false;
            }
        }

        public void ShowRewardedAds(Action onSuccess = null, Action onFail = null)
        {
            TryInitInternal();
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

        private void TryInitInternal()
        {
            UnityEngine.Debug.LogFormat("Will try to init ADS providers:");
            if (!_isInited)
            {
                UnityEngine.Debug.LogFormat("Try to init googleAdsProvider");
                UnityEngine.Debug.LogFormat("Is provider class exist? : {0}", _googleAdsProvider != null);
                UnityEngine.Debug.LogFormat("Is provider class inited? : {0}", _googleAdsProvider.IsProviderInited);
                if (!_googleAdsProvider.IsProviderInited)
                {
                    UnityEngine.Debug.LogFormat("Try to init provider class here");
                    _googleAdsProvider.Init();
                    UnityEngine.Debug.LogFormat("After inialization point");
                }
                _isInited = true;
                UnityEngine.Debug.LogFormat("INTERNAL ADS SERVICE INITED. Till this point Google Ads service initialization should be called");
            }
            UnityEngine.Debug.LogFormat("All ADS providers should be inited;");
        }
    }
}



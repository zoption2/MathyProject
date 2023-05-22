namespace Mathy.Services
{
    public interface IAdsProvider
    {
        bool IsProviderInited { get; }
        bool IsInterstitialLoaded { get; }
        bool IsRewardedLoaded { get; }
        void Init();
        void ShowInterstitial();
        void ShowRewarded();
        void PrepareInterstitial();
        void PrepareRewarded();
    }

}



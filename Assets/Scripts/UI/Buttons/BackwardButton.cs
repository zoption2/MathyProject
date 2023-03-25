using Mathy.Core;
using Mathy.Core.Tasks;
using UnityEngine;

public class BackwardButton : ToolbarButton
{
    #region Fields

    [Header("CONFIG:")]
    [SerializeField] private bool backToMainMenu = false;
    [SerializeField] private bool backToDifficultyPanel = false;
    [SerializeField] private bool showAdOnPress = true;

    #endregion

    protected override void OnTweenComplete()
    {
        base.OnTweenComplete();

        if (backToMainMenu)
        {
            TaskManager.Instance.ResetToDefault();
            GameManager.Instance.ChangeState(GameState.MainMenu);
            if(showAdOnPress) AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 10);
        }

        if(backToDifficultyPanel)
        {
            if (TaskManager.Instance.IsPractice)
            {
                TaskManager.Instance.ShowResult(true);
            }
            else
            {
                TaskManager.Instance.ResetToDefault();
                GameManager.Instance.ChangeState(GameState.MainMenu);
                if (showAdOnPress) AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 10);
            }

        }
    }
}
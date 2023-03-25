using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mathy.Core.Tasks;

public class RestartButton : ToolbarButton
{
    protected override void OnTweenComplete()
    {
        base.OnTweenComplete();

        if (TaskManager.Instance.enabled)
        {
            TaskManager.Instance.RestartTasks();
        }
        /*
        if (TaskManagerNew.Instance.enabled)
        {
            TaskManagerNew.Instance.RestartTasks();
        }
        */

        if (ChallengesManager.Instance.enabled)
        {
            ChallengesManager.Instance.RestartTasks();
        }
        AdManager.Instance.ShowAdWithProbability(AdManager.Instance.ShowInterstitialAd, 30);
    }
}
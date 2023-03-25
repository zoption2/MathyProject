using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyRewardsClaimButton : ButtonFX
{
    public override void OnPress()
    {
        base.OnPress();
        RewardEventManager.SendOnClaimPressed();
    }
}

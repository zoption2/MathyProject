using Mathy.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderButton : ButtonFX
{
    protected override void OnTweenComplete()
    {
        base.OnTweenComplete();

        /*MainMenuScene.Instance.SetActive(true);
        TaskScene.Instance.SetActive(false);*/

        GameManager.Instance.ChangeState(GameState.MainMenu);
    }
}

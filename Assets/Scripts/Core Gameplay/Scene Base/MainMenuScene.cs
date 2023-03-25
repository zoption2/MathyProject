using Mathy.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScene : SceneGFXContainer
{
    private void Awake()
    {
        ScenesManager.Instance.MainMenuScene = this;
    }
}

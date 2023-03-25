using Mathy.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengesScene : SceneGFXContainer
{
    private void Awake()
    {
        ScenesManager.Instance.ChallengesScene = this;
    }
}

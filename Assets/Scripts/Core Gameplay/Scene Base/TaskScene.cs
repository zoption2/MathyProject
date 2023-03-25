using Mathy.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskScene : SceneGFXContainer
{
    private void Awake()
    {
        ScenesManager.Instance.TaskScene = this;
    }
}

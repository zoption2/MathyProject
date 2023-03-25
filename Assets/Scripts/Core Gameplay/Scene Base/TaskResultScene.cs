using Mathy.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskResultScene : SceneGFXContainer
{
    private void Awake()
    {
        ScenesManager.Instance.TaskResultScene = this;
    }
}

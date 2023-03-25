using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBase : StaticInstance<SceneBase>
{
    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGFXContainer : MonoBehaviour
{
    public bool IsActive
    {
        get => gameObject.activeInHierarchy;
        set
        {
            gameObject.SetActive(value);
        }
    }
}

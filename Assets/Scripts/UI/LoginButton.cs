using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoginButton : MonoBehaviour
{
    [SerializeField] private bool isPressed = false;
    [SerializeField] private Vector3 rotationTo;
    public void OnPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            transform.DOShakeScale(1, rotationTo, 20, 90).OnComplete(() => isPressed = false);
        }
        
    }
}

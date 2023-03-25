using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RewardEvent
{
    private static int transportValue;
    public int streak { get => transportValue;  set => transportValue = value; }
    private static int _clickChestId;
    public int clickChestId { get => _clickChestId; set => _clickChestId = value; }
}

public class MyEvent : UnityEvent<RewardEvent>
{
    private int _param;
    public int param { get => _param;  set => _param = value; }
    public int GetValue() => _param;
    public void SetValue(int val) => _param = val;
}

public class BoolEvent
{
    private static bool transportValue;
    public bool value { get => transportValue; set => transportValue = value; }
}
public class RewardEventManager : MonoBehaviour
{
    public static UnityEvent OnStartDailyAwards = new UnityEvent();
    public static UnityEvent OnClaimPressed = new UnityEvent();
    public static UnityEvent<RewardEvent> OnChestPressed = new UnityEvent<RewardEvent>();
    public static UnityEvent OnFinishDailyAwards = new UnityEvent();

    public static void SendOnStartDailyAwards()
    {
        OnStartDailyAwards.Invoke();
    }

    public static void SendOnFinishDailyAwards()
    {
        OnFinishDailyAwards.Invoke();
    }

    public static void SendOnClaimPressed()
    {
        OnClaimPressed.Invoke();
    }

    public static void SendOnChestPressed(RewardEvent ev)
    {
        OnChestPressed.Invoke(ev);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTimer {


    private static List<ActionTimer> activeTimerList;
    private static GameObject initGameObject;

    private static void InitIfNeeded() {
        if (initGameObject == null) {
            initGameObject = new GameObject("ActionTimer_InitGameObject");
            activeTimerList = new List<ActionTimer>();
        }
    }

    public static ActionTimer Create( bool isCountdown, Action OnElapsed, Action OnUpdate, float timer, bool destroyOnElapse, string timerName = null) {
        InitIfNeeded();
        GameObject gameObject = new GameObject("ActionTimer", typeof(MonoBehaviourHook));

        ActionTimer actionTimer = new ActionTimer(isCountdown, OnElapsed, OnUpdate, timer, destroyOnElapse, timerName, gameObject);

        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = actionTimer.Update;

        activeTimerList.Add(actionTimer);

        return actionTimer;
    }

    public static ActionTimer Create(bool isCountdown, Action OnUpdate, string timerName = null)
    {
        InitIfNeeded();
        GameObject gameObject = new GameObject("ActionTimer", typeof(MonoBehaviourHook));

        ActionTimer actionTimer = new ActionTimer(isCountdown, OnUpdate, timerName, gameObject);

        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = actionTimer.Update;

        activeTimerList.Add(actionTimer);

        return actionTimer;
    }

    private static void RemoveTimer(ActionTimer actionTimer) {
        InitIfNeeded();
        activeTimerList.Remove(actionTimer);
    }

    public static void StopTimer(string timerName) {
        for (int i = 0; i < activeTimerList.Count; i++) {
            if (activeTimerList[i].timerName == timerName) {
                // Stop this timer
                activeTimerList[i].DestroySelf();
                i--;
            }
        }
    }



    // Сlass to have access to MonoBehaviour functions
    private class MonoBehaviourHook : MonoBehaviour {
        public Action onUpdate;
        private void Update() {
            if (onUpdate != null) onUpdate();
        }
    }

    private Action OnElapsed;
    private Action OnUpdate;
    public float timer { get; private set; }
    private float timeToElapse;
    private string timerName;
    private GameObject gameObject;
    private bool destroyOnElapse;
    private bool isDestroyed;
    private bool isCountdown;

    private ActionTimer(bool isCountdown, Action OnElapsed, Action OnUpdate, float timer, bool destroyOnElapse, string timerName, GameObject gameObject)
    {
        this.isCountdown = isCountdown;
        this.OnElapsed = OnElapsed;
        this.OnUpdate = OnUpdate;
        this.timer = timer;
        this.destroyOnElapse = destroyOnElapse;
        this.timeToElapse = timer;
        this.timerName = timerName;
        this.gameObject = gameObject;
        isDestroyed = false;
    }

    private ActionTimer(bool isCountdown, Action OnUpdate, string timerName, GameObject gameObject)
    {
        this.isCountdown = isCountdown;
        this.OnElapsed = null;
        this.OnUpdate = OnUpdate;
        this.timer = 0;
        this.destroyOnElapse = false;
        this.timeToElapse = timer;
        this.timerName = timerName;
        this.gameObject = gameObject;
        isDestroyed = false;
    }

    public void Update()
    {
        if (!isDestroyed)
        {
            if (isCountdown) 
            {
                timer -= Time.deltaTime;
                OnUpdate(); // Trigger the OnUpdate action
                if (timer < 0)
                {
                    OnElapsed(); // Trigger the OnElapsed action
                    if (destroyOnElapse) DestroySelf();
                }
            }
            else
            {
                timer += Time.deltaTime;
                OnUpdate();
            }
        }
    }

    public void RestartTimer()
    {
        timer = timeToElapse;
    }

    private void DestroySelf() {
        isDestroyed = true;
        UnityEngine.Object.Destroy(gameObject);
        RemoveTimer(this);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    private ActionTimer challengeTimer;
    public string bestTimeKey { get; set; }

    private float bestTime
    {
        get => PlayerPrefs.GetFloat(bestTimeKey, float.MaxValue);
        set
        {
            PlayerPrefs.SetFloat(bestTimeKey, value);
        }
    }

    private void OnDisable()
    {
        timerText.text = "00:00";
        if (challengeTimer != null)
        {
            ActionTimer.StopTimer("ChallengeTimer");
        }
    }

    public void StartTimer()
    {
        UpdateBestTimeText();
        timerText.text = 0.ToString();
        if (gameObject.activeSelf)
        {
            challengeTimer = ActionTimer.Create(false, DisplayTime, "ChallengeTimer");
        }
    }

    public float StopTimer(bool isComplete)
    {
        if (challengeTimer != null)
        {
            if (isComplete)
            {
                UpdateBestTime(challengeTimer.timer);
                return challengeTimer.timer;
            }
            ActionTimer.StopTimer("ChallengeTimer");
        }
        timerText.text = "00:00";
        return 0f;
    }

    private void UpdateBestTime(float time)
    {
        if(time < bestTime)
        {
            bestTime = time;
        }
        UpdateBestTimeText();
    }

    private void UpdateBestTimeText()
    {
        float time = bestTime < float.MaxValue ? bestTime : 0;
        TimeSpan t = TimeSpan.FromSeconds(time);
        int minutes = t.Minutes;
        float seconds = t.Seconds;
        bestTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DisplayTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(challengeTimer.timer);
        int minutes = t.Minutes;
        float seconds = t.Seconds;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

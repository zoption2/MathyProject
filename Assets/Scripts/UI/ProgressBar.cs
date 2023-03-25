using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using Mathy.Core.Tasks;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerBar;
    [SerializeField] private float maxTime;

    private ActionTimer practiceCountdownTimer;

    private int _numLoops;

    private void OnDisable()
    {
        StopTimer();
    }

    public void StartTimer(float time)
    {
        timerText.text = time.ToString();
        maxTime = time;
        if (gameObject.activeSelf)
        {
            //practiceCountdownTimer = ActionTimer.Create(TimeIsUp, DisplayTime, time, false, "CountdownTimer");
        		//New changes in timer in new version below
			practiceCountdownTimer = ActionTimer.Create(true, TimeIsUp, DisplayTime, time, false, "CountdownTimer");
		}
    }

    public void RestartTimer()
    {
        if (practiceCountdownTimer != null)
        {
            practiceCountdownTimer.RestartTimer();
        }
    }

    public void StopTimer()
    {
        if (practiceCountdownTimer != null)
        {
            ActionTimer.StopTimer("CountdownTimer");
        }
    }

    private void DisplayTime()
    {
        float seconds = practiceCountdownTimer.timer;
        float centisecond = Mathf.FloorToInt(seconds % 1 * 100);
        timerText.text = string.Format("{0:00}:{1:00}", seconds, centisecond);
        timerBar.fillAmount = seconds / maxTime;
    }

    private void TimeIsUp()
    {
        ActionTimer.StopTimer("CountdownTimer");
        Debug.LogError("Tise is up!!");
        TaskManager.Instance.ShowResult(true);
    }
}

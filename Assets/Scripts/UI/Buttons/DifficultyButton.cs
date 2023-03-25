using UnityEngine;
using UnityEngine.Events;
using Mathy.Core.Tasks;

public class DifficultyButton : ButtonFX
{
    [SerializeField] private float timerValue;
    [SerializeField] private int timerMode;
    [SerializeField] private TimerDifficultyMenu timerMenu;

    protected override void OnTweenComplete()
    {
        isPressed = false;
        TaskManager.Instance.SetTimerValue(timerValue, timerMode);
        timerMenu.ClosePanel();
    }
}

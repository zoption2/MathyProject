using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchGradeGroup : MonoBehaviour
{
    public Action<int> ON_GRADE_TAB_SWITCH;
    public Action<int, bool> ON_GRADE_TOGGLE_SWITCH;

    [SerializeField] private int _grade = 1; 
    [SerializeField] private Button _gradeTabButton;
    [SerializeField] private Toggle _gradeToggle;

    public void Enable()
    {
        _gradeTabButton.onClick.AddListener(OnGradeButtonClick);
        _gradeToggle.onValueChanged.AddListener(OnGradeToggleClick);
    }

    public void Disable()
    {
        _gradeTabButton.onClick.RemoveListener(OnGradeButtonClick);
        _gradeToggle.onValueChanged.RemoveListener(OnGradeToggleClick);
    }

    private void OnGradeButtonClick()
    {
        ON_GRADE_TAB_SWITCH?.Invoke(_grade);
    }

    private void OnGradeToggleClick(bool isOn)
    {
        ON_GRADE_TOGGLE_SWITCH?.Invoke(_grade, isOn);
    }
}
}



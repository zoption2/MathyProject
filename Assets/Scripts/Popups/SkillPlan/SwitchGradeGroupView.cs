using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchGradeGroupView : MonoBehaviour
{
    public Action<int> ON_GRADE_TAB_SWITCH;
    public Action<int, bool> ON_GRADE_TOGGLE_SWITCH;

    [SerializeField] private int _grade = 1;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _gradeImage;
    [SerializeField] private Button _gradeTabButton;
    [SerializeField] private Toggle _gradeToggle;
    [SerializeField] private GameObject _blackout;

    public int Grade => _grade;

    public void Init(string title, bool isSelected)
    {
        _title.text = title;
        _blackout.SetActive(!isSelected);
        _gradeToggle.isOn = isSelected;
    }

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

    public void SetActive(bool isActive)
    {
        _blackout.SetActive(!isActive);
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




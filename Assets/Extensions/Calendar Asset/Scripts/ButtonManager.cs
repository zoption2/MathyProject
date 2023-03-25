using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonManager : MonoBehaviour
{
	#region Fields

	[SerializeField] public TextMeshProUGUI label;
	[SerializeField] public GameObject Selector;

	private Button button;
	private UnityAction buttonAction;
	public CalendarDayCell calendarDay { get; set; }

	#endregion

	#region Public Methods

	public void Initialize(string label, Action<(string, string, ButtonManager)> clickEventHandler)
	{
		this.label.text = label;

		buttonAction += () => clickEventHandler((label, label, this));
		button.onClick.AddListener(buttonAction);
	}

	public void Select(bool isActive)
    {
		Selector.SetActive(isActive);
	}

    #endregion

    #region Private Methods

    private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void OnDestroy()
	{
		button.onClick.RemoveListener(buttonAction);
	}

	#endregion
}

using TMPro;
using UnityEngine;

public class TailManager : MonoBehaviour
{
	#region Fields

	[SerializeField] private TextMeshProUGUI legend;
	[SerializeField] private TextMeshProUGUI datePanel;

	#endregion

	#region Public Methods

	public void SetLegend(string text)
	{
		legend.text = text;
	}

	public void SetDate(string text)
	{
		datePanel.text = text;
	}

	#endregion
}

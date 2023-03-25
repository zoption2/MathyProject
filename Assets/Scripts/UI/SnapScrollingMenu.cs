using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class SnapScrollingMenu : MonoBehaviour
{
    #region Fields

    [SerializeField] private SimpleScrollSnap scrollSnap;
	[SerializeField] private TabGroup tabs;
	[SerializeField] private ParalaxBG bg;
    [SerializeField] private DatePanel datePanel;

    #endregion

    #region Methods

    private void Start()
	{
		if(scrollSnap == null) scrollSnap = GetComponent<SimpleScrollSnap>();
        ActivateStartingPanel();
    }

	public void ActivateStartingPanel()
    {
        List<Transform> content = new List<Transform>();
        foreach (Transform child in scrollSnap.Content)
        {
            content.Add(child);
        }

        int startingIndex = scrollSnap.StartingPanel;
        for (int i = 0; i < content.Count; i++)
        {
            content[i].gameObject.SetActive(i == startingIndex);
        }
        SelectTab(startingIndex);
    }

    public void DeactivateUncenteredPanels()
    {
        for (int i = 0; i < scrollSnap.Content.childCount; i++ )
        {
            GameObject panel = scrollSnap.Content.GetChild(i).gameObject;
            panel.SetActive(i == scrollSnap.CenteredPanel);
        }
    }

	public void SelectTab()
    {
        int tabIndex = scrollSnap.CenteredPanel;
        tabs.SelectTab(tabIndex);
        UpdateDisplayStyle(tabIndex);
        DeactivateUncenteredPanels();
        if (tabIndex == 1)
        {
            datePanel.OpenPanel();
        }
        else
        {
            datePanel.ClosePanel();
        }
    }

    private void SelectTab(int tabIndex)
    {
        tabs.SelectTab(tabIndex);
        UpdateDisplayStyle(tabIndex);
    }

    private void UpdateDisplayStyle(int tabIndex)
    {
        bg.MoveBG(tabIndex - 1);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsTabPanel : TabGroup
{
    [SerializeField] private PopupPanel calendarPanel;

    private void OnEnable()
    {
        if (selectedTabIndex == 0) SelectTab(selectedTabIndex);
    }

    private void Start()
    {
        CalendarManager.Instance.SelectToday();
    }
    public override void OnTabSelected(TabPanelButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;
        selectedTab.Select();
        ResetTabs();
        selectedTabIndex = tabButtons.IndexOf(button);
        SwapObjects();
    }

    protected override void SwapObjects()
    {
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            objectsToSwap[i].SetActive(i == selectedTabIndex);
        }
        if (selectedTabIndex == 0)
        {
            CalendarManager.Instance.SelectToday();
        }
    }
}

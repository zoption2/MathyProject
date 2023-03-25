using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeTabPanel : TabGroup
{
    private async void OnEnable()
    {
        await UniTask.Delay(500);
        SelectTab(selectedTabIndex);
    }

    public override void SelectTab(int tabIndex)
    {
        OnTabSelected(tabButtons[tabIndex]);
    }

    public override void OnTabSelected(TabPanelButton button)
    {
        if (selectedTab != null && selectedTab != button)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;
        selectedTab.Select();
        selectedTabIndex = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == selectedTabIndex)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    void ActivateAllChildren(Transform parent, bool isActive)
    {
        foreach (Transform child in parent)
        {
            foreach (Transform button in child)
                button.GetComponent<TaskPracriceButton>().IsActive = isActive;
        }
            
    }
}

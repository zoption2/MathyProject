using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    [SerializeField] protected List<TabPanelButton> tabButtons;
    [SerializeField] protected List<GameObject> objectsToSwap;
    [SerializeField] protected TextMeshProUGUI title;
    [SerializeField] protected int selectedTabIndex;
    protected TabPanelButton selectedTab;
    private string todayTitleText = "Daily Lessons";
    private string practiceTitleText = "Practice";
    private string theoryText = "Theory";

    private void Start()
    {
        LocalizationManager.OnLanguageChanged.AddListener(Localize);
    }

    private void Localize()
    {
        todayTitleText = LocalizationManager.GetLocalizedString("GUI Elements", "Today");
        practiceTitleText = LocalizationManager.GetLocalizedString("GUI Elements", "Practice");
        theoryText = LocalizationManager.GetLocalizedString("GUI Elements", "Theory");
        if (title != null) UpdateTitleText();
    }

    private void UpdateTitleText()
    {
        string titleText;
        switch (selectedTabIndex)
        {
            case 0:
                titleText = practiceTitleText;
                break;
            case 1:
                titleText = todayTitleText;
                break;
            case 2:
                titleText = theoryText;
                break;
            default:
                goto case 1;
        }
        title.text = titleText;
    }

    public void Subscribe(TabPanelButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabPanelButton>();
        }

        if (!tabButtons.Contains(button))
        {
            tabButtons.Add(button);
        }
    }

    public void OnTabEnter(TabPanelButton button)
    {
        ResetTabs();
    }
    public void OnTabExit(TabPanelButton button)
    {
        ResetTabs();
    }

    public virtual void SelectTab(int tabIndex)
    {
        OnTabSelected(tabButtons[tabIndex]);
        if (title != null) UpdateTitleText();
    }

    public virtual void OnTabSelected(TabPanelButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;
        selectedTab.Select();
        ResetTabs();
        selectedTabIndex = button.transform.GetSiblingIndex();
        SwapObjects();
    }

    protected virtual void SwapObjects()
    {
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

    public void ResetTabs()
    {
        /*foreach(TabPanelButton button in tabButtons)
        {
            //Skip over the currently selected tab
            if (selectedTab != null && button == selectedTab)
            {
                continue;
            }
            //button.tabImage.sprite = tabIdle;
        }*/
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeaderPanel : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private List<HeaderBar> headerPanels;
    [SerializeField] private List<GameObject> noAdsButtons;

    #endregion

    #region MONO AND INITIALIZATION

    void Awake()
    {
        UpdateButtonsVisability();
    }

    private void OnEnable()
    {
        SubscribeOnPlayerDataManager(true);
        UpdatePanels();
    }

    private void OnDisable()
    {
        SubscribeOnPlayerDataManager(false);
    }

    private void SubscribeOnPlayerDataManager(bool isSubscribed)
    {
        if (isSubscribed)
        {
            PlayerDataManager.OnPlayerStatsUpdated.AddListener(UpdatePanels);
        }
        else
        {
            PlayerDataManager.OnPlayerStatsUpdated.RemoveListener(UpdatePanels);
        }
    }

    #endregion

    public void UpdatePanels()
    {
        foreach (HeaderBar panel in headerPanels)
        {
            panel.UpdateText();
        }
    }

    public void UpdateButtonsVisability()
    {
        bool isIOS = Application.platform == RuntimePlatform.IPhonePlayer;
        foreach(GameObject button in noAdsButtons)
        {
            button.SetActive(!isIOS);
        }
    }
}
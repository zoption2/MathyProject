using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using Mathy.Data;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public class CalendarPanel : PopupPanel
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private Transform sidePanel;
    [SerializeField] private CanvasGroup visualContainer;
    [SerializeField] private List<Button> viewButtons;
    [SerializeField] private Button viewTimeButtons;
    [SerializeField] private TMP_Text selectedDayLable;
    [SerializeField] private GameObject notPlayedPlaceholder;

    private CalendarManager calendarManager;

    UnityAction viewSMode;
    UnityAction viewMMode;
    UnityAction viewLMode;

    #endregion

    private void Start()
    {
        _ = Initialize();
    }

    protected override void OnEnable()
    {
        OpenPanel();
    }

    private async UniTask Initialize()
    {
        await UniTask.WaitUntil(() => CalendarManager.Instance != null);

        calendarManager = CalendarManager.Instance;
        calendarManager.UpdateCalendar();

        viewSMode = () => { OnViewButtonPressed(0); };
        viewMMode = () => { OnViewButtonPressed(1); };
        viewLMode = () => { OnViewButtonPressed(2); };

        SubscribeViewButtons(true);
    }

    private void OnViewButtonPressed(int modeIndex)
    {
        calendarManager.ViewDetailsOfMode(modeIndex);
        for (int i = 0; i < viewButtons.Count; i++)
        {
            viewButtons[i].interactable = modeIndex == i;
        }
    } 

    public void SetInteractableAllViewButtons()
    {
        foreach (Button button in viewButtons)
        {
            button.interactable = true;
        }
    }

    private void SubscribeViewButtons(bool isSubscribed)
    {
        if (isSubscribed)
        {
            viewButtons[0].onClick.AddListener(viewSMode);
            viewButtons[1].onClick.AddListener(viewMMode);
            viewButtons[2].onClick.AddListener(viewLMode);
        }
        else
        {
            viewButtons[0].onClick.RemoveListener(viewSMode);
            viewButtons[1].onClick.RemoveListener(viewMMode);
            viewButtons[2].onClick.RemoveListener(viewLMode);
        }
    }

    public void UpdateSelectedDayTitle(string selectedDayText)
    {
        selectedDayLable.text = selectedDayText;
    }

    public override void OpenPanel()
    {
        if(calendarManager != null)
        {
            calendarManager.UpdateCalendar();
            UpdateViewButtons();
        }
        visualContainer.alpha = 0;
        visualContainer.DOFade(1, tweenDuration).SetEase(Ease.InOutQuad);
    }

    public async void UpdateViewButtons()
    {
        for (int i = 0; i < 3; i++)
        {           
            bool isDone = await DataManager.Instance.IsDateModeCompleted((TaskMode)i,
                CalendarManager.Instance.SelectedDate);
            
            viewButtons[i].gameObject.SetActive(isDone);
        }
        bool hasPlayedThisDay = viewButtons.Any(b => b.gameObject.activeInHierarchy);
        viewTimeButtons.gameObject.SetActive(hasPlayedThisDay);
        notPlayedPlaceholder.SetActive(!hasPlayedThisDay);
    }

    public override void ClosePanel()
    {
        visualContainer.alpha = 1;
        visualContainer.DOFade(0, tweenDuration).SetEase(Ease.InOutQuad);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ChallengeOLD : TaskOLD
{
    #region Fields

    [Header("GUI Elements:")]
    [SerializeField] protected Transform RightPanel;
    [SerializeField] protected Transform LeftPanel;
    [SerializeField] protected Transform TimerPanel;
    [SerializeField] protected LivesPanel LivesPanel;
    [SerializeField] protected Image BGImage;

    protected TimerPanel timerPanel;
    protected GameObject bgImageContainer;
    protected FlexibleGridLayout vGrid;
    protected RectTransform vPanel;
    protected RectTransform rectImage;
    protected RectTransform rectGrid;
    protected RectTransform rectRight;
    protected RectTransform rectLeft;
    protected RectTransform rectTaskPanel;
    protected AspectRatioFitter imageFitter;
    protected AspectRatioFitter gridFitter;

    [Header("Config:")]
    [SerializeField] protected int maxLives;
    [SerializeField] protected float panelsMarginX = 64f;

    #endregion

    protected void Initialization()
    {
        if(TimerPanel != null)
        {
            timerPanel = TimerPanel.GetComponent<TimerPanel>();
        }
        if (VariantsPanel != null)
        {
            bgImageContainer = BGImage.transform.parent.gameObject;
            vGrid = VariantsPanel.GetComponent<FlexibleGridLayout>();
            vPanel = VariantsPanel.parent.GetComponent<RectTransform>();

            rectImage = bgImageContainer.GetComponent<RectTransform>();
            rectGrid = vGrid.GetComponent<RectTransform>();
            imageFitter = bgImageContainer.GetComponent<AspectRatioFitter>();
            gridFitter = vGrid.GetComponent<AspectRatioFitter>();
        }
        if (TaskPanel != null)
        {
            rectTaskPanel = TaskPanel.GetComponent<RectTransform>();
        }
        rectRight = RightPanel.GetComponent<RectTransform>();
        rectLeft = LeftPanel.GetComponent<RectTransform>();
        ResetToDefault();
    }

    public virtual float GetCorrectRate()
    {
        float correctRate = LivesPanel.Lives / (float)maxLives * 100f;
        return correctRate;
    }

    public override void ResetToDefault()
    {
        UpdateDisplayStyle();
        maxLives = 3;
        gridFitter.enabled = true;
        imageFitter.enabled = true;
    }

    protected void SetLives()
    {
        LivesPanel.SetLives(maxLives);
    }

    #region Display style

    protected virtual void UpdateDisplayStyle()
    {
        ShowTaskPanel(true);
        ShowTimer(true, LeftPanel);
        ShowLivesPanel(true, RightPanel);
        ShowBGImage(true, BGImage.sprite);
        ShowVariantsPanel(true, 12, new Vector2(10, 10));
    }

    protected virtual IEnumerator UpdatePanels()
    {
        imageFitter.enabled = false;
        Vector2 center = new Vector2(0.5f, 0.5f);
        rectImage.anchorMin = center;
        rectImage.anchorMax = center;
        rectImage.pivot = center;

        yield return new WaitForFixedUpdate();

        gridFitter.aspectRatio = (float)vGrid.columns / (float)vGrid.rows;
        rectImage.sizeDelta = rectGrid.rect.size;
        UpdateTaskPanelSize();
        UpdatePanelPosition(rectLeft);
        UpdatePanelPosition(rectRight);
    }

    protected void UpdatePanelPosition(RectTransform panel)
    {
        bool isRight = panel.position.x > rectGrid.position.x;
        float panelWidth = panel.rect.size.x;
        float posX = (float)Math.Round((rectGrid.rect.size.x + panelWidth + panelsMarginX) / 2f, 0) * (isRight ? 1 : -1);

        panel.anchorMin = new Vector2(0.5f, panel.anchorMin.y);
        panel.anchorMax = new Vector2(0.5f, panel.anchorMax.y);
        panel.pivot = new Vector2(0.5f, 0.5f);

        panel.anchoredPosition = new Vector2(posX, panel.anchoredPosition.y);
        panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
    }

    protected void UpdateTaskPanelSize()
    {
        float panelWidth = rectGrid.rect.size.x;
        rectTaskPanel.anchorMin = new Vector2(0.5f, rectTaskPanel.anchorMin.y);
        rectTaskPanel.anchorMax = new Vector2(0.5f, rectTaskPanel.anchorMax.y);
        rectTaskPanel.pivot = new Vector2(0.5f, 0.5f);
        rectTaskPanel.sizeDelta = new Vector2(panelWidth, rectTaskPanel.sizeDelta.y);
    }

    static void uGUIAnchorAroundObject(RectTransform target)
    {
        var p = target.transform.parent.GetComponent<RectTransform>();

        var offsetMin = target.offsetMin;
        var offsetMax = target.offsetMax;
        var _anchorMin = target.anchorMin;
        var _anchorMax = target.anchorMax;

        var parent_width = p.rect.width;
        var parent_height = p.rect.height;

        var anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
                                    _anchorMin.y + (offsetMin.y / parent_height));
        var anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
                                    _anchorMax.y + (offsetMax.y / parent_height));

        target.anchorMin = anchorMin;
        target.anchorMax = anchorMax;

        target.offsetMin = new Vector2(0, 0);
        target.offsetMax = new Vector2(1, 1);
        target.pivot = new Vector2(0.5f, 0.5f);
    }

    protected void ShowTaskPanel(bool isActive)
    {
        float anchorMaxY = isActive ? 0.8f : 1f;
        TaskPanel.gameObject.SetActive(isActive);
        vPanel.anchorMax = new Vector2(vPanel.anchorMax.x, anchorMaxY);
        rectLeft.anchorMax = new Vector2(rectLeft.anchorMax.x, anchorMaxY);
        rectRight.anchorMax = new Vector2(rectRight.anchorMax.x, anchorMaxY);
    }

    #region SHOW TIMER

    protected void ShowTimer(bool isActive, Transform container)
    {
        TimerPanel.gameObject.SetActive(isActive);
        TimerPanel.SetParent(container, false);
    }

    protected void ShowTimer(bool isActive)
    {
        TimerPanel.gameObject.SetActive(isActive);
    }

    #endregion

    #region SHOW LIVES PANEL

    protected void ShowLivesPanel(bool isActive, Transform container)
    {
        LivesPanel.gameObject.SetActive(isActive);
        LivesPanel.transform.SetParent(container, false);
    }

    protected void ShowLivesPanel(bool isActive)
    {
        LivesPanel.gameObject.SetActive(isActive);
    }

    #endregion

    #region SHOW BGIMAGE

    protected void ShowBGImage(bool isActive, Sprite image)
    {
        bgImageContainer.SetActive(isActive);
        BGImage.sprite = image;
    }

    protected void ShowBGImage(bool isActive)
    {
        bgImageContainer.SetActive(isActive);
    }

    #endregion

    #region SHOW VARIANTS PANEL

    protected void ShowVariantsPanel(bool isActive, int top, Vector2 spacing)
    {
        vPanel.gameObject.SetActive(isActive);
        vGrid.padding.top = top;
        vGrid.spacing = spacing;
        if (isActive)
        {
            StartCoroutine(UpdatePanels());
        }
    }

    #endregion

    #endregion
}
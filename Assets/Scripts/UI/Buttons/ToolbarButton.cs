using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Core;

public class ToolbarButton : ButtonFX
{
    #region FIELDS

    public bool IsInteractable
    {
        get => button.interactable;
        set
        {
            button.interactable = value;
            iconImage.SetAlpha(value ? 1f : 0.5f);
        }
    }

    public Button Button { get => button; }

    [Header("COMPONENTS:")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image buttonImage;

    [Header("REFERENCES:")]
    [SerializeField] private List<Sprite> buttonImages;

    private AspectRatioFitter ratioFitter;

    #endregion

    protected override void Initialization()
    {
        base.Initialization();

        //Need to reset AspectRatioFitter for correct button display in Layout Group
        TryGetComponent<AspectRatioFitter>(out ratioFitter);
        if(ratioFitter != null)
        {
            ratioFitter.enabled = false;
            ratioFitter.enabled = true;
        }
    }

    public void SetButtonImage(int colorIndex)
    {
        buttonImage.sprite = buttonImages[colorIndex];
    }
}
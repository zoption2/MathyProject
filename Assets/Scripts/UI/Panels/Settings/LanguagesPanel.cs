using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Localization;

public class LanguagesPanel : PopupPanel
{
    public override void OpenPanel()
    {
        popupPanel.localScale = Vector3.zero;
        popupPanel.DOScale(Vector3.one, 0.2f).OnComplete(() => OnComplete(true));
    }

    public void ClosePanel(Locale locale)
    {
        ClosePanel();
    }
}

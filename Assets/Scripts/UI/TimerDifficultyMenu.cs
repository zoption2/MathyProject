using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using TMPro;
using Mathy.Core.Tasks;

public class TimerDifficultyMenu : MonoBehaviour
{
    [SerializeField] private Transform popupPanel;
    [SerializeField] private CanvasGroup background;
    [SerializeField] private Vector3 scaleTo = new Vector3(0.05f, 0.025f, 0);
    [SerializeField] private RawImage bgImage;

    [SerializeField] public List<TextMeshProUGUI> bestScoreTextLabels;

    private void OnEnable()
    {
        OpenPanel();
    }

    public void OpenPanel()
    {
        TaskManager.Instance.UpdateAllBestScoreText();
        background.alpha = 1;
        //background.DOFade(1, 0.5f);

        popupPanel.localScale = Vector3.zero;
        popupPanel.DOScale(Vector3.one, 0.2f).OnComplete(() => OnComplete(true));
    }

    public void ClosePanel()
    {
        background.DOFade(0, 0.25f);
        popupPanel.DOScale(Vector3.zero, 0.2f).OnComplete(() => OnComplete(false));
    }

    private void OnComplete(bool isOpened)
    {
        if (isOpened)
        {
            popupPanel.DOShakeScale(0.2f, scaleTo, 5, 30);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

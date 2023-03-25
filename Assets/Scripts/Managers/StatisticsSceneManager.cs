using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsSceneManager : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private CanvasGroup gfxCanvasGroup;
    [SerializeField] private StatisticsPanel sPanel;
    #endregion

    private void Start()
    {
        gfxCanvasGroup.interactable = false;
        gfxCanvasGroup.blocksRaycasts = false;
    }

    public void SetGFXActive(bool isActive)
    {
        gfxCanvasGroup.alpha = isActive ? 0 : 1;
        gfxCanvasGroup.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Join(gfxCanvasGroup.DOFade(isActive ? 1 : 0, 0.25f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() => OnTweenComplete(isActive));
    }

    private void OnTweenComplete(bool isActive)
    {
        gfxCanvasGroup.gameObject.SetActive(isActive);
        gfxCanvasGroup.interactable = isActive;
        gfxCanvasGroup.blocksRaycasts = isActive;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

public class TheorySceneManager : StaticInstance<TheorySceneManager>
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private CanvasGroup gfxCanvasGroup;
    [SerializeField] private Button backwardButton;
    [SerializeField] private TMP_Text title;

    [Header("THEORY GAMES:")]
    [SerializeField] private List<TheoryGame> theoryGames;

    public TheoryGame ActiveGame { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        backwardButton.onClick.AddListener(GoBack);
    }

    private void GoBack()
    {
        ActiveGame.gameObject.SetActive(false);
        SetGFXActive(false);
    }

    public void SetGFXActive(bool isActive)
    {
        gfxCanvasGroup.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Join(gfxCanvasGroup.DOFade(isActive ? 1 : 0, 0.5f).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() => OnTweenComplete(isActive));
    }

    private void OnTweenComplete(bool isActive)
    {
        gfxCanvasGroup.gameObject.SetActive(isActive);
        gfxCanvasGroup.interactable = isActive;
    }

    public void StartTheoryGameByIndex(int index)
    {
        SetGFXActive(true);
        ActiveGame = theoryGames[index];

        for (int i = 0; i < theoryGames.Count; i++)
        {
            theoryGames[i].gameObject.SetActive(i == index);
        }

        title.text = ActiveGame.Name;
    }
}

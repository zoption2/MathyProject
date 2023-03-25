using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PracticePanel : MonoBehaviour
{
    #region FIELDS

    [SerializeField] List<GameObject> allGamesPages;
    [SerializeField] Button nextPageButton;
    [SerializeField] Button prevPageButton;
    [SerializeField] TMP_Text leftPageText;
    [SerializeField] TMP_Text rightPageText;

    [SerializeField] List<Button> gameButtons;

    #endregion

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        nextPageButton.onClick.AddListener(NextAllGamesPage);
        prevPageButton.onClick.AddListener(PrevAllGamesPage);

        foreach (Button button in gameButtons)
        {
            button.interactable = true;
        }

        foreach (Button button in gameButtons)
        {
            button.onClick.AddListener(() => _= SetInteractableAllGameButton());
        }
    }

    private async UniTask SetInteractableAllGameButton()
    {
        foreach (Button button in gameButtons)
        {
            button.interactable = false;
        }
        await UniTask.Delay(1000);
        foreach (Button button in gameButtons)
        {
            button.interactable = true;
        }
    }

    public void NextAllGamesPage()
    {
        nextPageButton.gameObject.SetActive(false);
        prevPageButton.gameObject.SetActive(true);
        allGamesPages[0].SetActive(false);
        allGamesPages[1].SetActive(true);
        leftPageText.text = "3/4";
        rightPageText.text = "4/4";
    }

    public void PrevAllGamesPage()
    {
        prevPageButton.gameObject.SetActive(false);
        nextPageButton.gameObject.SetActive(true);
        allGamesPages[0].SetActive(true);
        allGamesPages[1].SetActive(false);
        leftPageText.text = "1/4";
        rightPageText.text = "2/4";
    }
}

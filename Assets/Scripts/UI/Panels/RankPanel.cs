using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankPanel : MonoBehaviour
{
    [Header("Components:")]

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI rankText;
    private RectTransform rectTransform;

    [Header("References:")]

    [SerializeField] private List<Sprite> icons;

    private void Awake()
    {
        Initialization();
    }

    private void Initialization()
    {
        rectTransform = icon.GetComponent<RectTransform>();
    }

    public void UpdateDisplayStyle()
    {
        int index = PlayerDataManager.Instance.PlayerRank;
        rankText.text = index.ToString();

        /*icon.sprite = icons[index];

        float offset = index > 5 ? 0f : -3.25f;
        rectTransform.SetTop(offset);*/
    }
}

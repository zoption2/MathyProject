using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesPanel : MonoBehaviour
{
    #region Fields

    [Header("Components:")]

    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;
    private List<HeartIcon> hearts;

    [Header("Config:")]

    [SerializeField] private float heightFactor = 150f;
    public int Lives { get; private set; }
    public int maxLives { get; private set; } = 3;

    #endregion

    public void SetDamage(int damage)
    {
        Lives -= damage;

        List<HeartIcon> heartsToTween = hearts.GetRange(maxLives - Lives - 1, damage);

        foreach (HeartIcon heart in heartsToTween)
        {
            heart.TweenHeart(true);
        }
    }

    public void SetLives(int lives)
    {
        maxLives = lives;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, maxLives * heightFactor);
        GenerateHearts();
    }

    private void GenerateHearts()
    {
        hearts = new List<HeartIcon>();
        heartContainer.DestroyChildren();
        Lives = maxLives;

        for (int i = 0; i < maxLives; i++)
        {
            var heartInstance = Instantiate(heartPrefab, heartContainer);
            hearts.Add(heartInstance.GetComponent<HeartIcon>());
        }
    }
}

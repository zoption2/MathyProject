using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheoryGame : MonoBehaviour
{
    [SerializeField] List<GameObject> digits;
    [SerializeField] private string nameKey;
    public string Name
    {
        get => LocalizationManager.GetLocalizedString("Theory Games", nameKey);
    }
    private int index = 0;

    private void OnEnable()
    {
        AudioManager.Instance.PlayDigitSound(index);
    }

    private void OnDisable()
    {
        index = 0;

        for (int i = 0; i < digits.Count; i++)
        {
            digits[i].SetActive(i == index);
        }
    }

    public void ActivateDigit(int step)
    {
        index = index + step;
        if (index > digits.Count - 1) index = 0;
        else if (index < 0) index = digits.Count - 1;

        for (int i = 0; i < digits.Count; i++)
        {
            digits[i].SetActive(i == index);
        }

        AudioManager.Instance.PlayDigitSound(index);
    }
}

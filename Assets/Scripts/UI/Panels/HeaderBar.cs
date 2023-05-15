using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Mathy.Core;

public class HeaderBar : MonoBehaviour
{
    #region Fields

    [Header("Components:")]

    [SerializeField] protected TMP_Text title;

    #endregion


    private void OnEnable()
    {
        if (title == null) title = GetComponentInChildren<TMP_Text>();
        UpdateText();
    }

    //void Start()
    //{
    //    Initialization();
    //}

    //private void Initialization()
    //{
        
    //    UpdateText();
    //}

    public virtual void UpdateText()
    {
        _ = AsyncUpdateText();
    }

    protected virtual async UniTask AsyncUpdateText()
    {
        await UniTask.WaitUntil(() => PlayerDataManager.Instance != null);
        SetText("Default");
    }

    public virtual void SetText(int newValue)
    {
        string value = newValue.ToString();
        SetText(value);
    }

    public virtual void SetText(string newValue)
    {
        title.text = newValue;
    }
}

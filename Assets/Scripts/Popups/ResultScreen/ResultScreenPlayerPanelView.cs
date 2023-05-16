using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface IResultScreenPlayerPanelView : IView
    {
        void SetPlayerName(string name);
        void SetPlayerIcon(Sprite icon);
    }


    public class ResultScreenPlayerPanelView : MonoBehaviour, IResultScreenPlayerPanelView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
           onHide?.Invoke();
        }

        public void SetPlayerIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }

        public void SetPlayerName(string name)
        {
            _name.text = name;
        }

        public void Release()
        {
            
        }
    }
}



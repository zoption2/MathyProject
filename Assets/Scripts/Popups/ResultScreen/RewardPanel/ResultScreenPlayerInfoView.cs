using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public interface IResultScreenPlayerInfoView : IView
    {
        void SetName(string name);
        void SetIcon(Sprite icon);
    }

    public class ResultScreenPlayerInfoView : MonoBehaviour, IResultScreenPlayerInfoView
    {
        [SerializeField] private TMP_Text _playerName;
        [SerializeField] private Image _playerIcon;

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            gameObject.SetActive(false);
            onHide?.Invoke();
        }

        public void Release()
        {
            
        }

        public void SetIcon(Sprite icon)
        {
            _playerIcon.sprite = icon;
        }

        public void SetName(string name)
        {
            _playerName.text = name;
        }
    }
}



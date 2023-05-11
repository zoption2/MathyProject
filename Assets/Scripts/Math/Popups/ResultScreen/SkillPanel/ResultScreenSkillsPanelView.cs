using System;
using TMPro;
using UnityEngine;


namespace Mathy.UI
{
    public interface IResultScreenSkillsPanelView : IView
    {
        void SetTitle(string title);
        void SetTotalLocalized(string text);
        void SetTotalResults(string text);
        ISkillResultProgressView[] ProgressViews { get; }
    }

    public class ResultScreenSkillsPanelView : MonoBehaviour, IResultScreenSkillsPanelView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _totalText;
        [SerializeField] private TMP_Text _totalResultText;
        [SerializeField] private SkillResultProgressView[] _progressViews;

        public ISkillResultProgressView[] ProgressViews => _progressViews;

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

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void SetTotalLocalized(string text)
        {
            _totalText.text = text;
        }

        public void SetTotalResults(string text)
        {
            _totalResultText.text = text;
        }
    }
}



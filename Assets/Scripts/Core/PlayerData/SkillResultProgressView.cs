using Mathy.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Services
{
    public interface ISkillResultProgressView
    {
        SkillType Skill { get; }
        void Init();
        void SetTitle(string title);
        void SetProgressRate(int progress);
        void SetResults(string results);
        void ShowChangedValue(string value);
    }


    public class SkillResultProgressView : MonoBehaviour, ISkillResultProgressView
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _resultsText;
        [SerializeField] private TMP_Text _changableValueText;
        [SerializeField] private GameObject _changableHolder;
        [field: SerializeField] public SkillType Skill { get; private set; }

        public void Init()
        {
            _changableHolder.SetActive(false);
        }

        public void SetProgressRate(int progress)
        {
            _progressBar.value = progress;
        }

        public void SetTitle(string  title)
        {
            _title.text = title;
        }

        public void SetResults(string results)
        {
            _resultsText.text = results;
        }

        public void ShowChangedValue(string value)
        {
            _changableHolder.SetActive(true);
            _changableValueText.text = value;
        }
    }
}



using TMPro;
using UnityEngine;

namespace Mathy.UI
{
    public interface IAchievementView
    {
        Achievements Achievement { get; }
        void SetValue(string value);
    }


    public class AchievementView : MonoBehaviour, IAchievementView
    {
        [field: SerializeField] public Achievements Achievement { get; private set; }
        [SerializeField] private TMP_Text _value;

        public void SetValue(string value)
        {
            _value.text = value;
        }
    }
}



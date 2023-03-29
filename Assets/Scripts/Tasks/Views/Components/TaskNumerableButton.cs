using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class TaskNumerableButton : MonoBehaviour
    {
        public Action<string> ON_CLICK;
        [SerializeField] private Button button;
        [field: SerializeField] public int Number { get; private set; }

        private void OnEnable()
        {
            button.onClick.AddListener(DoONButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(DoONButtonClick);
        }

        private void DoONButtonClick()
        {
            ON_CLICK?.Invoke(Number.ToString());
        }
    }
}


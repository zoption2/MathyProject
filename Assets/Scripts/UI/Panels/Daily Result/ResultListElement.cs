using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public class ResultListElement : MonoBehaviour
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private TMP_Text numberLabel;
        [SerializeField] private TMP_Text taskContentLabel;
        [SerializeField] private TMP_Text taskTimeLabel;
        [SerializeField] private Image statusImage;        
        //[SerializeField] private Button viewTaskButton;

        [Header("REFERENCES:")]
        [SerializeField] private List<Sprite> statusImages;

        private int taskNumber;
        public int TaskNumber 
        {
            get => taskNumber;
            private set
            {
                taskNumber = value;
                numberLabel.text = value.ToString();
            }
        }
        private bool taskCompleted;
        public bool IsTaskCompleted
        {
            get => taskCompleted;
            private set
            {
                taskCompleted = value;
                statusImage.sprite = statusImages[taskCompleted ? 0 : 1];
            }
        }

        #endregion

        //Subscribing action to the button
        //public void OnViewTaskButtonPressed(Action<int> action)
        //{
        //    viewTaskButton.onClick.AddListener(() => { action(TaskNumber-1); } );
        //}

        public void Initialize(int taskNumber, string taskResult, string taskTime, bool isTaskCompleted)
        {
            this.TaskNumber = taskNumber;
            this.taskContentLabel.text = taskResult;
            this.taskTimeLabel.text = taskTime;
            this.IsTaskCompleted = isTaskCompleted;
        }


    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;
using Mathy.Core.Tasks;
using Mathy.Core;

namespace Mathy.UI
{
    public class TaskCounterPanel : MonoBehaviour
    {
        #region Fields

        [Header("Components:")]
        [SerializeField] private Button labelButton;
        [SerializeField] private TMP_Text taskCounter;
        [SerializeField] private FlexibleGridLayout indicatorPanel;
        public List<TaskIndicator> TaskIndicators { get; private set; } = new List<TaskIndicator>();

        [Header("References:")]
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private List<AudioClip> panelSounds;

        [Header("Tween:")]
        [SerializeField] private float tweenDuration = 0.5f;
        [SerializeField] private List<float> moveTo = new List<float>();
        [SerializeField] private Vector3 scaleTo = new Vector3(0.05f, 0.1f, 0);

        [Header("Config:")]
        [SerializeField] private bool resetOnDisable = true;

        private float moveToY = 0;
        private RectTransform rTransform;
        private bool isTweening = false;
        private bool isOpened = false;

        #endregion

        void Start()
        {
           // Initialization();
        }

        private void OnDisable()
        {
            if (resetOnDisable)
            {
                ResetToDefault(true);
            }
        }

        private void Initialization()
        {
            if (labelButton == null)
            {
                labelButton = GetComponentInChildren<Button>();
            }
            taskCounter = GetComponentInChildren<TMP_Text>();

            if (indicatorPanel == null)
            {
                indicatorPanel = GetComponentInChildren<FlexibleGridLayout>();
            }
            TaskIndicators = indicatorPanel.gameObject.GetComponentsInChildren<TaskIndicator>().ToList();

            rTransform = GetComponent<RectTransform>();

            UpdatePanel(0);
        }

        public void ResetToDefault(bool resetStatuses)
        {
            isTweening = false;
            isOpened = false;
            rTransform.anchoredPosition = new Vector2(0, 0);

            if (resetStatuses)
            {
                foreach (TaskIndicator indicator in TaskIndicators)
                {
                    indicator.Status = TaskStatus.Pending;
                }
            }
        }

        /// <summary>
        /// Update the task counter text and the current and next task status indicator image
        /// </summary>
        public void UpdatePanel(int taskIndex)
        {
            UpdateCounterText(taskIndex, TaskManager.Instance.TasksAmount);
            SetIndicatorStatus(taskIndex, TaskStatus.InProgress);
        }

        public void UpdatePanel(int taskIndex, int taskAmount)
        {
            UpdateCounterText(taskIndex, taskAmount);
            SetIndicatorStatus(taskIndex, TaskStatus.InProgress);
        }

        public void UpdateCounterText(int taskIndex, int amount)
        {
            taskCounter.text = (taskIndex + 1) + "/" + amount;
        }

        public void UpdateIndicatorsAmount(int amount)
        {
            Transform indicatorContainer = indicatorPanel.transform;
            TaskIndicators = new List<TaskIndicator>();

            foreach (Transform child in indicatorPanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < amount; i++)
            {
                var indicator = Instantiate(indicatorPrefab, indicatorContainer);
                TaskIndicators.Add(indicator.GetComponent<TaskIndicator>());
            }

            UpdateDisplayStyle(amount);
        }

        private void UpdateDisplayStyle(int amount)
        {
            switch (amount)
            {
                case 10:
                    indicatorPanel.padding.bottom = 390;
                    indicatorPanel.columns = 10;
                    moveToY = moveTo[0];
                    break;
                case 20:
                    indicatorPanel.padding.bottom = 304;
                    indicatorPanel.columns = 10;
                    moveToY = moveTo[1];
                    break;
                case 30:
                    indicatorPanel.padding.bottom = 218;
                    indicatorPanel.columns = 10;
                    moveToY = moveTo[2];
                    break;
                case 40:
                    indicatorPanel.padding.bottom = 304;
                    indicatorPanel.columns = 20;
                    moveToY = moveTo[1];
                    break;
                case 60:
                    indicatorPanel.padding.bottom = 218;
                    indicatorPanel.columns = 20;
                    moveToY = moveTo[2];
                    break;
                default:
                    goto case 10;
            }
        }

        public void SetIndicatorStatus(int taskIndex, TaskStatus status)
        {
            TaskIndicators[taskIndex].Status = status;
        }

        public void TweenPanel()
        {
            if (!isTweening)
            {
                isTweening = true;
                DoMove();
            }
        }

        private void DoMove()
        {
            var sequence = DOTween.Sequence();

            if (isOpened)
            {
                sequence.Append(rTransform.DOShakeScale(tweenDuration, scaleTo, 10, 60f));
                sequence.Join(rTransform.DOAnchorPos(new Vector2(0, 0), tweenDuration).SetEase(Ease.OutBack)).OnComplete(() => OnTweenComplete());
                AudioSystem.Instance.PlaySound(panelSounds[0], UnityEngine.Random.Range(0.9f, 1.1f));
            }
            else
            {
                sequence.Append(rTransform.DOShakeScale(tweenDuration, scaleTo, 10, 60f));
                sequence.Join(rTransform.DOAnchorPos(new Vector2(0, moveToY), tweenDuration).SetEase(Ease.OutBack)).OnComplete(() => OnTweenComplete());
                AudioSystem.Instance.PlaySound(panelSounds[1], 0.5f, UnityEngine.Random.Range(0.9f, 1.1f));
            }
            isOpened = !isOpened;
        }

        private void OnTweenComplete()
        {
            isTweening = false;
        }
    }
}
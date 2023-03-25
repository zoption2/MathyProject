using UnityEngine;
using TMPro;
using Button = UnityEngine.UI.Button;

namespace Mathy.UI
{
    public sealed class DayResultsView : BehaviourWithModel<DayResultsModel>
    {
        [SerializeField] private TMP_Text selectedDayText;
        [SerializeField] private Button sModeButton;
        [SerializeField] private Button mModeButton;
        [SerializeField] private Button lModeButton;
        [SerializeField] private ModeStatView sStatsView;
        [SerializeField] private ModeStatView mStatsView;
        [SerializeField] private ModeStatView lStatsView;
        [SerializeField] private GameObject notPlayedPanel;

        private void OnEnable()
        {
            sModeButton.onClick.AddListener(OnSModeButtonClick);
            mModeButton.onClick.AddListener(OnMModeButtonClick);
            lModeButton.onClick.AddListener(OnLModeButtonClick);
        }

        private void OnDisable()
        {
            sModeButton.onClick.RemoveListener(OnSModeButtonClick);
            mModeButton.onClick.RemoveListener(OnMModeButtonClick);
            lModeButton.onClick.RemoveListener(OnLModeButtonClick);
        }

        protected override void OnModelApply(DayResultsModel model)
        {
            UpdateStatsViews(model.SelectedDayData.Value);
            model.SelectedDayData.ON_VALUE_CHANGED += UpdateStatsViews;
        }

        private void UpdateStatsViews(DayData dayData)
        {
            var sMode = dayData.ModeS;
            sStatsView.SetAnswerText(Model.LocalizedGrade, sMode.CorrectAnswers, sMode.TotalTasks);
            var sTime = dayData.GetLessonTimeSpan(sMode.LessonsTime);
            sStatsView.SetTimeText(Model.LocalizedTime, sTime.TotalMinutes, sTime.Seconds);
        }

        private void OnSModeButtonClick()
        {
            Model.ClickOn(TaskMode.Small);
        }

        private void OnMModeButtonClick()
        {
            Model.ClickOn(TaskMode.Medium);
        }

        private void OnLModeButtonClick()
        {
            Model.ClickOn(TaskMode.Large);
        }
    }

    public class ModeStatView : MonoBehaviour
    {
        [SerializeField] private TaskMode taskMode;
        [SerializeField] private TMP_Text gradeText;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private TMP_Text timeText;

        private string gradeFormat;
        private string answerFormat;
        private string timeFormat;

        private void Awake()
        {
            gradeFormat = gradeText.text;
            answerFormat = answerText.text;
            timeFormat = timeText.text;
        }

        public void SetGradeText(string localization, string grage)
        {
            gradeText.text = string.Format(gradeFormat, localization, grage);
        }

        public void SetAnswerText(string localization, int correct, int total)
        {
            answerText.text = string.Format(answerFormat, correct, total);
        }

        public void SetTimeText(string localization, double minuts, int seconds)
        {
            timeText.text = string.Format(timeFormat, minuts, seconds);
        }
    }
}








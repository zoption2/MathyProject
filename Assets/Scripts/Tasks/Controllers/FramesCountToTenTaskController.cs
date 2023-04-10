using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Mathy.UI.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class FramesCountToTenTaskController : BaseTaskController<IFramesCountToTenTaskView, IFramesCountToTenTaskModel>
    {
        private const string kSpritesTableKey = "CountedImages";
        private const int kMaxFrames = 10; 
        private const int kAnswersDelayMS = 50;

        private ITaskElementFrame[] allFrames;
        private List<ITaskElementFrame> unknownFrames;
        private ITaskViewComponentClickable[] variantsInputs;
        private CountedImageType selectedImageType;
        private string correctValueText;

        protected override string LocalizationTableKey => "TaskTitles";

        public FramesCountToTenTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) : base(refsHolder, backgroundSevice)
        {
        }

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }

        protected override async UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<VariantOneBackgroundType, VariantOneTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderImage(backgroundData.HeaderSprite);
            View.SetInputsHolderImage(backgroundData.HolderSprite);

            var correctValue = Model.CorrectValue;
            correctValueText = correctValue.ToString();
            var visibleFrames = Model.FramesToShow;
            var correctIndex = Model.CorrectIndex;
            allFrames = View.Frames;

            var imageValues = Enum.GetValues(typeof(CountedElementFrameType));
            selectedImageType = (CountedImageType)imageValues.GetValue(random.Next(imageValues.Length));
            Sprite sprite = await refsHolder.TaskCountedImageProvider.GetSpriteByType(selectedImageType);
            if (sprite == null)
            {
                Debug.LogFormat("Sprite from addresables is null");
            }

            unknownFrames = new List<ITaskElementFrame>(kMaxFrames);
            for (int i = 0; i < kMaxFrames; i++)
            {
                var value = (i + 1).ToString();
                TaskElementState frameState = TaskElementState.Default;
                if (i > visibleFrames - 1)
                {
                    frameState = TaskElementState.Unknown;
                    unknownFrames.Add(allFrames[i]);
                }
                allFrames[i].Init(i, value, sprite, frameState);
            }

            variantsInputs = View.Inputs;
            for (int i = 0, j = variantsInputs.Length; i < j; i++)
            {
                string value = (i + 1).ToString();
                variantsInputs[i].Init(i, value);
                variantsInputs[i].ON_CLICK += DoOnVariantClick;
            }
        }

        protected override string GetLocalizedTitle()
        {
            var localizedTitleFormat = LocalizationManager.GetLocalizedString(LocalizationTableKey, Model.TitleKey);
            string imageKey = selectedImageType.ToString();
            var countedObject = LocalizationManager.GetLocalizedString(kSpritesTableKey, imageKey);
            return string.Format(localizedTitleFormat, countedObject);
        }

        private async void DoOnVariantClick(ITaskViewComponent input)
        {
            UnsubscribeInputs();
            var isCorrect = input.Value == correctValueText;
            TaskElementState state = isCorrect ? TaskElementState.Correct : TaskElementState.Wrong;
            input.ChangeState(state);

            for (int i = 0, j = unknownFrames.Count; i < j; i++)
            {
                var value = (i + 1).ToString();
                var frame = unknownFrames[i];
                bool isLastValue = (i + 1) == j;
                frame.ChangeValue(value, isLastValue);
                frame.ChangeState(state);
                await UniTask.Delay(kAnswersDelayMS);
            }

            IsAnswerCorrect = isCorrect;
            SelectedAnswerIndexes.Add(input.Index);

            CompleteTask();
        }

        private void UnsubscribeInputs()
        {
            foreach (var variant in variantsInputs)
            {
                variant.ON_CLICK -= DoOnVariantClick;
            }
        }
    }
}
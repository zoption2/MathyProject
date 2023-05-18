using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class FramesCountToTwentyTaskController : BaseTaskController<IFramesCountToTwentyTaskView, ICountToAmountTaskModel>
    {
        private const string kSpritesTableKey = "CountedImages";
        private const string kViewElementsTableKey = "TaskViewComponents";
        private const string kThereAreKey = "thereAreKey";
        private const int kHolderCapacity = 10;
        private const int kMaxTaskElements = 20;
        private const int kCorrectAnswerIndex = 0;
        private const int kWrongAnswerIndex = 1;
        private const int kMaxInputs = 2;

        private int correctValue;
        private string correctValueString;
        private char[] correctChars;
        private ITaskViewComponentClickable inputFieldElement;
        private List<ITaskSimpleImageElement> elements;
        private ITaskViewComponentClickable[] variantInputs;
        private ITaskElementHolderView[] frames;
        private string localizedObjectName;
        private int inputs = 0;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }
        protected override string LocalizationTableKey => "TaskTitles";


        public FramesCountToTwentyTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) : base(refsHolder, backgroundSevice)
        {
        }


        protected override async UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<VariantOneBackgroundType, VariantOneTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderImage(backgroundData.HeaderSprite);
            View.SetInputsHolderImage(backgroundData.HolderSprite);
            var thereAreText = GetLocalizedThereAreText();
            View.SetThereAreText(thereAreText);


            correctValue = Model.CountToShow;
            correctValueString = correctValue.ToString();
            correctChars = correctValueString.ToCharArray();

            inputFieldElement = View.InputFieldElement;
            inputFieldElement.Init(0, "");
            inputFieldElement.ON_CLICK += DeleteInputedValue;

            frames = View.ElementsHolder;
            for (int i = 0, j = frames.Length; i < j; i++)
            {
                frames[i].Init(i);
            }

            var imageValues = Enum.GetValues(typeof(CountedImageType));
            var selectedImageType = (CountedImageType)imageValues.GetValue(random.Next(imageValues.Length));
            localizedObjectName = GetLocalizedObjectName(selectedImageType.ToString());
            View.SetObjectNameText(localizedObjectName);

            Sprite sprite = await refsHolder.TaskCountedImageProvider.GetSpriteByType(selectedImageType);
            if (sprite == null)
            {
                Debug.LogFormat("Sprite from addresables is null");
            }

            elements = new List<ITaskSimpleImageElement>(20);
            int remainingValue = correctValue;
            //filling holder 1
            for (int i = 0; i < kHolderCapacity && remainingValue > 0; i++)
            {
                var element = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskSimpleImageElement>(UIComponentType.SimpleImageElement, frames[0].Parent);
                element.Init(i, i.ToString(), sprite);
                elements.Add(element);
                remainingValue--;
            }

            //filling holder 2
            for (int i = 0; i < kHolderCapacity && remainingValue > 0; i++)
            {
                var element = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskSimpleImageElement>(UIComponentType.SimpleImageElement, frames[1].Parent);
                element.Init(i, i.ToString(), sprite);
                elements.Add(element);
                remainingValue--;
            }

            variantInputs = View.InputButtons;
            int lastInputIndex = variantInputs.Length - 1;
            string lastInputValue = 0.ToString();
            variantInputs[lastInputIndex].Init(lastInputIndex, lastInputValue);
            variantInputs[lastInputIndex].ON_CLICK += DoOnInputClick;
            for (int i = 0, j = variantInputs.Length - 1; i < j; i++)
            {
                string value = (i + 1).ToString();
                variantInputs[i].Init(i, value);
                variantInputs[i].ON_CLICK += DoOnInputClick;
            }
        }

        private void DoOnInputClick(ITaskViewComponent input)
        {
            var inputedValueString = input.Value;
            var totalValueString = inputFieldElement.Value;
            totalValueString += inputedValueString;
            var totalValue = int.Parse(totalValueString);

            inputFieldElement.ChangeValue(totalValueString);
            inputs++;

            if (correctChars.Length == 1)
            {
                var tempValue = correctChars[0].ToString();
                if (tempValue != inputedValueString)
                {
                    Fail();
                    return;
                }
            }

            if (totalValue > correctValue)
            {
                Fail();
            }
            else if (totalValue == correctValue)
            {
                Success();
            }
            else if (inputs >= kMaxInputs)
            {
                Fail();
            }

            void Fail()
            {
                UnsubscribeInputs();
                inputFieldElement.ChangeState(TaskElementState.Wrong);
                IsAnswerCorrect = false;
                taskData.VariantValues.Add(totalValueString);
                SelectedAnswerIndexes.Add(kWrongAnswerIndex);
                UpdateHolders(TaskElementState.Wrong);
                CompleteTask();
            }

            void Success()
            {
                UnsubscribeInputs();
                inputFieldElement.ChangeState(TaskElementState.Correct);
                IsAnswerCorrect = true;
                taskData.VariantValues.Add(totalValueString);
                SelectedAnswerIndexes.Add(kCorrectAnswerIndex);
                UpdateHolders(TaskElementState.Correct);
                CompleteTask();
            }
        }

        protected override string GetLocalizedTitle()
        {
            var localizedTitleFormat = LocalizationManager.GetLocalizedString(LocalizationTableKey, Model.TitleKey);
            return string.Format(localizedTitleFormat, localizedObjectName);
        }

        private void DeleteInputedValue(ITaskViewComponent component)
        {
            inputFieldElement.ChangeValue("");
            inputs = 0;
        }

        private void UpdateHolders(TaskElementState state)
        {
            for (int i = 0, j = frames.Length; i < j; i++)
            {
                frames[i].ChangeState(state);
            }
        }

        private void UnsubscribeInputs()
        {
            foreach (var variant in variantInputs)
            {
                variant.ON_CLICK -= DoOnInputClick;
            }

            inputFieldElement.ON_CLICK -= DeleteInputedValue;
        }

        private string GetLocalizedThereAreText()
        {
            return LocalizationManager.GetLocalizedString(kViewElementsTableKey, kThereAreKey);
        }

        private string GetLocalizedObjectName(string key)
        {
            localizedObjectName = LocalizationManager.GetLocalizedString(kSpritesTableKey, key);
            return localizedObjectName;
        }
    }
}
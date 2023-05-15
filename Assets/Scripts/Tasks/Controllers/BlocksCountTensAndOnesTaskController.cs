using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class BlocksCountTensAndOnesTaskController : BaseTaskController<IFramesCountTensAndOnesTaskView, ICountToAmountTaskModel>
    {
        private const string kSpritesTableKey = "CountedImages";
        private const string kViewElementsTableKey = "TaskViewComponents";
        private const string kTens = "tens";
        private const string kOnes = "ones";
        private const int kHolderCapacity = 20;
        private const int kMaxTaskElements = 20;
        private const int kCorrectAnswerIndex = 0;
        private const int kWrongAnswerIndex = 1;
        private const int kMaxInputs = 2;

        private int correctValue;
        private string correctValueString;
        private char[] correctChars;
        private ITaskViewComponent inputFieldElementTens;
        private ITaskViewComponent inputFieldElementOnes;
        private ITaskViewComponent resultField;
        private List<ITaskSimpleImageElement> elements;
        private ITaskViewComponentClickable[] variantInputs;
        private ITaskElementHolderView[] frames;
        private string localizedObjectName;
        private int inputs = 0;
        private bool tens = true;
        private bool tensState = true;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }
        protected override string LocalizationTableKey => "TaskTitles";


        public BlocksCountTensAndOnesTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) : base(refsHolder, backgroundSevice)
        {
        }


        protected override async UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<VariantOneBackgroundType, VariantOneTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderImage(backgroundData.HeaderSprite);
            View.SetInputsHolderImage(backgroundData.HolderSprite);
            View.SetTens(GetLocalizedTens());
            View.SetOnes(GetLocalizedOnes());

            correctValue = Model.CountToShow;
            correctValueString = correctValue < 10 
                ? correctValue.ToString().PadLeft(2, '0')
                : correctValue.ToString();
            correctChars = correctValueString.ToCharArray();

            resultField = View.ResultField;
            resultField.Init(0, "");

            inputFieldElementTens = View.InputFieldElementTens;
            inputFieldElementTens.Init(0, "?", TaskElementState.Unknown);

            inputFieldElementOnes = View.InputFieldElementOnes;
            inputFieldElementOnes.Init(0, "");

            frames = View.ElementsHolder;
            for (int i = 0, j = frames.Length; i < j; i++)
            {
                frames[i].Init(i);
            }

            var imageValues = Enum.GetValues(typeof(CountedBlocksImageType));
            var selectedImageType = (CountedBlocksImageType)imageValues.GetValue(random.Next(imageValues.Length));
            localizedObjectName = GetLocalizedObjectName(selectedImageType.ToString());

            Sprite sprite = await refsHolder.TaskCountedBlocksImageProvider.GetSpriteByType(selectedImageType);
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
            var totalValueString = tens == true
                ? inputedValueString + inputFieldElementOnes.Value
                : inputFieldElementTens.Value + inputedValueString;
            int totalValue = 0;
            if (totalValueString != "")
            {
                totalValue = int.Parse(totalValueString);
            }
            inputFieldElementTens.ChangeState(UI.Tasks.TaskElementState.Default);
            resultField.ChangeValue(totalValue.ToString());
            
            //var totalValue = int.Parse(inputFieldElementTens.Value + inputFieldElementOnes.Value);

            if (tens == true)
            {
                inputFieldElementTens.ChangeValue(inputedValueString);
                tens = false;
                inputFieldElementOnes.ChangeState(UI.Tasks.TaskElementState.Unknown);
                inputFieldElementOnes.ChangeValue("?");
            }
            else
            {
                inputFieldElementOnes.ChangeValue(inputedValueString);
                tens = true;
            }


            inputs++;


            if (correctChars[0].ToString() != inputFieldElementTens.Value && tens == false)
            {
                 Fail();
                 return;
                
            }

            if (totalValueString != correctValueString && tens == true)
            {
                Fail();
            }
            else if (totalValueString == correctValueString)
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
                inputFieldElementTens.ChangeState(UI.Tasks.TaskElementState.Wrong);
                inputFieldElementOnes.ChangeState(UI.Tasks.TaskElementState.Wrong);
                resultField.ChangeState(UI.Tasks.TaskElementState.Wrong);
                IsAnswerCorrect = false;
                taskData.VariantValues.Add(FinalValueString(inputedValueString));
                SelectedAnswerIndexes.Add(kWrongAnswerIndex);
                UpdateHolders(TaskElementState.Wrong);
                CompleteTask();
            }

            void Success()
            {
                UnsubscribeInputs();
                inputFieldElementTens.ChangeState(UI.Tasks.TaskElementState.Correct);
                inputFieldElementOnes.ChangeState(UI.Tasks.TaskElementState.Correct);
                resultField.ChangeState(UI.Tasks.TaskElementState.Correct);
                IsAnswerCorrect = true;
                taskData.VariantValues.Add(FinalValueString(inputedValueString));
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
        private string FinalValueString(string inputedValueString)
        {
            var tempValue = inputedValueString != "0" ? "0" : inputedValueString;
            var totalValueStringFin = tens == true
                ? tempValue + "+" + inputFieldElementOnes.Value
                : inputFieldElementTens.Value + "+" + tempValue;
            return totalValueStringFin;
        }

        private string GetLocalizedTens()
        {
            return LocalizationManager.GetLocalizedString(kViewElementsTableKey, kTens);
        }

        private string GetLocalizedOnes()
        {
            return LocalizationManager.GetLocalizedString(kViewElementsTableKey, kOnes);
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
        }

        private string GetLocalizedObjectName(string key)
        {
            localizedObjectName = LocalizationManager.GetLocalizedString(kSpritesTableKey, key);
            return localizedObjectName;
        }

        /*private string GetVariantString(string variant)
        {
            int result = int.Parse("1+1".Substring(0, "1+1".IndexOf('+'))) +
             int.Parse("1+1".Substring("1+1".IndexOf('+') + 1));

        }*/
    }
}
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using Mathy.UI.Tasks;
using UnityEngine;
using System.Linq;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class SelectFromThreeCountTaskController : BaseTaskController<ISelectFromThreeCountTaskView, ISelectFromThreeCountTaskModel>
    {
        private const int kMaxElementsCount = 30;
        private const int kMaxImagesVariants = 3;
        private const float kElementSize = 250;
        private const float kScalingCoef = 10f;
        private const string kGreenTextFormat = "<color=#00ff00>{0}</color>";

        protected override string LocalizationTableKey => "TaskTitles";

        private List<ITaskElementImageWithCollider> elements;
        private ITaskViewComponentClickable[] variantsInputs;
        private List<object> presentImages;
        private string correctAnswer;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }


        public SelectFromThreeCountTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) : base(refsHolder, backgroundSevice)
        {
        }


        protected override async UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<VariantOneBackgroundType, VariantOneTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderImage(backgroundData.HeaderSprite);

            var questionSign = ((char)ArithmeticSigns.QuestionMark).ToString();

            var values = Model.Values;
            var correctValue = Model.CorrectValue;
            var parents = View.GroupParents;
            variantsInputs = View.Inputs;

            correctAnswer = correctValue.ToString();

            presentImages = new List<object>(kMaxImagesVariants);

            elements = new List<ITaskElementImageWithCollider>(kMaxElementsCount);

            for (int i = 0, j = values.Count; i < j; i++)
            {
                var imageValues = Enum.GetValues(typeof(SelectFromThreeImageType));
                imageValues = imageValues.Cast<object>()
                    .Except(presentImages)
                    .ToArray();

                var selectedValue = imageValues.GetValue(random.Next(imageValues.Length));
                presentImages.Add(selectedValue);
                var castedImageType = (CountedImageType)selectedValue;

                var groupValue = values[i];
                var groupParent = parents[i];
                var size = CalculateImageSize(kElementSize, groupValue);

                for (int g = 0; g < groupValue; g++)
                {
                    var component = await refsHolder.UIComponentProvider
                        .InstantiateFromReference<ITaskElementImageWithCollider>(UIComponentType.ImageWithColliderElement, groupParent);
                    Sprite sprite = await refsHolder.TaskCountedImageProvider.GetSpriteByType(castedImageType);
                    if (sprite == null)
                    {
                        Debug.LogFormat("Sprite from addresables is null");
                    }
                    component.Init(i, sprite);
                    var randomPosition = View.GetRandomPositionAtGroup(i);
                    component.SetPosition(randomPosition);
                    component.SetSize(size);
                    elements.Add(component);
                }

                string value = groupValue.ToString();
                variantsInputs[i].Init(i, value);
                variantsInputs[i].ON_CLICK += DoOnVariantClick;
            }
        }

        protected override string GetLocalizedTitle()
        {
            var localizedTitleFormat = LocalizationManager.GetLocalizedString(LocalizationTableKey, Model.TitleKey);
            string searchingCount = string.Format(kGreenTextFormat, correctAnswer);
            return string.Format(localizedTitleFormat, searchingCount);
        }

        protected override void OnTaskShowed()
        {
            EnableColliders(false);
        }

        protected override void DoOnTaskPrepare()
        {
            EnableColliders(true);
        }

        private void DoOnVariantClick(ITaskViewComponent input)
        {
            UnsubscribeInputs();

            var isCorrect = input.Value == correctAnswer;
            TaskElementState state = isCorrect ? TaskElementState.Correct : TaskElementState.Wrong;
            input.ChangeState(state);

            IsAnswerCorrect = isCorrect;
            SelectedAnswerIndexes.Add(input.Index);

            CompleteTask();
        }

        private float CalculateImageSize(float baseSize, int amountOfImages)
        {
            var scaling = amountOfImages * kScalingCoef;
            var result = baseSize - scaling;
            return result;
        }

        private void EnableColliders(bool isEnable)
        {
            foreach (var element in elements)
            {
                element.EnableColliders(isEnable);
            }
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
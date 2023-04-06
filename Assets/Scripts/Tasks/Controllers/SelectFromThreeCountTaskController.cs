using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using Mathy.UI.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class SelectFromThreeCountTaskController : BaseTaskController<ISelectFromThreeCountTaskView, ISelectFromThreeCountTaskModel>
    {
        private const int kMaxElementsCount = 30;
        private const float kElementSize = 200;
        protected override string LocalizationTableKey => "VariantOneTaskView";

        private List<ITaskElementImageWithCollider> elements;
        private ITaskViewComponentClickable[] variantsInputs;
        private CountedImageType selectedImageType;
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

            elements = new List<ITaskElementImageWithCollider>(kMaxElementsCount);

            for (int i = 0, j = values.Count; i < j; i++)
            {
                var imageValues = Enum.GetValues(typeof(SelectFromThreeImageType));
                selectedImageType = (CountedImageType)imageValues.GetValue(random.Next(imageValues.Length));

                var groupValue = values[i];
                var groupParent = parents[i];

                for (int g = 0; j < groupValue; i++)
                {
                    var component = await refsHolder.UIComponentProvider
                        .InstantiateFromReference<ITaskElementImageWithCollider>(UIComponentType.ImageWithColliderElement, groupParent);
                    Sprite sprite = await refsHolder.TaskCountedImageProvider.GetSpriteByType(selectedImageType);
                    if (sprite == null)
                    {
                        Debug.LogFormat("Sprite from addresables is null");
                    }
                    component.Init(i, sprite);
                    var randomPosition = View.GetRandomPositionAtGroup(i);
                    component.SetPosition(randomPosition);
                    component.SetSize(kElementSize);
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
            string searchingCount = correctAnswer;
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
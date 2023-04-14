using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class CountToTenImagesTaskController : BaseTaskController<ICountingToTenTaskView, ICountToAmountTaskModel>
    {
        private const string kSpritesTableKey = "CountedImages";

        private List<ITaskElementImageWithCollider> elements;
        private ITaskViewComponentClickable[] variantInputs;
        private CountedImageType selectedImageType;
        private ITaskElementHolderView holderView;
        private string correctAnswer;

        protected override string LocalizationTableKey => "TaskTitles";
        protected override bool IsAnswerCorrect {get; set;}
        protected override List<int> SelectedAnswerIndexes { get; set; }

        public CountToTenImagesTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) 
            : base(refsHolder, backgroundSevice)
        {
        }

        protected override async UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<VariantOneBackgroundType, VariantOneTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderImage(backgroundData.HeaderSprite);
            View.SetInputsHolderImage(backgroundData.HolderSprite);

            var questionSign = ((char)ArithmeticSigns.QuestionMark).ToString();

            var countOfElements = Model.CountToShow;
            correctAnswer = countOfElements.ToString();

            holderView = View.ElementsHolder;
            holderView.Init(0);

            elements = new List<ITaskElementImageWithCollider>(10);

            var imageValues = Enum.GetValues(typeof(CountedImageType));
            selectedImageType = (CountedImageType)imageValues.GetValue(random.Next(imageValues.Length));



            for (int i = 0; i < countOfElements; i++)
            {
                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskElementImageWithCollider>(UIComponentType.ImageWithColliderElement, holderView.Parent);
                Sprite sprite = await refsHolder.TaskCountedImageProvider.GetSpriteByType(selectedImageType);
                if (sprite == null)
                {
                    Debug.LogFormat("Sprite from addresables is null");
                }
                component.Init(i, sprite);
                var randomPosition = View.GetRandomPositionAtHolder();
                component.SetPosition(randomPosition);
                elements.Add(component);
            }

            variantInputs = View.Inputs;
            for (int i = 0, j = variantInputs.Length; i < j; i++)
            {
                string value = (i + 1).ToString();
                variantInputs[i].Init(i, value);
                variantInputs[i].ON_CLICK += DoOnVariantClick;
            }
        }

        protected override string GetLocalizedTitle()
        {
            var localizedTitleFormat = LocalizationManager.GetLocalizedString(LocalizationTableKey, Model.TitleKey);
            string imageKey = selectedImageType.ToString();
            var countedObject = LocalizationManager.GetLocalizedString(kSpritesTableKey, imageKey);
            return string.Format(localizedTitleFormat, countedObject);
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
            holderView.ChangeState(state);
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
            foreach (var variant in variantInputs)
            {
                variant.ON_CLICK -= DoOnVariantClick;
            }
        }
    }
}
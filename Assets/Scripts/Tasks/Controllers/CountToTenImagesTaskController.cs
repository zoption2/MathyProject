using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using Mathy.Data;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class CountToTenImagesTaskController : BaseTaskController<ICountingToTenTaskView, ICountToTenImagesTaskModel>
    {
        private List<ITaskElementImageWithCollider> elements;
        private ITaskViewComponentClickable[] variantsInputs;
        private string correctAnswer;

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

            var elementsHolder = View.ElementsHolder;

            elements = new List<ITaskElementImageWithCollider>(10);

            var sprite = await refsHolder.TaskCountedImageProvider.GetRandomSprite();
            for (int i = 0; i < countOfElements; i++)
            {
                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskElementImageWithCollider>(UIComponentType.ImageWithColliderElement, elementsHolder);
                component.Init(i, sprite);
                var randomPosition = View.GetRandomPositionAtHolder();
                component.SetPosition(randomPosition);
                elements.Add(component);
            }

            variantsInputs = View.Inputs;
            for (int i = 0, j = variantsInputs.Length; i < j; i++)
            {
                string value = (i + 1).ToString();
                variantsInputs[i].Init(i, value);
                variantsInputs[i].ON_CLICK += DoOnVariantClick;
            }
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

            taskData.TaskPlayDuration = TotalPlayingTime;
            taskData.IsAnswerCorrect = isCorrect;
            taskData.SelectedAnswerIndexes = new List<int>(1);
            taskData.SelectedAnswerIndexes.Add(input.Index);

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
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class IsThatTrueTaskController : BaseTaskController<IStandardTaskView, IIsThatTrueTaskModel>
    {
        private List<ITaskViewComponent> taskElements;
        private List<ITaskViewComponentClickable> taskVariants;
        private string userAnswer;
        private string correctAnswer;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }

        public IsThatTrueTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice)
            : base(refsHolder, backgroundSevice)
        {
        }

        protected override async UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<StandardBackgroundType, DefaultTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderColor(backgroundData.HeaderColor);

            var questionSign = ((char)ArithmeticSigns.QuestionMark).ToString();

            var expression = Model.Expression;
            var elementsParent = View.ElementsParent;
            taskElements = new List<ITaskViewComponent>(expression.Count);
            for (int i = 0; i < expression.Count; i++)
            {
                var elementType = expression[i].Type;
                var elementValue = expression[i].Value;
                UIComponentType elementView = GetElementViewByType(elementType);

                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskViewComponent>(elementView, elementsParent);

                TaskElementState state = TaskElementState.Default;
                component.Init(i, elementValue, state);
                taskElements.Add(component);
            }

            var variants = Model.Variants;
            correctAnswer = variants[Model.CorrectVariantIndex];
            var variantsParent = View.VariantsParent;
            taskVariants = new List<ITaskViewComponentClickable>(variants.Count);
            for (int i = 0; i < variants.Count; i++)
            {
                var variantValue = variants[i];
                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskViewComponentClickable>(UIComponentType.DefaultVariant, variantsParent);
                component.Init(i, variantValue);
                component.ON_CLICK += DoOnClick;
                taskVariants.Add(component);
            }
        }

        private void DoOnClick(ITaskViewComponent view)
        {
            UnsubscribeInputs();
            bool isAnswerCorrect;
            userAnswer = view.Value;
            if (userAnswer.Equals(correctAnswer))
            {
                view.ChangeState(TaskElementState.Correct);
                isAnswerCorrect = true;
            }
            else
            {
                view.ChangeState(TaskElementState.Wrong);
                isAnswerCorrect = false;
            }

            IsAnswerCorrect = isAnswerCorrect;
            SelectedAnswerIndexes.Add(view.Index);

            CompleteTask();
        }

        private void UnsubscribeInputs()
        {
            foreach (var variant in taskVariants)
            {
                variant.ON_CLICK -= DoOnClick;
            }
        }
    }
}
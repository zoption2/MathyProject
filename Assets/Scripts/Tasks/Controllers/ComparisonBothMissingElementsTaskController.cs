using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class ComparisonBothMissingElementsTaskController : BaseTaskController<IStandardTaskView, IComparisonBothMissingElementsTaskModel>
    {
        private List<ITaskViewComponent> taskElements;
        private List<ITaskViewComponentClickable> taskVariants;
        private List<ITaskViewComponent> correctVariants;
        private List<ITaskViewComponent> unknownElements;
        private ITaskViewComponent firstComponent;
        private string userAnswer;
        private bool isFirstElementSelected;

        protected override bool IsAnswerCorrect {get; set;}
        protected override List<int> SelectedAnswerIndexes { get; set; }

        public ComparisonBothMissingElementsTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) : base(refsHolder, backgroundSevice)
        {
        }



        protected async override UniTask DoOnInit()
        {
            var backgroundData = await backgroundSevice.GetData<StandardBackgroundType, DefaultTaskViewDecorData>(View);
            View.SetBackground(backgroundData.BackgroundSprite);
            View.SetHeaderColor(backgroundData.HeaderColor);

            var questionSign = ((char)ArithmeticSigns.QuestionMark).ToString();

            var expression = Model.Expression;
            var elementsParent = View.ElementsParent;
            taskElements = new List<ITaskViewComponent>(expression.Count);
            unknownElements = new List<ITaskViewComponent>(2);
            for (int i = 0; i < expression.Count; i++)
            {
                var elementType = expression[i].Type;
                var elementValue = expression[i].Value;
                var isUnknown = expression[i].IsUnknown;
                UIComponentType elementView = GetElementViewByType(elementType);

                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskViewComponent>(elementView, elementsParent);

                TaskElementState state = TaskElementState.Default;
                if (isUnknown)
                {
                    unknownElements.Add(component);
                    elementValue = questionSign;
                    state = TaskElementState.Unknown;
                }
                component.Init(i, elementValue, state);
                taskElements.Add(component);
            }

            var variants = Model.Variants;
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
            DiscardVariant(view);
            var value = view.Value;
            if (!isFirstElementSelected)
            {
                firstComponent = view;
                firstComponent.ChangeState(TaskElementState.Correct);
                unknownElements[0].ChangeValue(value);
                unknownElements[0].ChangeState(TaskElementState.Default);
                taskData = Model.UpdateModelBasedOnPlayerChoice(value);
                SelectedAnswerIndexes.Add(view.Index);
                isFirstElementSelected = true;
            }
            else
            {
                UnsubscribeInputs();
                var correctVariants = Model.CorrectVariants;
                bool isAnswerCorrect = correctVariants.Contains(value);

                if (isAnswerCorrect)
                {
                    view.ChangeState(TaskElementState.Correct);
                    unknownElements[1].ChangeState(TaskElementState.Correct);
                    unknownElements[1].ChangeValue(value);
                }
                else
                {
                    view.ChangeState(TaskElementState.Wrong);
                    unknownElements[0].ChangeState(TaskElementState.Wrong);
                    unknownElements[1].ChangeState(TaskElementState.Wrong);
                    unknownElements[1].ChangeValue(value);
                }

                IsAnswerCorrect = isAnswerCorrect;
                SelectedAnswerIndexes.Add(view.Index);

                CompleteTask();
            }
        }

        private void DiscardVariant(ITaskViewComponent component)
        {
            var clickable = (ITaskViewComponentClickable)component;
            if (taskVariants.Contains(clickable))
            {
                clickable.ON_CLICK -= DoOnClick;
                taskVariants.Remove(clickable);
            }
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
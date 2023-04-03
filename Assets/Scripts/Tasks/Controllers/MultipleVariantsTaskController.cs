using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class MultipleVariantsTaskController : BaseTaskController<IStandardTaskView, IComparisonWithMissingElementTaskModel>
    {
        private List<ITaskViewComponent> taskElements;
        private List<ITaskViewComponentClickable> taskVariants;
        private List<ITaskViewComponent> correctVariants;
        private ITaskViewComponent unknownElement;
        private string userAnswer;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }

        public MultipleVariantsTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice)
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
                var isUnknown = expression[i].IsUnknown;
                UIComponentType elementView = GetElementViewByType(elementType);

                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskViewComponent>(elementView, elementsParent);

                TaskElementState state = TaskElementState.Default;
                if (isUnknown)
                {
                    unknownElement = component;
                    elementValue = questionSign;
                    state = TaskElementState.Unknown;
                }
                component.Init(i, elementValue, state);
                taskElements.Add(component);
            }

            var variants = Model.Variants;
            var modelsCorrectVariants = Model.CorrectVariants;
            var variantsParent = View.VariantsParent;
            correctVariants = new List<ITaskViewComponent>();
            taskVariants = new List<ITaskViewComponentClickable>(variants.Count);
            for (int i = 0; i < variants.Count; i++)
            {
                var variantValue = variants[i];
                var component = await refsHolder.UIComponentProvider
                    .InstantiateFromReference<ITaskViewComponentClickable>(UIComponentType.DefaultVariant, variantsParent);
                component.Init(i, variantValue);
                component.ON_CLICK += DoOnClick;
                taskVariants.Add(component);
                if (modelsCorrectVariants.Exists(x => x == variants[i]))
                {
                    correctVariants.Add(component);
                }
            }
        }

        private void DoOnClick(ITaskViewComponent view)
        {
            UnsubscribeInputs();
            bool isAnswerCorrect = correctVariants.Contains(view);
            userAnswer = view.Value;
            if (isAnswerCorrect)
            {
                view.ChangeState(TaskElementState.Correct);
                unknownElement.ChangeState(TaskElementState.Correct);
                unknownElement.ChangeValue(userAnswer);
            }
            else
            {
                view.ChangeState(TaskElementState.Wrong);
                unknownElement.ChangeState(TaskElementState.Wrong);
                unknownElement.ChangeValue(userAnswer);
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
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using Mathy.Data;
using System;
using Mathy;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class SumOfNumbersTaskController : BaseTaskController<IStandardTaskView, ISumOfNumbersTaskModel>
    {
        private List<ITaskViewComponent> taskElements;
        private List<ITaskViewComponentClickable> taskVariants;
        private List<ITaskViewComponent> correctVariants;
        private List<string> userAnswers;
        private List<string> correctAnswers;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }

        public SumOfNumbersTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) 
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
            correctVariants = new List<ITaskViewComponent>();
            correctAnswers = new List<string>();

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
                    correctVariants.Add(component);
                    correctAnswers.Add(elementValue);
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
            var selectedAnswerValue = view.Value;
            if (userAnswers == null) userAnswers = new List<string>();
            userAnswers.Add(selectedAnswerValue);
            bool isAnswerCorrect = correctAnswers.Contains(selectedAnswerValue); 

            IsAnswerCorrect = isAnswerCorrect;

            if (SelectedAnswerIndexes == null) SelectedAnswerIndexes = new List<int>();
            SelectedAnswerIndexes.Add(view.Index);
            int unknownElementIndex = SelectedAnswerIndexes.IndexOf(view.Index);

            if (isAnswerCorrect)
            {
                view.ChangeState(TaskElementState.Correct);
                correctVariants[unknownElementIndex].ChangeState(TaskElementState.Correct);
                correctVariants[unknownElementIndex].ChangeValue(selectedAnswerValue);
            }
            else
            {
                view.ChangeState(TaskElementState.Wrong);
                correctVariants[unknownElementIndex].ChangeState(TaskElementState.Wrong);
                correctVariants[unknownElementIndex].ChangeValue(selectedAnswerValue);
            }

            if (userAnswers.Count >= Model.UnknowntElementsAmount || !isAnswerCorrect)
            {
                foreach (var variant in taskVariants) variant.IsInteractable = false;
                CompleteTask();
            }
        }

        protected override void DoOnRelease()
        {
            foreach (var variant in taskVariants)
            {
                variant.ON_CLICK -= DoOnClick;
            }
        }
    }
}
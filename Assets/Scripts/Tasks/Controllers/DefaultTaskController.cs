using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using Mathy.Data;
using System;
using Mathy;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class DefaultTaskController : BaseTaskController<IStandardTaskView, IDefaultTaskModel>
    {
        private List<ITaskViewComponent> taskElements;
        private List<ITaskViewComponentClickable> taskVariants;
        private ITaskViewComponent correctVariant;
        private string userAnswer;
        private string correctAnswer;

        protected override bool IsAnswerCorrect { get; set; }
        protected override List<int> SelectedAnswerIndexes { get; set; }

        public DefaultTaskController(IAddressableRefsHolder refsHolder, ITaskBackgroundSevice backgroundSevice) 
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
                    correctVariant = component;
                    correctAnswer = elementValue;
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
            bool isAnswerCorrect;
            userAnswer = view.Value;
            if (userAnswer.Equals(correctAnswer))
            {
                view.ChangeState(TaskElementState.Correct);
                correctVariant.ChangeState(TaskElementState.Correct);
                correctVariant.ChangeValue(correctAnswer);
                isAnswerCorrect = true;
            }
            else
            {
                view.ChangeState(TaskElementState.Wrong);
                correctVariant.ChangeState(TaskElementState.Wrong);
                correctVariant.ChangeValue(correctAnswer);
                isAnswerCorrect = false;
            }

            IsAnswerCorrect = isAnswerCorrect;
            SelectedAnswerIndexes.Add(view.Index);

            CompleteTask();
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
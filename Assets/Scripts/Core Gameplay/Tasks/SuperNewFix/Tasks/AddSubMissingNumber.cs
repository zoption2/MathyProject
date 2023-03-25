using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CustomRandom;

namespace Mathy.Core.Tasks
{
    public class AddSubMissingNumber : ArithmeticTask
    {
        public string Expression { get; protected set; }
        public int unknownElementIndex { get; protected set; }
        public AddSubMissingNumber(int seed, ScriptableTask taskSettings)
        {
            this.variants = new List<Variant>();
            this.operators = new List<Operator>();

            this.TaskSettings = taskSettings;

            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;

            CreateTaskElementsAsync();
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            int oprIndex = 0;
            List<ArithmeticSigns> signs = new List<ArithmeticSigns>() 
            { ArithmeticSigns.Plus, ArithmeticSigns.Minus };

            while (oprIndex < TaskSettings.BaseStats.OperatorsAmount - 1)
            {
                this.operators.Add(new Operator(signs[Random.Range(0, signs.Count)]));
                oprIndex++;
            }
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }

        protected override async System.Threading.Tasks.Task CreateVariants()
        {
            Expression = GetExpression;
            int expressionAnswer = MathOperations.EvaluateInt(Expression);
            this.Elements[Elements.Count - 1] = new TaskElement(expressionAnswer);

            unknownElementIndex = this.Random.Range(0, Elements.Count - 1);
            int answer = (int)this.Elements[unknownElementIndex].Value;

            this.Elements[unknownElementIndex] = new TaskElement(ArithmeticSigns.QuestionMark);
            unknownElement = this.Elements[unknownElementIndex].ElementView;


            int answerIndex = Random.Range(0, TaskSettings.BaseStats.VariantsAmount - 1);
            CorrectVariantIndexes.Add(answerIndex);

            List<int> variants = await Random.ExclusiveNumericRange(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber, TaskSettings.BaseStats.VariantsAmount, answer);

            for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
            {
                if (i == answerIndex)
                {
                    this.variants.Add(new Variant(answer, true));
                }
                else
                {
                    this.variants.Add(new Variant(variants[i], false));
                }
            }



            foreach (Variant variant in this.variants)
            {
                variant.OnPressedEvent += VariantOnPressedEvent;
            }
        }

        protected override async UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            unknownElement = this.Elements[unknownElementIndex].ElementView;
        }
    }
}
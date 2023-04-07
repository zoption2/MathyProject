using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public interface ISelectFromThreeCountTaskModel : ITaskModel
    {
        public int CorrectValue { get; }
        public List<int> Values { get; }
    }


    public class SelectFromThreeCountTaskModel : BaseTaskModel, ISelectFromThreeCountTaskModel
    {
        private const int kMaxValueLimit = 10;
        private const int kMinValueLimit = 1;
        private const int kAmountOfVariants = 3;
        private const string kQuestionSign = "?";

        public int CorrectValue { get; private set; }
        public List<int> Values { get; private set; }


        public SelectFromThreeCountTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var selectedValue = random.Next(kMinValueLimit, kMaxValueLimit + 1);
            var selecterStringValue = selectedValue.ToString();

            CorrectValue = selectedValue;

            variants = GetVariants(selectedValue, kAmountOfVariants, kMinValueLimit, kMaxValueLimit, out int correctVariantIndex);
            Values = MathOperations.ConvertStringsToInt(variants);

            correctAnswersIndexes = new List<int>()
            {
                correctVariantIndex
            };

            elements = new List<string>(kAmountOfVariants);
            for (int i = 0, j = variants.Count; i < j; i++)
            {
                string element = variants[i].Equals(selecterStringValue)
                    ? kQuestionSign
                    : variants[i];
                elements.Add(element);
            }

            operators = new List<string>();
        }


    }
}
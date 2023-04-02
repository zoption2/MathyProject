using Mathy.Data;
using System;
using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public interface ICountToTenImagesTaskModel : ITaskModel
    {
        int CountToShow { get; }
    }


    public class CountToTenImagesTaskModel : BaseTaskModel, ICountToTenImagesTaskModel
    {
        private const int kMaxValueLimit = 10;

        private int countToShow;
        private List<string> variants;
        private List<string> operators;
        private List<string> elements;
        private List<int> correctIndexes;

        public int CountToShow => countToShow;


        public CountToTenImagesTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            countToShow = random.Next(1 , kMaxValueLimit + 1);

            elements = new List<string>() 
            { 
                countToShow.ToString() 
            };

            correctIndexes = new List<int>()
            {
                countToShow - 1
            };

            operators = new List<string>()
            {
                "="
            };

            variants = new List<string>()
            {
                "1","2","3","4","5","6","7","8","9","10"
            };
        }

        public override TaskData GetResult()
        {
            var result = new TaskData();
            result.TaskType = TaskType;
            result.ElementValues = elements;
            result.OperatorValues = operators;
            result.VariantValues = variants;
            result.CorrectAnswerIndexes = correctIndexes;
            return result;
        }
    }
}
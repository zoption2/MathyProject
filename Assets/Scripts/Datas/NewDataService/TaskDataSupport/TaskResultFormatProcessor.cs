using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks;
using Mathy.Data;
using ModestTree;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Mathy.Services
{
    public interface ITaskResultFormatProcessor
    {
        List<string> GetTaskResultsFormatted(List<TaskResultData> tasks);
    }


    public class TaskResultFormatProcessor : ITaskResultFormatProcessor
    {
        private const string correctResultColor = "#15c00f";
        private const string wrongResultColor = "#f94934";
        private const string kUnknownElementValue = "?";


        Dictionary<string, string> operatorChars = new Dictionary<string, string>()
        {
            {$"{nameof(ArithmeticSigns.Plus)}", "+"},
            {$"{nameof(ArithmeticSigns.Minus)}", "-"},
            {$"{nameof(ArithmeticSigns.Multiply)}", "x"},
            {$"{nameof(ArithmeticSigns.Divide)}", ":"},
            {$"{nameof(ArithmeticSigns.GreaterThan)}", ">"},
            {$"{nameof(ArithmeticSigns.LessThan)}", "<"},
            {$"{nameof(ArithmeticSigns.Equal)}", "="},
            {$"{nameof(ArithmeticSigns.QuestionMark)}", "?"},
        };

        public List<string> GetTaskResultsFormatted(List<TaskResultData> tasks)
        {
            var results = new List<string>();

            for (int x = 0, y = tasks.Count; x < y; x++)
            {
                bool isCorrect = tasks[x].IsAnswerCorrect;
                List<string> new_elements = tasks[x].ElementValues;
                List<string> new_operators = tasks[x].OperatorValues;
                List<string> new_variants = tasks[x].VariantValues;
                List<int> new_selectedIndexes = tasks[x].SelectedAnswerIndexes;
                List<int> new_correctIndexes = tasks[x].CorrectAnswerIndexes;

                for (int i = 0; i < new_variants.Count; i++)
                {
                    if (operatorChars.ContainsKey(new_variants[i]))
                        new_variants[i] = operatorChars[new_variants[i]];
                }

                int variantIndex = 0;
                for (int i = 0; i < new_elements.Count; i++)
                {
                    if (new_elements[i] == kUnknownElementValue)
                    {
                        if (variantIndex >= new_selectedIndexes.Count)
                        {
                            new_elements[i] = "?";
                        }
                        else
                        {
                            int index = new_selectedIndexes[variantIndex];
                            new_elements[i] = $"<color={(isCorrect ? correctResultColor : wrongResultColor)}>" +
                                            $"{new_variants[index]}</color>";
                            variantIndex++;
                        }
                    }
                }

                var t = new_variants[new_selectedIndexes[0]];

                var coloredOperatorList = new_operators.Select(o =>
                {
                    if (o == kUnknownElementValue)
                    {
                        int index = new_selectedIndexes.FirstOrDefault();
                        return $"<color={(isCorrect ? correctResultColor : wrongResultColor)}>{new_variants[index]}</color>";
                    }
                    else
                        return operatorChars.ContainsKey(o) ? operatorChars[o] : o;
                }).ToList();

                StringBuilder sbResult = new StringBuilder();
                for (int i = 0, j = new_elements.Count; i < j; i++)
                {
                    sbResult.Append($"{new_elements[i]} ");
                    if (i < coloredOperatorList.Count())
                    {
                        if (!coloredOperatorList[i].IsEmpty())
                        {
                            sbResult.Append($"{coloredOperatorList[i]} ");
                        }
                    }
                }
                if (!isCorrect)
                {
                    var correctAnswersValues = new_correctIndexes.Select(i => i)
                                .Where(i => i >= 0 && i < new_variants.Count)
                                .Select(i => new_variants[i].TryLocalizeTaskVariant());
                    sbResult.Append($" ({string.Join(", ", correctAnswersValues)})");

                }
                results.Add(sbResult.ToString());
            }
            return results;
        }
    }
}


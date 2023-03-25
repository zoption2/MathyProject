using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class TaskElement : Element
    {
        public TaskElement(object value)
        {
            this.Value = value;
        }

        public override async UniTask CreateView(Transform parent, TaskType taskType)
        {
            this.viewParent = parent;

            if(Value.GetType() == typeof(ArithmeticSigns) && (ArithmeticSigns)Value == ArithmeticSigns.QuestionMark)
            {
                //Unknown element view loading
                if (taskType == TaskType.MissingExpression)
                {
                    await LoadAsset("UnknownExpressionElementView");
                }
                else
                {
                    await LoadAsset("UnknownElementViewText");
                }
            }
            else
            {
                switch (taskType)
                {
                    case TaskType t when (taskType == TaskType.Addition || taskType == TaskType.Subtraction || taskType == TaskType.Multiplication
                    || taskType == TaskType.Division || taskType == TaskType.Comparison || taskType == TaskType.ComplexAddSub
                    || taskType == TaskType.MissingNumber || taskType == TaskType.ImageOpening || taskType == TaskType.MissingSign
                    || taskType == TaskType.IsThatTrue || taskType == TaskType.ComparisonWithMissingNumber || taskType == TaskType.ComparisonMissingElements 
                    || taskType == TaskType.AddSubMissingNumber || taskType == TaskType.MissingMultipleSigns || taskType == TaskType.SumOfNumbers
                    || taskType == TaskType.MissingExpression):
                        {
                            // Base task element loading
                            await LoadAsset("ElementViewText");
                            break;
                        }
                    case TaskType.ComparisonExpressions:
                        {   // Wider version of the ElementViewText prefab
                            await LoadAsset("ElementViewExpression");
                            break;
                        }
                    case TaskType.ShapeGuessing:
                        {   // Image task element loading (need to create ElementViewImage prefab for ShapeGuessing)
                            await LoadAsset("");
                            break;
                        }
                    case TaskType.PairsNumbers:
                        {
                            await LoadAsset("PairsElementView");
                            break;
                        }
                }
            }
        }

        protected override async UniTask LoadAsset(string name)
        {
            GameObject temp;
            using (AddressableResourceLoader<GameObject> loader = new AddressableResourceLoader<GameObject>())
            {
                temp = await loader.LoadAndInstantiateSingle(name, viewParent);
            }
            this.ElementView = temp.GetComponent<TaskViewElement>();
            this.ElementView.Initialization(this);
        }

    }
}
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class Variant : Element
    {
        public bool IsVariantCorrect { get; protected set; }

        public event EventHandler OnPressedEvent;

        public Variant(object value, bool isCorrect)
        {
            this.Value = value;
            this.IsVariantCorrect = isCorrect;
        }

        public override async UniTask CreateView(Transform parent, TaskType taskType)
        {
            this.viewParent = parent;

            switch (taskType)
            {
                case TaskType t when (taskType == TaskType.Addition || taskType == TaskType.Subtraction || taskType == TaskType.Multiplication
                || taskType == TaskType.Division || taskType == TaskType.Comparison || taskType == TaskType.ComplexAddSub
                || taskType == TaskType.RandomArithmetic || taskType == TaskType.MissingNumber || taskType == TaskType.ImageOpening
                || taskType == TaskType.MissingSign || taskType == TaskType.IsThatTrue || taskType == TaskType.ComparisonWithMissingNumber
                || taskType == TaskType.ComparisonMissingElements || taskType == TaskType.AddSubMissingNumber || taskType == TaskType.MissingMultipleSigns
                || taskType == TaskType.ComparisonExpressions || taskType == TaskType.SumOfNumbers):
                    {
                        // Base virant loading
                        await LoadAsset("BaseVariantView");
                        break;
                    }
                case TaskType.MissingExpression:
                    {
                        //Variant with text loading
                        await LoadAsset("VariantViewExpression");
                        break;
                    }
                case TaskType.ShapeGuessing:
                    {
                        //Variant with text loading
                        await LoadAsset("TextVariantView");
                        break;
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
            this.ElementView = temp.GetComponent<VariantView>();
            ((VariantView)this.ElementView).Initialization(this);
            ((VariantView)this.ElementView).OnButtonPressedEvent += OnButtonPressedEvent;
        }
        
        // One event triger another
        private void OnButtonPressedEvent(object sender, EventArgs e)
        {
            this.OnPressedEvent.Invoke(this, EventArgs.Empty);
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ElementView?.Dispose();
                }

                Value = null;
                ElementView = null;
                viewParent = null;
                OnPressedEvent = null;

                disposedValue = true;
            }
        }
        #endregion

    }

}
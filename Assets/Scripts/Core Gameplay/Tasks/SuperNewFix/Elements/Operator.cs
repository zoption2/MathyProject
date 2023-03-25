using System;
using UnityEngine;
using Mathy.UI.Tasks;
using Cysharp.Threading.Tasks;

namespace Mathy.Core.Tasks
{
    public class Operator : Element
    {
        public Operator(ArithmeticSigns value)
        {
            this.Value = value;
        } 

        public override async UniTask CreateView(Transform parent, TaskType taskType)
        {
            viewParent = parent;
            await LoadAsset("OperatorView");
        }

        protected override async UniTask LoadAsset(string name)
        {
            GameObject temp;
            using (AddressableResourceLoader<GameObject> loader = new AddressableResourceLoader<GameObject>())
            {
                temp = await loader.LoadAndInstantiateSingle(name, viewParent);
            }
            this.ElementView = temp.GetComponent<OperatorView>();
            ((OperatorView)this.ElementView).Initialization(this);
        }
    }
}
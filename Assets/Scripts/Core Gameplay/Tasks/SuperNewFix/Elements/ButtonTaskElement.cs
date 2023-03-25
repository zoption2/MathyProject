using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using System;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class ButtonTaskElement : TaskElement
    {
        public event EventHandler OnPressedEvent;

        public ButtonTaskElement(object value) : base(value)
        {
            this.Value = value;
        }

        public override async UniTask CreateView(Transform parent, TaskType taskType)
        {
            this.viewParent = parent;
            switch (taskType)
            {
                case TaskType.PairsNumbers:
                    {
                        await LoadAsset("PairsElementView");
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
            this.ElementView = temp.GetComponent<ButtonTaskElementView>();
            ((ButtonTaskElementView)this.ElementView).Initialization(this);
            ((ButtonTaskElementView)this.ElementView).OnButtonPressedEvent += OnButtonPressedEvent;
        }

        // One event triger another
        private void OnButtonPressedEvent(object sender, EventArgs e)
        {
            this.OnPressedEvent.Invoke(this, EventArgs.Empty);
        }

    }
}

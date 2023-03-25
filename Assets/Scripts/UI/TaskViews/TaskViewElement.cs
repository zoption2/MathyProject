using UnityEngine.UI;
using UnityEngine;
using Mathy.Core.Tasks;
using System;

namespace Mathy.UI.Tasks
{
    public abstract class TaskViewElement : MonoBehaviour, IDisposable
    {
        #region Fields

        [Header("Components:")]

        [SerializeField] 
        protected Image backgroundImage;

        public Element Element { get; protected set; }

        protected object value;
        public virtual object Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }

        #endregion

        public virtual TaskViewElement Initialization(Element element)
        {
            this.Element = element;
            this.Value = element.Value;
            return this;
        }

        private void Awake()
        {
            GetVisualElements();
        }

        //Initializing view Element
        protected virtual void GetVisualElements()
        {
            if (backgroundImage == null)
            {
                backgroundImage = GetComponentInChildren<Image>();
            }
        }

        public virtual void SetBackgroundImage(Sprite newImage)
        {
            backgroundImage.sprite = newImage;
        }

        public virtual void Dispose()
        {
            this.gameObject.SetActive(false);
            backgroundImage = null;
            Element = null;
            Value = null;

            GameObject.Destroy(this.gameObject);
        }

    }
}
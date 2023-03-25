using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using Mathy.Core.Tasks;

namespace Mathy.UI.Tasks
{
    public enum TaskElementState
    {
        Default = 0,
        Correct = 1,
        Wrong = 2,
        Unknown = 3,
    }

    public class TextTaskViewElement : TaskViewElement
    {
        [SerializeField] protected TextMeshProUGUI textLable;
        [SerializeField] protected List<Sprite> stateImages;
		//Not shure about state
        protected TaskElementState state = 0;
        public TaskElementState State
        {
            get => state;
            set
            {
                state = value;
                UpdateState();
            }
        }

        public override object Value
        {
            get => value;
            set
            {
                this.value = value;
                //If value type is char, string or int
                if (value.GetType() == typeof(string) || 
                    value.GetType() == typeof(int) || 
                    value.GetType() == typeof(char))
                {
                    textLable.text = value.ToString();
                    textLable.transform.DOShakeRotation(Random.Range(0.5f, 1f), new Vector2(20, 60)).
                        SetEase(Ease.InOutQuad).OnComplete(() => textLable.transform.eulerAngles = Vector3.zero);
                }
            }
        }

        private void Awake()
        {
            GetVisualElements();
        }

        //Initializing view Element
        protected override void GetVisualElements()
        {
            if (textLable == null)
            {
                textLable = GetComponentInChildren<TextMeshProUGUI>();
            }
            if (backgroundImage == null)
            {
                backgroundImage = GetComponentInChildren<Image>();
            }
        }

        public virtual void SetState(TaskElementState newState)
        {
            State = newState;
        }

        private void UpdateState()
        {
            backgroundImage.sprite = stateImages[(int)state];
            backgroundImage.transform.DOPunchScale(new Vector2(-0.1f, 0.1f), 0.5f).SetEase(Ease.InOutQuad);
        }
		
        public override void Dispose()
        {
            this.gameObject.SetActive(false);
            backgroundImage = null;
            textLable = null;
            Element = null;
            value = null;

            GameObject.Destroy(this.gameObject);
        }
    }
}
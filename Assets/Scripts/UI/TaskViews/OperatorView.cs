using DG.Tweening;
using Mathy.Core.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Mathy.UI.Tasks
{
    public class OperatorView : TextTaskViewElement
    {
        [SerializeField] protected List<TMP_FontAsset> fonts;
        [SerializeField] protected Color operatorColor;

        public Color OperatorColor 
        {
            get => operatorColor;
            private set
            {
                operatorColor = value;
                textLable.color = value;
            } 
        }

        public Operator Operator { get; private set; }

        public override object Value
        {
            get => value;
            set
            {
                this.value = value;
                var type = value.GetType();
                if (type == typeof(string) || type == typeof(int) || type == typeof(char) || type == typeof(ArithmeticSigns))
                {
                    if (type == typeof(ArithmeticSigns))
                    {
                        textLable.text = System.Convert.ToChar(value).ToString();
                    }
                    else
                    {
                        textLable.text = value.ToString();
                    }
                    SetTextOffsets();
                    textLable.transform.DOShakeRotation(Random.Range(0.5f, 1f), new Vector2(20, 60)).
                        SetEase(Ease.InOutQuad).OnComplete(() => textLable.transform.eulerAngles = Vector3.zero);
                }
            }
        }

        // Fill this
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorValue"></param>
        /// <param name="color">Operator color, by defalut its #C57BE5</param>
        /// <returns></returns>
        public OperatorView Initialization(Operator operatorValue, Color color)
        {
            this.Operator = operatorValue;
            this.Value = (char)operatorValue.Value;
            this.OperatorColor = color;
            return this;
        }

        //Default color
        public OperatorView Initialization(Operator operatorValue)
        {
            this.Operator = operatorValue;
            this.Value = (char)(ArithmeticSigns)operatorValue.Value;
            this.OperatorColor = new Color().FromHexString("#C57BE5");
            return this;
        }

        //Need for fixind font offsets
        protected void SetTextOffsets()
        {
            float bottom = textLable.rectTransform.GetBottom();
            float defaultSize = textLable.fontSize;            
            float size = defaultSize;

            switch (textLable.text)
            {
                case "+":
                    bottom = 24;
                    break;
                case "-":
                    bottom = 24;
                    break;
                case "X":
                    size = 100;
                    break;
                case ":":
                    bottom = 32;
                    break;
                case "<":
                    bottom = 48;
                    break;
                case ">":
                    bottom = 48;
                    break;
            }
            textLable.rectTransform.SetBottom(bottom);
            textLable.fontSize = size;
        }

        public override void SetState(TaskElementState newState)
        {
            State = newState;
            if (State == TaskElementState.Default)
                textLable.font = fonts[0];
            else
                textLable.font = fonts[1];
        }

        public void SetAsGoal()
        {
            this.textLable.text = "?";
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
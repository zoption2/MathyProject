using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI.Tasks
{
    public class ImageElementView : TaskViewElement
    {
        [SerializeField]
        private Image image;

        public override object Value 
        { 
            get => this.value;
            set
            {
                this.value = value;
                //Setting image here
                image.sprite = (Sprite)value;
            }
        }



        //Incorrect dispose here, need to be rewrited
        public override void Dispose()
        {
            this.gameObject.SetActive(false);
            backgroundImage = null;
            image = null;
            Element = null;
            value = null;

            GameObject.Destroy(this.gameObject);
        }

    }

}
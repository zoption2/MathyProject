using UnityEngine;
using Mathy.UI.Tasks;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class TaskElementViewWithMaterials :  TaskElementView
    {
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material answerMaterial;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color answerColor;

        public override void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            base.Init(index, value, initedState);
            valueText.fontMaterial = defaultMaterial;
            valueText.color = defaultColor;
        }

        protected override void DoOnStateChanged(TaskElementState state)
        {
            if (state != TaskElementState.Default)
            {
                valueText.fontMaterial = answerMaterial;
                valueText.color = answerColor;
            }
            else
            {
                valueText.fontMaterial = defaultMaterial;
                valueText.color = defaultColor;
            }
        }
    }
}


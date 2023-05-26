using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class TaskElementViewValueColor : TaskElementView
    {
        [SerializeField] private Color[] colors;


        public override void Init(int index, string value, TaskElementState initedState = TaskElementState.Default)
        {
            base.Init(index, value, initedState);
            var colorIndex = (int)initedState;
            valueText.color = colors[colorIndex];
        }

        protected override void DoOnStateChanged(TaskElementState state)
        {
            var colorIndex = (int)state;
            valueText.color = colors[colorIndex];
        }
    }
}


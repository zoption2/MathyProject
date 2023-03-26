using UnityEngine;
using System;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskView : IView
    {
        event Action ON_EXIT_CLICK;
        public void SetTitle(string title);
        public void SetDescription(string description);
        public void SetBackground(Sprite image);
    }
}


using System;

namespace Mathy.Data
{
    public class SaveEventArgs : EventArgs
    {
        public SaveEventArgs()
        {  
            this.TaskData = new TaskResultData();
        }
        public TaskResultData TaskData { get; set; }


        public new static readonly SaveEventArgs Empty = null;
    }
}

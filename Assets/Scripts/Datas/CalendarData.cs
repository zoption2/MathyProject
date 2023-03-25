using System;
using System.Collections.Generic;

namespace Mathy.Data
{
    public class CalendarData
    {
        public CalendarData(DateTime date)
        {
            this.Date = date;
            this.ModeData = new Dictionary<TaskMode, bool>();
        }
        public DateTime Date { get; private set; }

        public Dictionary<TaskMode, bool> ModeData { get; set; }//rename it to smt better
    }
}
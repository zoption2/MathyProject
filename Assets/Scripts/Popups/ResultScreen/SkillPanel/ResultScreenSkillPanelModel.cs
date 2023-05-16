using Mathy.Data;
using System.Collections.Generic;


namespace Mathy.UI
{
    public class ResultScreenSkillPanelModel : IModel
    {
        public string LocalizedTitle;
        public string TotalLocalized;
        public int TotalTasks;
        public int TotalCorrect;
        public int MiddleRate;
        public Dictionary<SkillType, SkillResultProgressModel> SkillsProgressModels;
    }
}



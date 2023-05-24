using System.Collections.Generic;

namespace Mathy.UI
{
    public class SkillPlanTabModel : IModel
    {
        public int Grade { get; set; }
        public List<BaseSkillSettingModel> SkillsSettings { get; set; }
    }
}



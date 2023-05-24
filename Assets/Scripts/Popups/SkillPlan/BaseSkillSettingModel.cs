using Mathy.Data;

namespace Mathy.UI
{
    public class BaseSkillSettingModel : IModel
    {
        public SkillType SkillType { get; set; }
        public string LocalizedTitle { get; set; }
        public int Value { get; set; }
        public bool IsEnable { get; set; }
    }
}



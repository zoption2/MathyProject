namespace Mathy.Data
{
    public class SkillSettingsData
    {
        public int Grade { get; set; } = 1;
        public SkillType Skill { get; set; }
        public bool IsEnabled { get; set; } = true;
        public int Value { get; set; } = 20;
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 100;
    }
}


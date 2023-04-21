using System;

namespace Mathy.Data
{
    //potential replacemant for TaskSettings ScriptableObject
    [Serializable]
    public class TaskParamsData
    {
        public int Grade { get; set; } = 1;
        public int Version { get; set; } = 1;
        public SkillType SkillType { get; set; }
        public int TimesPlayed { get; set; }
        public int ElementsAmount { get; set; }
        public int VariantsAmount { get; set; } = 4;
        public int MinValue { get; set; } = 0;
        public int MaxValue { get; set; } = 100;
        public int MinLimit { get; set; } = 0;
        public int MaxLimit { get; set; } = 100;
        public string TitleKey { get; set; }
        public string DescriptionKey { get; set; } = "Empty description";

    }
}


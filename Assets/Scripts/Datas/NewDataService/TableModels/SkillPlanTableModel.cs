namespace Mathy.Data
{
    public class SkillPlanTableModel
    {
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Skill { get; set; }
        public bool IsEnabled { get; set; }
        public int Value { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}


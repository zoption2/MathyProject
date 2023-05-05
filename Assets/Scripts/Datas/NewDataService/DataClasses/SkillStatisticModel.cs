namespace Mathy.Data
{
    public class SkillStatisticModel
    {
        public int ID { get; set; }
        public string Skill { get; set; }
        public int SkillIndex { get; set; }
        public int Total { get; set; }
        public int Correct { get; set; }
        public int Rate { get; set; }
        public double Duration { get; set; }
        public int Grade { get; set; }

        public bool IsCorrectRequest { get; set; }
    }
}


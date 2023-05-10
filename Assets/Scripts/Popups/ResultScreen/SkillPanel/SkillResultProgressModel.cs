using Mathy.Data;


namespace Mathy.UI
{
    public class SkillResultProgressModel : IModel
    {
        public SkillType SkillType;
        public string LocalizedTitle;
        public int TotalPlayed;
        public int TotalCorrect;
        public int CorrectRate;
    }
}



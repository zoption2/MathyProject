using Mathy.Services;


namespace Mathy.UI
{
    public interface ISkillPlanSecondGradeController : ISkillPlanGradeController
    {

    }


    public class SkillPlanSecondGradeController : BaseSkillPlanGradeController, ISkillPlanSecondGradeController
    {
        public override int Grade => 2;
        protected override SkillPlanPopupComponents SettingsComponent => SkillPlanPopupComponents.SkillSettingsUpTo100;


        public SkillPlanSecondGradeController(ISkillPlanService skillPlanService, IAddressableRefsHolder refsHolder) : base(skillPlanService, refsHolder)
        {
        }
    }
}

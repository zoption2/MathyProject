using Mathy.Services;


namespace Mathy.UI
{
    public interface ISkillPlanFirstGradeController : ISkillPlanGradeController
    {

    }


    public class SkillPlanFirstGradeController : BaseSkillPlanGradeController, ISkillPlanFirstGradeController
    {
        public override int Grade => 1;
        protected override SkillPlanPopupComponents SettingsComponent => SkillPlanPopupComponents.SkillSettingsUpTo100;


        public SkillPlanFirstGradeController(ISkillPlanService skillPlanService, IAddressableRefsHolder refsHolder) : base(skillPlanService, refsHolder)
        {
        }
    }
}

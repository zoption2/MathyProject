using Mathy.Services;


namespace Mathy.UI
{
    public interface ISkillPlanSecondGradeController : ISkillPlanGradeController
    {

    }


    public class SkillPlanSecondGradeController : BaseSkillPlanGradeController, ISkillPlanSecondGradeController
    {
        public override int Grade => 2;
        protected override SkillPlanPopupComponents SettingsComponent => SkillPlanPopupComponents.SkillSettingsUpTo1000;
        protected override int _minValue => 100;
        protected override int _maxValue => 1000;
        protected override int _valueMultiplier => 100;

        public SkillPlanSecondGradeController(ISkillPlanService skillPlanService, IAddressableRefsHolder refsHolder) : base(skillPlanService, refsHolder)
        {
        }
    }
}

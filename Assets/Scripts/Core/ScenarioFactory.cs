using Zenject;

namespace Mathy.Core.Tasks
{
    public interface IScenarioFactory
    {
        IScenario GetScenario(TaskMode mode);
    }


    public class ScenarioFactory : IScenarioFactory
    {
        private DiContainer container;
        public ScenarioFactory(DiContainer container)
        {
            this.container= container;
        }

        public IScenario GetScenario(TaskMode mode)
        {
            IScenario scenario;
            switch (mode)
            {
                case TaskMode.Small:
                    scenario = container.Resolve<SmallScenario>();
                    break;

                case TaskMode.Medium:
                    scenario = container.Resolve<MediumScenario>();
                    break;

                case TaskMode.Large:
                    scenario = container.Resolve<LargeScenario>();
                    break;

                //case TaskMode.Challenge:
                //    break;
     
                case TaskMode.Practic:
                    scenario = container.Resolve<PracticeScenario>();
                    break;
                default:
                    throw new System.ArgumentException(
                        string.Format("Scenario for TaskMode {0} doesn't exist of not implemented at {1}"
                        , mode, typeof(ScenarioFactory))
                        );
            }

            return scenario;
        }
    }
}



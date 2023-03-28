using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public interface IGameplayService
    {
        public void StartGame(TaskMode mode, List<ScriptableTask> availableTasks);
    }

    public class GameplayService : IGameplayService
    {
        private IScenarioFactory scenarioFactory;
        private IScenario currentScenario;
        public GameplayService(IScenarioFactory scenarioFactory) 
        {
            this.scenarioFactory = scenarioFactory;
        }

        public void StartGame(TaskMode mode, List<ScriptableTask> availableTasks)
        {
            currentScenario = scenarioFactory.GetScenario(mode);
            currentScenario.StartScenario(availableTasks);
        }
    }
}



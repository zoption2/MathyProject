using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    [CreateAssetMenu(fileName = "DefaultTaskViewDecorData", menuName = "ScriptableObjects/DefaultTaskViewDecorData")]
    public class DefaultTaskViewDecorData : BaseTaskViewDecorData
    {
        [field: SerializeField] public Color HeaderColor { get; private set; }
    }
}


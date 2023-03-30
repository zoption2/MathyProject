using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public interface ITaskViewDecorData
    {

    }


    [CreateAssetMenu(fileName = "BaseTaskViewDecorData", menuName = "ScriptableObjects/BaseTaskViewDecorData")]
    public class BaseTaskViewDecorData : ScriptableObject, ITaskViewDecorData
    {
        [field: SerializeField] public Sprite BackgroundSprite { get; private set; }
    }
}


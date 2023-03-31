using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    [CreateAssetMenu(fileName = "VariantOneTaskViewDecorData", menuName = "ScriptableObjects/VariantOneTaskViewDecorData")]
    public class VariantOneTaskViewDecorData : BaseTaskViewDecorData
    {
        [field: SerializeField] public Sprite HeaderSprite { get; private set; }
        [field: SerializeField] public Sprite HolderSprite { get; private set; }
    }
}


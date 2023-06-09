using UnityEngine;

/// <summary>
/// Create a scriptable task with basic settings preset
/// </summary>
[CreateAssetMenu(fileName = "New Scriptable Task", menuName = "ScriptableObjects/Task Preset")]
public class ScriptableTask : ScriptableTaskBase
{
    [SerializeField]
    public TaskType TaskType;
}
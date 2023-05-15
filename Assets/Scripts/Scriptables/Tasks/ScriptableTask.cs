using Mathy.Data;
using UnityEngine;

/// <summary>
/// Create a scriptable task with basic settings preset
/// </summary>
[CreateAssetMenu(fileName = "New Scriptable Task", menuName = "ScriptableObjects/Task Preset")]
public class ScriptableTask : ScriptableTaskBase
{
    public TaskType TaskType;
    public SkillType SkillType;
    public int Grade = 1;
}
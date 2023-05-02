using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathy.Data
{
    /// <summary>
    /// Grade settings preset containing a list of skill settings
    /// </summary>
    [CreateAssetMenu(fileName = "New Grade Settings", menuName = "ScriptableObjects/Grade Settings")]
    public class GradeSettings : ScriptableObject
    {
        public List<SkillSettings> SkillSettings;
    }

    [Serializable]
    public struct SkillSettings
    {
        public SkillType SkillType;
        public List<ScriptableTask> TaskSettings;

        public SkillSettings(SkillType skillType, List<ScriptableTask> taskSettings)
        {
            this.SkillType = skillType;
            this.TaskSettings = taskSettings;
        }
    }

    public enum SkillType
    {
        Count = 10,
        Addition = 11,
        Subtraction = 12,
        Comparison= 13,
        Shapes = 14,
        Sorting = 15,
        Complex = 16
    }
}
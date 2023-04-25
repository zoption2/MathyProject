using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathy.Data
{
    /// <summary>
    /// Represents the learning subdivisions that controls the task types that will be available in the game
    /// </summary>
    [Serializable]
    public struct GradeData
    {
        public int GradeIndex;
        public bool IsActive;
        public List<SkillData> SkillDatas;
    }

    [Serializable]
    public class SkillData
    {
        public SkillSettingsData Settings;
        public List<ScriptableTask> Tasks;
    }
}
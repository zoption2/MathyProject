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

        public GradeData(int index, bool isActive, List<SkillData> skillDatas)
        {
            this.GradeIndex = index;
            this.IsActive = isActive;
            this.SkillDatas = skillDatas;
        }
    }

    [Serializable]
    public struct SkillData
    {
        public SkillType SkillType;
        public bool IsActive;
        public int MaxNumber;
        public List<ScriptableTask> TaskSettings;

        public SkillData(SkillType skillType, bool isActive, int maxNumber, List<ScriptableTask> taskSettings)
        {
            this.SkillType = skillType;
            this.IsActive = isActive;
            this.MaxNumber = maxNumber;
            this.TaskSettings = taskSettings;
        }
    }
}
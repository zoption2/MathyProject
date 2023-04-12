using Mathy.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class GradeManager : StaticInstance<GradeManager>
{
    [Header("REFERENCES:")]
    [SerializeField] private List<GradeSettings> gradeSettings;
    [SerializeField] List<GradeData> gradeDatas;

    public List<GradeData> GradeDatas
    {
        get => gradeDatas;
    }

    public bool IsAnySkillActivated
    {
        get
        {
            return gradeDatas
            .Where(g => g.IsActive)
            .SelectMany(g => g.SkillDatas)
            .Any(s => s.IsActive);
        }
    }
    public bool IsInitialized { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
        _ = Initialize();
    }

    private async UniTask Initialize()
    {
        await UniTask.WaitUntil(() => DataManager.Instance != null);
        gradeDatas = await DataManager.Instance.GetGradeDatas(gradeSettings);
        IsInitialized = true;
    }

    public List<ScriptableTask> AvailableTaskSettings()
    {
        List<ScriptableTask> taskSettings = new List<ScriptableTask>();

        taskSettings = gradeDatas
            .Where(g => g.IsActive)
            .SelectMany(g => g.SkillDatas)
            .Where(s => s.IsActive)
            .SelectMany(s => s.TaskSettings.Where(t => t.MaxLimit <= s.MaxNumber))
            .ToList();
        return taskSettings;
    }

    public void SetSkillIsActive(int gradeIndex, int skillIndex, bool isActive)
    {
        var gradeData = gradeDatas.FirstOrDefault(g => g.GradeIndex == gradeIndex);
        var skillData = gradeData.SkillDatas[skillIndex];
        skillData.IsActive = isActive;
        gradeData.SkillDatas[skillIndex] = skillData;
        _ = DataManager.Instance.SaveGradeDatas(gradeDatas);
    }

    public void SetSkillMaxNumber(int gradeIndex, int skillIndex, int maxNumber)
    {
        var gradeData = gradeDatas.FirstOrDefault(g => g.GradeIndex == gradeIndex);
        var skillData = gradeData.SkillDatas[skillIndex];
        skillData.MaxNumber = maxNumber;
        skillData.TaskSettings.ForEach(task => task.MaxNumber = skillData.MaxNumber);
        gradeData.SkillDatas[skillIndex] = skillData;
        _ = DataManager.Instance.SaveGradeDatas(gradeDatas);
    }
}
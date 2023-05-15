using CustomRandom;
using Mathy.Core;
using Mathy.Core.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mathy.Core.Tasks.DailyTasks;

namespace Mathy.Data
{
    /// <summary>
    /// Task generator that used to generate tasks with speciffic seed.
    /// Due to the fact that IDisposable is better use it with using, 
    /// or dispose it after usage and store somehow the seed data if needed
    /// </summary>
    public class TaskGenerator : IDisposable
    {
        private Task GetTaskBySettings(int seed, ScriptableTask taskSettings)
        {
            switch (taskSettings.TaskType)
            {
                //case TaskType t when (tempTask.TaskType == TaskType.Addition || tempTask.TaskType == TaskType.ComplexAddSub):
                case TaskType.Addition:
                    {
                        return new Addition(seed, taskSettings);
                    }
                case TaskType.Subtraction:
                    {
                        return new Subtraction(seed, taskSettings);
                    }
                case TaskType.Comparison:
                    {
                        return new Comparison(seed, taskSettings);
                    }
                case TaskType.MissingNumber:
                    {
                        return new MissingNumber(seed, taskSettings);
                    }
                case TaskType.MissingSign:
                    {
                        return new MissingSign(seed, taskSettings);
                    }
                case TaskType.IsThatTrue:
                    {
                        return new IsThatTrue(seed, taskSettings);
                    }
                case TaskType.AddSubMissingNumber:
                    {
                        return new AddSubMissingNumber(seed, taskSettings);
                    }
                case TaskType.ComparisonWithMissingNumber:
                    {
                        return new ComparisonWithMissingNumber(seed, taskSettings);
                    }
                case TaskType.ComparisonMissingElements:
                    {
                        return new ComparisonMissingElements(seed, taskSettings);
                    }
                case TaskType.ComparisonExpressions:
                    {
                        return new ComparisonExpressions(seed, taskSettings);
                    }
                case TaskType.SumOfNumbers:
                    {
                        return new SumOfNumbers(seed, taskSettings);
                    }
                case TaskType.MissingExpression:
                    {
                        return new MissingExpression(seed, taskSettings);
                    }
            }
            Debug.LogError("There is no tasks available");
            return null;
        }

        //public async System.Threading.Tasks.Task<Task> GenerateSingle()
        //{
        //    System.Random sysRandom = new System.Random();
        //    int seed = sysRandom.Next(0, Int32.MaxValue);

        //    FastRandom random = new FastRandom(seed);

        //    List<ScriptableTask> tastSettings;
        //    using (AddressableResourceLoader<ScriptableTask> loader = new AddressableResourceLoader<ScriptableTask>())
        //    {
        //        tastSettings = await loader.LoadListByAssetLable("Beginner");
        //    }

        //    if (tastSettings.Count != 0)
        //    {
        //        ScriptableTask tempTask = tastSettings[random.Range(0, tastSettings.Count)];
        //        return GetTaskBySettings(seed, tempTask);
        //    }

        //    Debug.Log("There is no tasks available");
        //    return null;
        //}

        //Return a list of tasks based on skill settings
        public async System.Threading.Tasks.Task<Task> GenerateSingleFromSkillList(List<ScriptableTask> tastSettings)
        {
            System.Random sysRandom = new System.Random();
            int seed = sysRandom.Next(0, Int32.MaxValue);

            FastRandom random = new FastRandom(seed);

            if (tastSettings.Count != 0)
            {
                ScriptableTask tempTask = tastSettings[random.Range(0, tastSettings.Count)];
                return GetTaskBySettings(seed, tempTask);
            }

            Debug.Log("There is no tasks available");
            return null;
        }

        public async System.Threading.Tasks.Task<Task> GenerateSingleBySetting(ScriptableTask taskSetting)
        {
            System.Random sysRandom = new System.Random();
            int seed = sysRandom.Next(0, Int32.MaxValue);
            return GetTaskBySettings(seed, taskSetting);
        }

        //public async System.Threading.Tasks.Task<Task> GenerateSingleFromSeed(int seed)
        //{
        //    System.Random sysRandom = new System.Random();

        //    FastRandom random = new FastRandom(seed);

        //    List<ScriptableTask> tastSettings;
        //    using (AddressableResourceLoader<ScriptableTask> loader = new AddressableResourceLoader<ScriptableTask>())
        //    {
        //        tastSettings = await loader.LoadListByAssetLable("Beginner");
        //    }

        //    if (tastSettings.Count != 0)
        //    {
        //        ScriptableTask tempTask = tastSettings[random.Range(0, tastSettings.Count)];
        //        return GetTaskBySettings(seed, tempTask);
        //    }

        //    Debug.Log("There is no tasks available");
        //    return null;
        //}

        //public async System.Threading.Tasks.Task<List<Task>> Generate(int amount)
        //{
        //    if (amount > 0)
        //    {
        //        List<Task> tasks = new List<Task>();

        //        for (int i = 0; i < amount; i++)
        //        {
        //            tasks.Add(await GenerateSingle());
        //        }
        //        return tasks;
        //    }
        //    else
        //    {
        //        Debug.LogError("The number of requested tasks is less or equal 0");
        //        throw new ArgumentException();
        //    }
        //}

        //public async System.Threading.Tasks.Task<List<Task>> GenerateByAvailableSkills(int amount)
        //{
        //    if (amount > 0)
        //    {
        //        List<Task> tasks = new List<Task>();
        //        List<ScriptableTask> tastSettings;
        //        tastSettings = GradeManager.Instance.AvailableTaskSettings();
        //        for (int i = 0; i < amount; i++)
        //        {
        //            tasks.Add(await GenerateSingleFromSkillList(tastSettings));
        //        }
        //        return tasks;
        //    }
        //    else
        //    {
        //        Debug.LogError("The number of requested tasks is less or equal 0");
        //        throw new ArgumentException();
        //    }
        //}

        public async System.Threading.Tasks.Task<List<Task>> GenerateBySetting(ScriptableTask taskSetting, int amount)
        {
            if (amount > 0)
            {
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < amount; i++)
                {
                    tasks.Add(await GenerateSingleBySetting(taskSetting));
                }
                return tasks;
            }
            else
            {
                Debug.LogError("The number of requested tasks is less or equal 0");
                throw new ArgumentException();
            }
        }

        //public async System.Threading.Tasks.Task<List<Task>> GenerateFromSeeds(List<int> seeds)
        //{
        //    if (seeds.Count > 0)
        //    {
        //        List<Task> tasks = new List<Task>();

        //        for (int i = 0; i < seeds.Count; i++)
        //        {
        //            tasks.Add(await GenerateSingleFromSeed(seeds[i]));
        //        }
        //        return tasks;
        //    }
        //    else
        //    {
        //        Debug.LogError("The number of requested tasks is less or equal 0");
        //        throw new ArgumentException();
        //    }
        //}

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
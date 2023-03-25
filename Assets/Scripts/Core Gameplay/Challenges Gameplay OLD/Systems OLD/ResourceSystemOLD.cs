using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Mathy.Core;

//Todo: rewrite summary
/// <summary>
/// One repository for all script
/// </summary>
public class ResourceSystemOLD : StaticInstance<ResourceSystemOLD>
{
    /*
    private Dictionary<TaskDifficulty, List<ScriptableTask>> Tasks;

    #region Initialization

    protected override void Awake()
    {
        base.Awake();
        Tasks = new Dictionary<TaskDifficulty, List<ScriptableTask>>();
        AssembleResources();
    }


    private void AssembleResources()
    {
        foreach (TaskDifficulty difficulty in (TaskDifficulty[])Enum.GetValues(typeof(TaskDifficulty)))
        {
            List<ScriptableTask> temp = new List<ScriptableTask>();
            temp = Resources.LoadAll<ScriptableTask>("Tasks/" + difficulty.ToString()).ToList();
            Tasks.Add(difficulty, temp);
        }
    }

    #endregion

    //Возвращает список Типов задачь которые доступны в текущий момент
    public List<TaskType> GetCurrentTaskTypes()
    {
        List<TaskType> types = new List<TaskType>();
        types = Tasks[GameManager.Instance.DifficultyMode].Select(d=> d.TaskType).ToList();

        return types;
    }

    //Get all available tasks
    public List<ScriptableTask> GetTasks()
    {
        List<ScriptableTask> tasks;
        tasks = Tasks[GameManager.Instance.DifficultyMode];

        if (tasks.Count() > 0)
        {
            return tasks;
        }
        else
        {
            Debug.Log("Can't find any task");
            return null;
        }
    }

    // Getting list of tasks by given type
    public List<ScriptableTask> GetTasks(TaskType type)
    {
        List<ScriptableTask> tasks = new List<ScriptableTask>();
        tasks = Tasks[GameManager.Instance.DifficultyMode].Where(d => d.TaskType == type).ToList();

        if (tasks.Count() > 0)
        {
            return tasks;
        }
        else
        {
            Debug.Log("Can't find any " + type + " task!");
            return null;
        }
    }

    public List<ScriptableTask> GetTasks(int maxNumber)
    {
        List<ScriptableTask> tasks;
        tasks = Tasks[GameManager.Instance.DifficultyMode].Where(d => d.BaseStats.MaxNumber == maxNumber).ToList();

        if (tasks.Count() > 0)
        {
            return tasks;
        }
        else
        {
            Debug.Log("Can't find any task with maxNumber = " + maxNumber);
            return null;
        }
    }

    public List<ScriptableTask> GetTasks(TaskType type, int maxNumber)
    {
        List<ScriptableTask> tasks;
        tasks = Tasks[GameManager.Instance.DifficultyMode].Where(d => d.TaskType == type && d.BaseStats.MaxNumber == maxNumber).ToList();

        if (tasks.Count() > 0)
        {
            return tasks;
        }
        else
        {
            Debug.Log("Can't find any " + type + " task with maxNumber = " + maxNumber);
            return null;
        }
    }

    //Getting random task
    public ScriptableTask GetTask()
    {
        List<ScriptableTask> tempList = GetTasks();
        return tempList[UnityEngine.Random.Range(0, Tasks.Count)];
    }

    //Getting random task by type
    public ScriptableTask GetTask(TaskType type)
    {
        List<ScriptableTask> tasks = GetTasks(type);
        if (tasks != null && tasks.Count() > 0)
        {
            return tasks[UnityEngine.Random.Range(0, tasks.Count)];
        }
        else
        {
            Debug.Log("Can't find any " + type + " task!");
            return null;
        }
    }

    public ScriptableTask GetTask(TaskType type, int maxNumber)
    {
        List<ScriptableTask> tasks = GetTasks(type);
        ScriptableTask task;
        if (tasks.Count() > 0)
        {
            task = tasks.First(t => t.BaseStats.MaxNumber == maxNumber);
            return task;
        }
        else
        {
            Debug.Log("Can't find any " + type + " task with " + maxNumber + "!");
            throw new ArgumentException();
        }
    }
    */
}
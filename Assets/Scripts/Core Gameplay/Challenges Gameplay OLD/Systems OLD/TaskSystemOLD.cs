using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Find out why we have TaskSystem taskManager and so on
public class TaskSystemOLD : StaticInstance<TaskSystemOLD>
{
    [SerializeField] private Transform chalengesPanel;

    private List<TaskOLD> tasks;
    private List<TaskOLD> challenges;

    //Возможно главная проблема генерации и регенерации тасков у меня лежит вот тут
    private System.Random random = new System.Random();
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        challenges = chalengesPanel.GetComponents<TaskOLD>().ToList();
    }

    public bool IsChallengeOfTypeExits(TaskType type) => challenges.Any(task => task.TaskType == type);

    public TaskOLD GetTask(TaskType type) => tasks.First(task => task.TaskType == type);

    public TaskOLD GetRandomTask() => tasks[random.Next(0, tasks.Count - 1)];

    public TaskOLD GetChallenge(TaskType type) => challenges.First(challenge => challenge.TaskType == type);

    public TaskOLD GetRandomChallenge() => challenges[random.Next(0, challenges.Count-1)];
    
}

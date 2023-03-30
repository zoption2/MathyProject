using Mathy.Core.Tasks;
using UnityEngine;
using System;

/// <summary>
/// Represent the mode of Task which make effect on amount of task
/// </summary>
public enum TaskMode
{
    Small = 0,
    Medium = 1,
    Large = 2,
    Challenge = 3,
    Practic = 4
}

public enum TaskType
{
    Addition = 0,
    Subtraction = 21,
    Comparison = 1,
    Multiplication = 2,
    Division = 3,
    ComplexAddSub = 4,
    RandomArithmetic = 5,
    MissingNumber = 6,
    ImageOpening = 7,
    PairsNumbers = 8,
    PairsEquation = 9,
    PairsOperands = 10,
    ShapeGuessing = 11,
    MissingSign = 12,
    MissingMultipleSigns = 13,
    IsThatTrue = 14,
    ComparisonWithMissingNumber = 15,
    ComparisonMissingElements = 16,
    AddSubMissingNumber = 17,
    ComparisonExpressions = 18,
    SumOfNumbers = 19,
    MissingExpression = 20,
    CountTo10Images = 22,
}

[Serializable]
public struct Stats
{
    // The amount of task operators
    public int OperatorsAmount;
    // The amount of task Elements
    public int ElementsAmount;
    // The amount of task answer variants
    public int VariantsAmount;
    public int MinNumber;
    public int MaxNumber;

    public Stats(int operatorsAmount, int elementsAmount, int variantsAmount, int minNumber, int maxNumber)
    {
        this.OperatorsAmount = operatorsAmount;
        this.ElementsAmount = elementsAmount;
        this.VariantsAmount = variantsAmount;
        this.MinNumber = minNumber;
        this.MaxNumber = maxNumber;
    }
}

public abstract class ScriptableTaskBase : ScriptableObject
{
    [SerializeField] private Stats _stats;
    public Stats BaseStats
    {
        get
        {
            if (!TaskManager.Instance.IsPractice)
            {
                return _stats;
            }
            else
            {
                Stats settingsStats = new Stats(
                    _stats.OperatorsAmount,
                    _stats.ElementsAmount,
                    _stats.VariantsAmount,
                    _stats.MinNumber,
                    GameSettingsManager.Instance.MaxNumber);
                return settingsStats;
            }
        }
    }

    public string Title = "Title";
    public string Description = "Description of the task should be here";
    public Sprite bgImage;
    public Color operatorColor = Color.blue;
    public Color panelColor = Color.yellow;
}
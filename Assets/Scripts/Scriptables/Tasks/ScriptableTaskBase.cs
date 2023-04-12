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
    SelectFromThreeCount = 23,
    CountTo10Frames = 24,
    CountTo20Frames = 25,
}

public abstract class ScriptableTaskBase : ScriptableObject
{
    public int ElementsAmount;
    public int VariantsAmount;
    public int MinNumber;
    public int MaxNumber;
    public int MinLimit = 0;
    public int MaxLimit = 100;
    public string Title = "Title";
    public string Description = "Description of the task should be here";
}
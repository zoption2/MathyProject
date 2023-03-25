using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathOperatorOLD : TaskElementOLD
{
    private List<string> addSubOperators = new List<string>() { "+", "-" };
    public List<string> ComparisonOperators = new List<string>() { "<", "=", ">" };
    private string equals = "=";

    public void SetRandom()
    {
        string randomOperator = addSubOperators[UnityEngine.Random.Range(0, addSubOperators.Count)];
        SetText(randomOperator);
    }

    public void SetRandomComp()
    {
        string randomOperator = ComparisonOperators[UnityEngine.Random.Range(0, addSubOperators.Count)];
        SetText(randomOperator);
    }

    public void SetEquals()
    {
        SetText(equals);
    }

    protected override void ValueUpdate()
    {
        value = textLable.text;
    }

    public override void SetAsGoal()
    {
        SetText("?");
        SetTextStyle(operatorFont, Color.white);
        DoTextTween();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathElementOLD : TaskElementOLD
{
    public override void SetAsGoal()
    {
        SetImage(answerImage);
        SetText("?");
        SetTextStyle(operatorFont, Color.white);
        DoTextTween();
    }
}

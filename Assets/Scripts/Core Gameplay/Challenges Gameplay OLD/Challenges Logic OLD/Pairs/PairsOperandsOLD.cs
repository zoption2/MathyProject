using System.Collections;
using System.Collections.Generic;
using Fallencake.Tools;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Mathy.Core;

public class PairsOperandsOLD : ChallengeOLD
{
    #region Fields

    public override TaskType TaskType { get; } = TaskType.PairsOperands;
    private List<AnswerVariantOLD> selectedVariants = new List<AnswerVariantOLD>();
    private List<AnswerVariantOLD> operandsVariants;
    private MathElementOLD answerElement;

    #endregion

    public override void RunTask()
    {
        Initialization();
        UpdateDisplayStyle();
        GenerateElements();
        GenerateVariants();
        SetAnswer();
        //UpdateTextStyle();
        SubscribeOnVariants();
        SetLives();
    }

    protected override void UpdateDisplayStyle()
    {
        ShowTaskPanel(true);
        ShowTimer(true, LeftPanel);
        ShowLivesPanel(true, RightPanel);
        ShowBGImage(true, BGImage.sprite);
        ShowVariantsPanel(true, 12, new Vector2(10, 10));
    }

    protected override void GenerateElements()
    {
        ClearPanels();
        InstantiateElements();
        SetElementsValues();
    }

    protected override void SetElementsValues()
    {
        foreach (MathElementOLD element in elements)
        {
            element.SetAsGoal();
        }
    }

    protected override void SetAnswer()
    {
        answerElement = Instantiate(elementPrefab, TaskPanel).GetComponent<MathElementOLD>();
        ResetGoal();
    }

    private void ResetOperands()
    {
        operandsVariants = Enumerable.Range(0, variants.Count)
       .OrderBy(i => Random.value)
       .Select(i => variants[i])
       .Take(ElementsAmount).ToList();
        SetOperatorsValues();
    }

    private void ResetGoal()
    {
        ResetOperands();
        CalculateExpression();
        answerElement.SetText(Answer.ToString());
    }

    public override void CalculateExpression()
    {
        string expression = Expression(operandsVariants);
        Answer = (int)Evaluate(expression);

        if (Answer > MaxNumber || (onlyPositive && Answer < 0))
        {
            ResetGoal();
        }
    }

    private string Expression(List<AnswerVariantOLD> operands)
    {
        string expression = "";
        for (int i = 0; i < elements.Count; i++)
        {
            expression += operands[i].value;
            if (i < operators.Count - 1) expression += operators[i].value;
        }
        return expression;
    }

    private async void CorrectVariant(List<AnswerVariantOLD> selectedVariants)
    {
        VibrationManager.Instance.TapPeekVibrate();
        var tasks = new List<System.Threading.Tasks.Task>();

        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetInteractable(false);
            tasks.Add(variant.selectionTask);
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);

        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetAsCorrect(0.5f, true);
            variants.Remove(variant);
        }
        AudioManager.Instance.CorrectVariantSound();
        ResultCheck();
    }

    private async void WrongVariant(List<AnswerVariantOLD> selectedVariants)
    {
        VibrationManager.Instance.TapNopeVibrate();
        LivesPanel.SetDamage(1);
        if (LivesPanel.Lives <= 0)
        {
            ChallengesManager.Instance.ShowResult(true);
        }

        var tasks = new List<System.Threading.Tasks.Task>();
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetInteractable(false);
            tasks.Add(variant.selectionTask);
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);
        AudioManager.Instance.WrongVariantSound();
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SetAsWrong(0.5f);
        }

        tasks = new List<System.Threading.Tasks.Task>();
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            tasks.Add(variant.tweenTask);
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);
        foreach (AnswerVariantOLD variant in selectedVariants)
        {
            variant.SelectToTask(false);
            variant.SetInteractable(true);
        }
    }

    private void ResultCheck()
    {
        if (variants.Count > 0)
        {
            ResetGoal();
        }
        else
        {
            ChallengesManager.Instance.ShowResult(true);
        }
    }

    protected override void SetVariantsValues()
    {
        List<int> variantsValues = new List<int>();

        foreach (AnswerVariantOLD variant in variants)
        {
            int randomInt = Random.Range(0, MaxNumber);
            variantsValues.Add(randomInt);
            variant.SetText(randomInt.ToString());
        }
    }

    private void SubscribeOnVariants()
    {
        foreach (AnswerVariantOLD variant in variants)
        {
            variant.GetComponent<Button>().onClick.AddListener(delegate { SelectVariant(variant); });
        }
    }

    private void SelectVariant(AnswerVariantOLD selectedVariant)
    {
        TaskElementOLD target = elements[selectedVariants.Count];
        selectedVariant.SelectToTask(target);

        if (selectedVariants.Count == ElementsAmount-1)
        {
            selectedVariants.Add(selectedVariant);

            if (Answer == (int)Evaluate(Expression(selectedVariants)))
            {
                CorrectVariant(selectedVariants);
            }
            else
            {
                WrongVariant(selectedVariants);
            }

            selectedVariants = new List<AnswerVariantOLD>();
        }
        else
        {
            selectedVariants.Add(selectedVariant);
        }
    }
}

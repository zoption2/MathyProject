using System.Collections;
using System.Collections.Generic;
using Fallencake.Tools;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using Mathy.Core;

public class PairsEquationOLD : ChallengeOLD
{
    #region Fields

    public override TaskType TaskType { get; } = TaskType.PairsEquation;
    private List<AnswerVariantOLD> selectedVariants = new List<AnswerVariantOLD>();

    #endregion

    public override void RunTask()
    {
        Initialization();
        UpdateDisplayStyle();
        ClearPanels();
        GenerateVariants();
        StartCoroutine(UpdateTextStyle());
        SubscribeOnVariants(); 
        SetLives();
    }

    protected override void UpdateDisplayStyle()
    {
        ShowTaskPanel(false);
        ShowTimer(true, LeftPanel);
        ShowLivesPanel(true, RightPanel);
        ShowBGImage(true, BGImage.sprite);
        ShowVariantsPanel(true, 12, new Vector2(10, 10));
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
            variant.SelectTween2(false);
            variant.SetInteractable(true);
        }
    }

    private void ResultCheck()
    {
        if (variants.Count == 0)
        {
            ChallengesManager.Instance.ShowResult(true);
        }
    }

    protected override void SetVariantsValues()
    {
        List<int> numberList = new List<int>();
        numberList = ListExtensions.UniqueRandomInts(MaxNumber, variants.Count / 2);
        /*for (int i = 0; i < variants.Count / 2; i++)
        {
            //int randomInt = numberList.UniqueRandom(0, stats.maxNumber);

            int randomInt = Random.Range(0, stats.maxNumber);
            numberList.Add(randomInt);
        }*/
        List<int> variantsValues = numberList.Concat(numberList).ToList();
        variantsValues.FCShuffle();

        for (int i = 0; i < variants.Count; i++)
        {
            int c = variantsValues[i];
            int a = Random.Range(-c, c);
            int b = c - a;

            while (b > MaxNumber || a == 0 || b == 0)
            {
                a = Random.Range(-c, c);
                b = c - a;
            }

            string expression;
            if(a > 0)
            {
                expression = a.ToString() + "+" + b.ToString();
            }
            else
            {
                expression = b.ToString() + a.ToString();
            }

            variants[i].SetText(expression);
            variants[i].SetValue(c);
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
        selectedVariant.SelectTween2(true);
        if (selectedVariants.Count == 1)
        {
            selectedVariants.Add(selectedVariant);

            if (selectedVariants[0].GetValueInt() == selectedVariants[1].GetValueInt())
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

    protected override string Expression()
    {
        string expression = "";
        for (int i = 0; i < elements.Count; i++)
        {
            expression += elements[i].value;
            if (i < operators.Count - 1) expression += operators[i].value;
        }
        return expression;
    }
}
using System.Collections.Generic;
using Fallencake.Tools;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Collections;
using System;

public abstract class TaskOLD : MonoBehaviour
{
    #region Fields

    [Header("Config:")]

    [SerializeField] protected bool onlyPositive = true;
    [SerializeField] protected float variantTweenDuration = 1.5f;

    [Header("GUI Panels:")]

    [SerializeField] protected Transform TaskPanel;
    [SerializeField] protected Transform VariantsPanel;

    [Header("Prefabs:")]

    [SerializeField] protected GameObject elementPrefab;
    [SerializeField] protected GameObject operatorPrefab;
    [SerializeField] protected GameObject variantPrefab;

    //Temp public coz we cant just set varuiant value right now
    public List<AnswerVariantOLD> variants; //was protected

    protected List<TaskElementOLD> elements;
    protected List<MathOperatorOLD> operators;
    protected AnswerVariantOLD correctVariant;
    public Stats stats { get; set; }
    public virtual TaskType TaskType { get; } = TaskType.Addition;   
    public int Answer { get; protected set; }
    public virtual string answerText { get; protected set; }
    public int correctVariantIndex { get; protected set; }
    public int selectedVariantIndex { get; protected set; }
    public List<string> _elements { get; protected set; } //Why its public
    public List<string> _variants { get; protected set; } //Why its public

    #endregion

    public virtual void RunTask()
    {
        GenerateElements();
        GenerateVariants();
    }

    public virtual void ResetToDefault()
    {
        ClearPanels();
    }

    #region Elements

    protected virtual void GenerateElements()
    {
        ClearPanels();
        InstantiateElements();
        SetElementsValues();
        SetOperatorsValues();
        SetAnswer();
    }

    protected virtual void ClearPanels()
    {
        elements = new List<TaskElementOLD>();
        operators = new List<MathOperatorOLD>();
        variants = new List<AnswerVariantOLD>();

        TaskPanel.DestroyChildren();
        VariantsPanel.DestroyChildren();
    }

    protected virtual void InstantiateElements()
    {
        for (int i = 0; i < stats.ElementsAmount; i++)
        {
            var elementInstance = Instantiate(elementPrefab, TaskPanel);
            elements.Add(elementInstance.GetComponent<MathElementOLD>());

            var operatorInstance = Instantiate(operatorPrefab, TaskPanel);
            operators.Add(operatorInstance.GetComponent<MathOperatorOLD>());
        }
    }

    protected virtual void SetElementsValues()
    {
        foreach (MathElementOLD element in elements)
        {
            element.SetRandom(1, stats.MaxNumber);
        }
    }

    protected virtual void SetOperatorsValues()
    {
        foreach (MathOperatorOLD mathOperator in operators)
        {
            mathOperator.SetRandom();
            if (mathOperator == operators.Last()) mathOperator.SetEquals();
        }
    }

    protected virtual void SetAnswer()
    {
        var answerInstance = Instantiate(elementPrefab, TaskPanel);
        answerInstance.GetComponent<MathElementOLD>().SetAsGoal();
    }

    public virtual void CalculateExpression()
    {
        string expression = Expression();
        Answer = (int)Evaluate(expression);

        if (Answer > stats.MaxNumber || (onlyPositive && Answer < 0))
        {
            GenerateElements();
            CalculateExpression();
        }
    }

    #endregion

    #region Variants

    protected virtual void GenerateVariants()
    {
        InstantiateVariants();
        SetVariantsValues();
    }

    protected virtual void InstantiateVariants()
    {
        for (int i = 0; i < stats.VariantsAmount; i++)
        {
            var variantInstance = Instantiate(variantPrefab, VariantsPanel);
            var variant = variantInstance.GetComponent<AnswerVariantOLD>();
            variant.SetIndex(i);
            variants.Add(variant);
        }
    }

    protected virtual void SetVariantsValues()
    {
        List<int> variantsValues = new List<int>() { Answer };
        int variantIndex = UnityEngine.Random.Range(0, variants.Count);
        var randomVariant = variants[variantIndex];
        randomVariant.SetText(Answer.ToString());
        correctVariant = randomVariant;
        correctVariantIndex = variantIndex;
        correctVariant.gameObject.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(CorrectVariant(correctVariant.index)); });

        foreach (AnswerVariantOLD variant in variants)
        {
            if (variant != correctVariant)
            {
                int randomInt = variantsValues.UniqueRandom(0, stats.MaxNumber);
                variantsValues.Add(randomInt);
                variant.SetText(randomInt.ToString());
                variant.gameObject.GetComponent<Button>().onClick.AddListener(delegate { StartCoroutine(WrongVariant(variant.index)); });
            }
        }
    }

    protected virtual IEnumerator CorrectVariant(int variantIndex)
    {
        Debug.LogError("Correct!!");
        AudioManager.Instance.CorrectVariantSound();
        selectedVariantIndex = variantIndex;
        var selectedVariant = variants[variantIndex];
        selectedVariant.SetAsCorrect(variantTweenDuration, false);

        foreach(AnswerVariantOLD variant in variants)
        {
            variant.SetInteractable(false);
        }

        yield return new WaitWhile(() => selectedVariant.isPressed == true);
        SaveTaskResult();
        ChallengesManager.Instance.CorrectAnswer();
    }

    protected virtual IEnumerator WrongVariant(int variantIndex)
    {
        Debug.LogError("Wrong!!");
        AudioManager.Instance.WrongVariantSound();
        selectedVariantIndex = variantIndex;
        var selectedVariant = variants[variantIndex];
        selectedVariant.SetAsWrong(variantTweenDuration);

        foreach (AnswerVariantOLD variant in variants)
        {
            variant.SetInteractable(false);
        }

        yield return new WaitWhile(() => selectedVariant.isPressed == true);
        SaveTaskResult();
        ChallengesManager.Instance.WrongAnswer();
    }

    #endregion
    
    protected virtual void SaveTaskResult()
    {
        if (!ChallengesManager.Instance.IsPractice)
        {
            _elements = GetAllElements();
            _variants = variants.Select(v => v.value).ToList();
            //ChallengesManager.Instance.SaveTaskData();
        }
    }

    protected virtual List<string> GetAllElements()
    {
        return elements.Select(e => e.value).ToList();
    }

    protected virtual string Expression()
    {
        string expression = "";
        for (int i = 0; i < elements.Count; i++)
        {
            expression += elements[i].value;
            if (i < operators.Count - 1) expression += operators[i].value;
        }
        return expression;
    }

    //Result calculation
    public static double Evaluate(string expression)
    {
        var doc = new System.Xml.XPath.XPathDocument(new System.IO.StringReader("<r/>"));
        var nav = doc.CreateNavigator();
        var newString = expression;
        newString = (new System.Text.RegularExpressions.Regex(@"([\+\-\*])")).Replace(newString, " ${1} ");
        newString = newString.Replace("/", " div ").Replace("%", " mod ");
        return (double)nav.Evaluate("number(" + newString + ")");
    }

    protected virtual IEnumerator UpdateTextStyle()
    {
        yield return new WaitForSeconds(0.1f);
        float fontSize = variants.OrderByDescending(v => v.fontSize()).Last().fontSize();
        foreach (AnswerVariantOLD variant in variants)
        {
            variant.SetFontSize(fontSize);
        }
        
    }
}

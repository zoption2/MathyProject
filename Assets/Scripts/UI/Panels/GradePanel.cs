using BansheeGz.BGSpline.Components;
using System.Collections.Generic;
using BansheeGz.BGSpline.Curve;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Zenject;
using Mathy.Services;

public class GradePanel : PopupPanel
{
    [Inject] private IDataService _dataService;

    [SerializeField] TMP_Text gradeCenterText;
    [SerializeField] BGCurve progressPath;
    [SerializeField] BGCurve progressPathFill;
    [SerializeField] Transform cursorTransform;
    [SerializeField] List<GradeButton> gradeButtons;
    [SerializeField] List<float> cursorDistantValues = new List<float>
    { 0.009f, 0.034f, 0.063f, 0.092f, 0.126f, 0.206f, 0.252f, 0.285f, 0.357f, 0.45f,
    0.472f, 0.52f, 0.578f, 0.605f, 0.629f, 0.704f, 0.776f, 0.843f, 0.885f, 0.952f, 0.98f};

    private BGCcCursor pathCursor;
    private Material pathFillMat;
    private float lastCursorValue = 0f;

    public override void Initialization()
    {
        base.Initialization();
        pathCursor = progressPath.GetComponent<BGCcCursor>();
        LineRenderer renderer = progressPathFill.gameObject.GetComponent<LineRenderer>();
        pathFillMat = renderer.material;
    }

    protected async override void OnComplete(bool isOpened)
    {
        if (isOpened)
        {
            int rank = await _dataService.PlayerData.Progress.GetRankAsynk();
            if (gameObject.activeSelf) StartCoroutine(AnimateGrades(rank));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateGrades(int rank)
    {
        float elapsedTime = 0;
        float waitTime = 2f;
        float currentCursorValue = lastCursorValue;
        //int rankIndex = PlayerDataManager.Instance.PlayerRank - 1;
        int rankIndex = rank - 1;
        float targetCursorValue = rankIndex < 0 ? 0 : cursorDistantValues[rankIndex];

        while (elapsedTime < waitTime)
        {
            pathCursor.DistanceRatio = Mathf.Lerp(currentCursorValue, targetCursorValue, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        pathCursor.DistanceRatio = targetCursorValue;
        lastCursorValue = targetCursorValue;
        yield return null;

        for (int i = 0; i < gradeButtons.Count; i++)
        {
            if (i < rankIndex)
            {
                gradeButtons[i].SetStatus(GradeStatus.Achieved);
            }
            else if (i == rankIndex)
            {
                gradeButtons[i].SetStatus(GradeStatus.Current);
                gradeButtons[i].transform.DOPunchScale(new Vector3(0.02f, 0.03f, 0), 1f).SetEase(Ease.InOutQuad);
            }
            else
            {
                gradeButtons[i].SetStatus(GradeStatus.Pending);
            }

        }
        pathFillMat.SetFloat("_Filling", 1- targetCursorValue);
        cursorTransform.DOPunchScale(new Vector3(0.01f, 0.02f, 0), 1f).SetEase(Ease.InOutQuad);
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class LoadingTextIndicator : MonoBehaviour
{
    #region FIELDS

    [Header("COMPONENTS:")]
    [SerializeField] private TMP_Text loadingText;

    [Header("TWEEN:")]
    [SerializeField] protected float tweenDuration = 1f;

    private int tweenDelay
    {
        get
        {
            int delay = (int)(tweenDuration * 1000);
            return delay;
        }
    }

    #endregion

    private void OnEnable()
    {
        _ = LoadingTextTween();
    }

    private async UniTask LoadingTextTween()
    {
        loadingText.text = "Loading";
        await UniTask.Delay(tweenDelay);
        loadingText.text = "Loading .";
        await UniTask.Delay(tweenDelay);
        loadingText.text = "Loading ..";
        await UniTask.Delay(tweenDelay);
        loadingText.text = "Loading ...";
        await UniTask.Delay(tweenDelay);
        if (gameObject.activeInHierarchy)_ = LoadingTextTween();
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class TaskElementViewExtensionHighlighter : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TaskElementState _highlightedState = TaskElementState.Unknown;
        [SerializeField] private Color _color;
        [SerializeField] private float _blinkTime = 0.5f;

        private ITaskViewComponent _mainComponent;
        private Color _originColor;
        private bool _inited;

        private void Awake()
        {
            if ( _mainComponent == null )
            {
                if (TryGetComponent<ITaskViewComponent>(out _mainComponent))
                {
                    _inited = true;
                    _mainComponent.ON_STATE_CHANGE += DoWork;
                }
            }
            _originColor = _image.color;
        }

        private void OnDestroy()
        {
            if (_inited)
            {
                _mainComponent.ON_STATE_CHANGE -= DoWork;
            }
            DOTween.Kill(transform);
            _image.color = _originColor;
        }

        private void DoWork(TaskElementState state)
        {
            DOTween.Kill(transform);
            if (state == _highlightedState)
            {
                _image.DOColor(_color, _blinkTime).SetId(transform).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                _image.color = _originColor;
            }
        }
    }
}


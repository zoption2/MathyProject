using Mathy.UI;
using UnityEngine;

namespace Mathy.Core.Tasks
{
    public class GameplayScenePointer : StaticInstance<GameplayScenePointer>
    {
        [SerializeField] private Transform taskParent;

        [field: SerializeField] public Transform CounterParent { get; private set; }
        [field: SerializeField] public ResultWindow ResultsWindow { get; private set; }


        public Transform GetNewTaskParent()
        {
            var holderGO = new GameObject("TaskViewSubholder", typeof(RectTransform));
            var holderTransform = holderGO.transform;
            holderTransform.SetParent(taskParent, false);
            var rect = holderGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector3.zero;
            rect.anchorMax = Vector3.one;
            rect.anchoredPosition = Vector3.zero;
            rect.SetLeft(0);
            rect.SetRight(0);
            rect.SetTop(0);
            rect.SetBottom(0);
            holderTransform.SetAsFirstSibling();
            return holderTransform;
        }
    }
}
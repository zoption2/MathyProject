using UnityEngine;

namespace Mathy
{
    public class IngameDebugConsoleEnter : MonoBehaviour
    {
        [SerializeField] private GameObject _debugConsole;

        private void Start()
        {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            _debugConsole.SetActive(true);
#endif
        }
    }
}


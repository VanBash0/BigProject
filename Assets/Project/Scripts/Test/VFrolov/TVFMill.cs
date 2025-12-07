using UnityEngine;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    ///  олесо мельницы вообще не требует действий, при Completed просто включаем.
    /// </summary>
    public class TVFMill : MonoBehaviour
    {
        [SerializeField]
        QuestActionHandlerMono _actionHandler;

        TVFRotate _rotate;

        void Start()
        {
            _rotate = GetComponent<TVFRotate>();
        }

        private void OnEnable()
        {
            _actionHandler.StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            _actionHandler.StateChanged -= OnStateChanged;
        }

        // ” колеса мельницы кодга состо€ние Completed просто включаем вращение.
        // ¬сю логику по нему отслеживает сам квест и когда активированы нужные позиции мен€ет состо€ние.
        private void OnStateChanged()
        {
            _rotate.enabled = _actionHandler.CurrentState == QuestActionState.Completed;
        }
    }
}

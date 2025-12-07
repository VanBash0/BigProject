using UnityEngine;
using Zenject;

namespace BigProject.Test.VFrolov
{
    // см TVFInstallGear с пояснениями.
    public class TVFButton : MonoBehaviour
    {
        [SerializeField]
        private QuestActionHandlerMono _actionHandler;
        [SerializeField]
        private int _toCompletedId;
        [Inject]
        TVFInput _input;
        QuestActionState _state;
        private Vector3 _startPosition, _targetPosition;

        private void Start()
        {
            _state = _actionHandler.CurrentState;
            _startPosition = transform.position;
            _targetPosition = _startPosition;
            _targetPosition.x -= 0.1f;
        }

        private void OnEnable()
        {
            _actionHandler.StateChanged += OnStateChanged;
            _input.GOClicked += OnGOClicked;
        }

        private void OnDisable()
        {
            _actionHandler.StateChanged -= OnStateChanged;
            _input.GOClicked -= OnGOClicked;
        }

        private void OnStateChanged()
        {
            if (_actionHandler.CurrentState == QuestActionState.Completed)
                transform.position = _targetPosition;
            else
                transform.position = _startPosition;
        }

        private void OnGOClicked(GameObject go)
        {
            if (go == gameObject)
            {
                if (_actionHandler.CurrentState == QuestActionState.Active)
                    _actionHandler.MakeTransition(_toCompletedId);
                else
                    _actionHandler.MakeTransition(1);
            }
        }
    }
}
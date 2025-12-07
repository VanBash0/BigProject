using UnityEngine;
using Zenject;

namespace BigProject.Test.VFrolov
{
    // см TVFInstallGear с пояснениями.
    public class TVFSwitchLever : MonoBehaviour
    {
        [SerializeField]
        private QuestActionHandlerMono _actionHandler;

        [SerializeField]
        private int _toCompletedId, _toActivateId;

        [Inject]
        private TVFInput _input;
        QuestActionState _state;
        private float _startRotarion, _targetRotation;

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

        private void Start()
        {
            _state = _actionHandler.CurrentState;
            _startRotarion = transform.eulerAngles.x;
            _targetRotation = _startRotarion - 100f;
        }

        private void OnGOClicked(GameObject go)
        {
            if (go == gameObject)
                _actionHandler.MakeTransition(_state == QuestActionState.Active ? _toCompletedId : _toActivateId);      
        }

        private void OnStateChanged()
        {
            _state = _actionHandler.CurrentState;
            var rotation = transform.eulerAngles;

            if (_state == QuestActionState.Completed)
                rotation.x = _targetRotation;
            else
                rotation.x = _startRotarion;

            transform.eulerAngles = rotation;
        }
    }
}
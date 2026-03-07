using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Utilities;
using UnityEngine;

namespace BigProject.Systems.QuestSystem
{
    public class QuestInteractableHandler : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private int _questId;
        [SerializeField]
        private int _actionId;
        [SerializeField]
        private int _transitionId;

        private ProgressManager _progressManager;
        private IQuestActionHandler _actionHandler;

        private void Start()
        {
            SetTransition(_actionId, _transitionId);
        }

        public void Init(ProgressManager progressManager)
        {
            _progressManager = progressManager;
            ExceptionUtilities.ThrowIfNull(_progressManager, string.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Progress manager"));
        }

        public void SetTransition(int actionId, int transitionId)
        {
            _actionId = actionId;
            _transitionId = transitionId;
            
            if (!_progressManager.TryGetQuestActionHandler(_questId, _actionId, out _actionHandler))
            {
                Debug.LogWarning(string.Format(LogStr.WARNING_QUEST, $"{gameObject.name} unable to get quest {_questId} action handler {_actionId}"));
                Destroy(this);
            }
        }

        public void Interact()
        {
            _actionHandler.MakeTransition(_transitionId);
        }
    }
}
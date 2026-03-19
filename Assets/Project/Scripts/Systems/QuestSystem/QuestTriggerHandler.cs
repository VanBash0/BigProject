using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Utilities;
using UnityEngine;

namespace BigProject.Systems.QuestSystem
{
    public class QuestTriggerHandler : MonoBehaviour
    {
        [SerializeField]
        private int _questId;
        [SerializeField]
        private int _actionId;
        [SerializeField]
        private int _transitionId;
        [SerializeField]
        private string _forTag = "Player";
        [SerializeField]
        private bool _destroyAfterTrigger = true;

        private IQuestActionHandler _actionHandler;


        public void Init(ProgressManager progressManager)
        {
            ExceptionUtilities.ThrowIfNull(progressManager, string.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Progress manager"));

            if (!progressManager.TryGetQuestActionHandler(_questId, _actionId, out _actionHandler))
            {
                Debug.LogWarning(string.Format(LogStr.WARNING_QUEST, $"{gameObject.name} unable to get quest {_questId} action handler {_actionId}"));
                Destroy(this);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_forTag))
            {
                _actionHandler.MakeTransition(_transitionId);

                if (_destroyAfterTrigger)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
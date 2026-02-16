using System;
using UnityEngine;
using Zenject;
using BigProject.Managers;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// MonoBehaviour обертка активности квеста.
    /// </summary>
    public class QuestActionHandlerMono : MonoBehaviour, IQuestActionHandler
    {
        [SerializeField, Tooltip("Tracked quest ID.")]
        private int _questId;
        [SerializeField, Tooltip("Tracked activity ID.")]
        private int _actionId;

        // Делегируем логику обычному обработчику.
        private IQuestActionHandler _actionHandler;

        public event Action StateChanged;
        public string ActionName => _actionHandler.ActionName;
        public QuestActionState CurrentState => _actionHandler.CurrentState;
        public IQuest Quest => _actionHandler.Quest;

        private void OnDestroy()
        {
            if (_actionHandler != null)
            {
                _actionHandler.StateChanged -= SendToActions;
            }
        }

        public void MakeTransition(int transitionId) => _actionHandler?.MakeTransition(transitionId);  

        [Inject]
        public void Construct(ProgressManager progressManager)
        {
            if (!progressManager.TryGetQuestActionHandler(_questId, _actionId, out _actionHandler))
            {
                Debug.LogError($"{gameObject.name} failed to get quest activity.");
            }
            else
            {
                _actionHandler.StateChanged += SendToActions;
            }
        }

        private void SendToActions()
        {
            StateChanged?.Invoke();
        }
    }
}
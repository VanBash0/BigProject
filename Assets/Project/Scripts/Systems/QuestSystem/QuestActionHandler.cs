using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems.QuestSystem
{
    public class QuestActionHandler: IQuestActionHandler
    {
        public string ActionName { get; private set; }
        public QuestActionState CurrentState { get; private set; }
        public event Action StateChanged;
        public IQuest Quest { get; private set; }

        // For manual transitions in external code.
        private Dictionary<int, (QuestActionState, QuestActionState)> _manualTransitions;
        private int _id;

        /// <param name="state">Initial action state</param>
        /// <param name="manualTransitions">Manual transitions of this action</param>
        internal QuestActionHandler(IQuest quest, int actionId, string name, QuestActionState state, Dictionary<int, (QuestActionState, QuestActionState)> manualTransitions)
        {
            Quest = quest;
            _id = actionId;
            ActionName = name;
            CurrentState = state;
            _manualTransitions = manualTransitions;
        }

        public void MakeTransition(int transitionId) 
        {
            // Manual transition is possible only with the correct ID and from the state specified in the transition.
            if (_manualTransitions.TryGetValue(transitionId, out var transition) &&
                (CurrentState == transition.Item1 || transition.Item1 == QuestActionState.Undefined))
            {
                    if (!Quest.ManualTransition(_id, transition.Item2))
                    {
                        Debug.LogWarning($"Activity handler of [{ActionName}] unable to make transition.");
                    }
            }          
        }

        internal void RemoveTransition(int id) => _manualTransitions.Remove(id);

        /// <summary>
        /// Called by the handler owner when the activity state changes.
        /// </summary>
        internal void OnStateChanged(QuestActionState newState)
        {
            CurrentState = newState;
            StateChanged?.Invoke();
        }
    }
}
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

        // Ручные транзакции.
        private Dictionary<int, (QuestActionState, QuestActionState)> _manualTransitions;
        private int _id;

        /// <param name="quest">Квест, в рамках которого происходит активность</param>
        /// <param name="actionId">IDактивности</param>
        /// <param name="name">Имя активности</param>
        /// <param name="state">Начальное состояние</param>
        /// <param name="manualTransitions">Ручные транзакции данной активности</param>
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
            // Ручной переход возможен только по корректному ID и из указанного в транзакции состояния.
            if (_manualTransitions.TryGetValue(transitionId, out var transition) &&
                (CurrentState == transition.Item1 || transition.Item1 == QuestActionState.Undefined))
            {
                    if (!Quest.ManualTransition(_id, transition.Item2))
                        Debug.LogWarning($"Activity handler of [{ActionName}] unable to make transition.");
            }          
        }

        internal void RemoveTransition(int id) => _manualTransitions.Remove(id);

        /// <summary>
        /// Вызывается владельцем обработчика при смене состояния активности.
        /// </summary>
        internal void OnStateChanged(QuestActionState newState)
        {
            CurrentState = newState;
            StateChanged?.Invoke();
        }
    }
}
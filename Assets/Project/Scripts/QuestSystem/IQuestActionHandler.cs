using System;
using UnityEngine;

namespace BigProject
{
    /// <summary>
    /// ќбработчик активности: обертка над активностью дл€ автоматизации отслеживани€ ее состо€ни€.
    /// </summary>
    public interface IQuestActionHandler
    {
        string ActionName { get; }
        QuestActionState CurrentState { get; }

        /// <summary>
        /// —обытие вызываетс€ при изменении состо€ни€ активности.
        /// </summary>
        event Action StateChanged;

        /// <summary>
        /// »спользовать в случае необходимости ручного управлени€ квестом.
        /// </summary>
        IQuest Quest { get; }

        /// <summary>
        /// —овершает переход активности квеста в новое состо€ние согласно протоколу с указанным id.
        /// ѕри недопустимости перехода игнорирует его.
        /// </summary>
        /// <param name="transitionId">id перехода</param>
        void MakeTransition(int transitionId);
    }
}

using System;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// Action handler: a wrapper around an action to automate tracking of its state.
    /// </summary>
    public interface IQuestActionHandler
    {
        public string ActionName { get; }
        public QuestActionState CurrentState { get; }

        /// <summary>
        /// The event is fired when the activity's state changes.
        /// </summary>
        public event Action StateChanged;

        /// <summary>
        /// Use when manual control of the quest is required.
        /// </summary>
        public IQuest Quest { get; }

        /// <summary>
        /// Transitions the quest activity to a new state according to the protocol with the specified id.
        /// If the transition is invalid, ignores it.
        /// </summary>
        public void MakeTransition(int transitionId);
    }
}

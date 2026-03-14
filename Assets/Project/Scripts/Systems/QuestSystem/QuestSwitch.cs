using BigProject.Utilities;
using System;
using UnityEngine;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// Switch quest global state by condition.
    /// </summary>
    public class QuestSwitch : IDisposable
    {
        private IQuest _quest;
        private IQuest _trackableQuest;
        private int _initActionId;
        private QuestActionState _initActionState;

        public event Action<QuestSwitch> QuestSwitched;

        public QuestSwitch(IQuest quest, IQuest trackableQuest, int initActionId, QuestActionState initActionState)
        {
            _quest = quest;
            _trackableQuest = trackableQuest;
            _initActionId = initActionId;
            _initActionState = initActionState;
            ExceptionUtilities.ThrowIfNull(_quest, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "QuestSwitch", "Quest"));
            ExceptionUtilities.ThrowIfNull(_trackableQuest, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "QuestSwitch", "Trackable Quest"));
            _trackableQuest.StateChanged += OnTrackableQuestChanged;
        }

        public void Dispose()
        {
            _trackableQuest.StateChanged -= OnTrackableQuestChanged;
        }

        private void OnTrackableQuestChanged(IQuest trackableQuest)
        {
            if (trackableQuest.CurrentState == QuestState.Completed)
            {
                _quest.ManualTransition(_initActionId, _initActionState, true);
                QuestSwitched?.Invoke(this);
            }
        }
    }
}
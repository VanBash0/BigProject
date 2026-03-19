using BigProject.Managers;
using BigProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// Tracker of quest transition to global states (see QuestState).
    /// </summary>
    public class QuestsBoundariesTracker : IDisposable
    {
        private Dictionary<int, List<IQuestBoundariesController>> _questsControllers = new();
        private ProgressManager _progressManager;
        private List<int> _questsIds;

        public QuestsBoundariesTracker(ProgressManager progressManager, List<int> questsIds)
        {
            _progressManager = progressManager;
            _questsIds = questsIds;
            ExceptionUtilities.ThrowIfNull(_progressManager, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "QuestsBoundariesController", "ProgressManager"));
            ExceptionUtilities.ThrowIfNull(_questsIds, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "QuestsBoundariesController", "Quests Ids list"));

            foreach (int questId in _questsIds)
            {
                if (_progressManager.AddQuestStateListener(questId, OnQuestStateChanged))
                {
                    GameLogManager.Info(String.Format(LogStr.INFO_SYSTEM, "QuestsBoundariesController", $"subscribe to quest {questId}"));
                }
                else
                {
                    Debug.LogWarning(String.Format(LogStr.WARNING_SYSTEM, "QuestsBoundariesController", $"unable to subscribe to quest {questId}"));
                }
            }
        }

        public void AddQuestController(IQuestBoundariesController questController)
        {
            ExceptionUtilities.ThrowIfNull(questController, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "QuestsBoundariesController", "Quest Controller"));
            GameLogManager.Info(String.Format(LogStr.INFO_SYSTEM, "QuestsBoundariesController", $"add quest {questController.QuestId} controller"));

            if (!_questsControllers.ContainsKey(questController.QuestId))
            {
                _questsControllers.Add(questController.QuestId, new());
            }

            _questsControllers[questController.QuestId].Add(questController);
        }

        /// <summary>
        /// Don't forget to remove monobehaviour implementation before destroy it.
        /// </summary>
        public void RemoveQuestController(IQuestBoundariesController questController)
        {
            if (!_questsControllers.ContainsKey(questController.QuestId))
            {
                Debug.LogWarning(String.Format(LogStr.WARNING_SYSTEM, "QuestsBoundariesController", $"unable to remove {questController.QuestId} controller"));
                return;
            }

            _questsControllers[questController.QuestId].Remove(questController);

            if (_questsControllers[questController.QuestId].Count == 0)
            {
                _questsControllers.Remove(questController.QuestId);
            }
        }

        /// <summary>
        /// Remove all controllers of quest.
        /// </summary>
        public void RemoveQuestsController(int questId)
        {
            _questsControllers.Remove(questId);
        }

        /// <summary>
        /// Invoke when enter scene.
        /// </summary>
        public void OnSceneEntry()
        {
            foreach (KeyValuePair<int, List<IQuestBoundariesController>> questControllers in _questsControllers)
            {
                if (_progressManager.GetQuestState(questControllers.Key) == QuestState.Active)
                {
                    GameLogManager.Info(String.Format(LogStr.INFO_SYSTEM, "QuestsBoundariesController", $"{questControllers.Key} quest controllers init on scene entry"));
                    questControllers.Value.ForEach(x => x.InitOnSceneEntry());
                }
                else
                {
                    GameLogManager.Info(String.Format(LogStr.INFO_SYSTEM, "QuestsBoundariesController", $"{questControllers.Key} quest controllers deinit on scene entry"));
                    questControllers.Value.ForEach(x => x.DeinitOnSceneEntry());
                }
            }
        }

        private void OnQuestStateChanged(IQuest quest)
        {
            if (_questsControllers.TryGetValue(quest.ID, out List<IQuestBoundariesController> questsControllers))
            {
                GameLogManager.Info(String.Format(LogStr.INFO_SYSTEM, "QuestsBoundariesController", $"set quest {quest.ID} controller on state {quest.CurrentState}"));
                SetQuestControllers(quest.CurrentState, questsControllers);
            }
            else
            {
                Debug.Log(String.Format(LogStr.WARNING_SYSTEM, "QuestsBoundariesController", $"has no controllers of quest {quest.ID}"));
            }
        }

        private void SetQuestControllers(QuestState questState, List<IQuestBoundariesController> questControllers)
        {
            switch (questState)
            {
                case QuestState.Active:
                    questControllers.ForEach(x => x.Begin());
                    break;
                case QuestState.Inactive:
                case QuestState.Completed:
                case QuestState.Failed:
                    questControllers.ForEach(x => x.End());
                    break;
            }
        }

        public void Dispose()
        {
            foreach (int questId in _questsIds)
            {
                _progressManager.RemoveQuestStateListener(questId, OnQuestStateChanged);
                GameLogManager.Info(String.Format(LogStr.INFO_SYSTEM, "QuestsBoundariesController", $"unsubscribe to quest {questId}"));
            }
        }
    }
}
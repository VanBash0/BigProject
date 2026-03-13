using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.QuestSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BigProject.Gameplay.Village
{
    public class EntryPoint : MonoBehaviour
    {
        [Serializable]
        private class QuestEntry
        {
            public int questId;
            public UnityEvent init;
        }

        [SerializeField]
        private List<QuestEntry> _questsEntries;

        public void Init()
        {
            ProgressManager _progressManager = ServiceLocator.GetService<ProgressManager>();

            foreach (QuestEntry entry in _questsEntries)
            {
                if (_progressManager.GetQuestState(entry.questId) == QuestState.Active)
                {
                    entry.init?.Invoke();
                    GameLogManager.Info(String.Format(LogStr.INFO_QUEST, $"invoke init of quest {entry.questId}"));
                    return;
                }
            }

            GameLogManager.Info(String.Format(LogStr.WARNING_QUEST, $"has no active quest for init"));
        }
    }
}
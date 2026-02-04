using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems.HUD
{
    /// <summary>
    /// Конфигурация для журнала - какие тригеры вызывают запись.
    /// </summary>
    [CreateAssetMenu(fileName = "QuestJournalConfig", menuName = "Scriptable Objects/Configs/QuestJournalConfig")]
    public class QuestJournalConfig : ScriptableObject, IEnumerable<QuestJournalTriggers>
    {
        [field: SerializeField]
        public string LocalizationTableName { get; private set; }

        [SerializeField]
        private List<QuestJournalTriggers> _questsJournalTriggers;

        public QuestJournalTriggers GetQuestJournalTriggers(int questId)
        {
            return _questsJournalTriggers.Find(x => x.QuestId == questId);
        }

        public IEnumerator<QuestJournalTriggers> GetEnumerator()
        {
            foreach (QuestJournalTriggers questTriggers in _questsJournalTriggers)
            {
                yield return questTriggers;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
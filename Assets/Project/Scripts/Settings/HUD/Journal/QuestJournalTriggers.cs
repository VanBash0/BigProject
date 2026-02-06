using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems.HUD
{
    /// <summary>
    /// Набор триггеров квеста для фиксации в журнале.
    /// </summary>
    [Serializable]
    public class QuestJournalTriggers
    {
        [field: SerializeField]
        public int QuestId {  get; private set; }
        [field: SerializeField]
        public string NameTableEntryKey {get; private set; }
        [field: SerializeField]
        public List<JournalTrigger> Triggers { get; private set; }

        [Serializable]
        public class JournalTrigger
        {
            [field: SerializeField]
            public int ActionId { get; private set; }
            [field: SerializeField]
            public QuestActionState StateWhenWrite { get; private set; }
            [field: SerializeField]
            public string TableEntryKey { get; private set; }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using BigProject.Systems.QuestSystem;

namespace BigProject.Systems.HUD
{
    /// <summary>
    /// Set of quest triggers for recording in journal.
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
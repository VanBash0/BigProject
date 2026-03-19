using BigProject.Systems.QuestSystem;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Settings
{
    [CreateAssetMenu(fileName = "QuestSwitchConfig", menuName = "Scriptable Objects/Configs/QuestSwitchConfig")]
    public class QuestSwitchConfig : ScriptableObject, IEnumerable<QuestSwitchConfig.Condition>
    {
        [Serializable]
        public class Condition
        {
            [field: SerializeField]
            public int QuestId { get; private set; }
            [field: SerializeField]
            public int InitActionId { get; private set; }
            [field: SerializeField]
            public QuestActionState InitActionState { get; private set; }
            [field: SerializeField]
            public int TrackableQuestId { get; private set; }
            [field: SerializeField]
            public int TrackableQuestActionId {  get; private set; }
        }

        [SerializeField]
        private List<Condition> _questSwitchConditions;

        public IEnumerator<Condition> GetEnumerator()
        {
            foreach (Condition switchCondition in _questSwitchConditions)
            {
                yield return switchCondition;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
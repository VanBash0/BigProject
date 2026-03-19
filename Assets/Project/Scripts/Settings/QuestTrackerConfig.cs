using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Settings
{
    [CreateAssetMenu(fileName = "QuestTrackerConfig", menuName = "Scriptable Objects/Configs/QuestTrackerConfig")]
    public class QuestTrackerConfig : ScriptableObject
    {
        [SerializeField]
        private List<int> _trackedQuests;

        public IReadOnlyList<int> QuestsIds => _trackedQuests;
    }
}
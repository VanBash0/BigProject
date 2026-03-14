using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.QuestSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Gameplay.Village
{
    public class EntryPoint : MonoBehaviour
    {

        [SerializeField]
        private AudioClip _music;

        [SerializeField]
        private List<MonoBehaviour> _questsControllers;

        private QuestsBoundariesTracker _questsTracker;

        public void Init()
        {
            _questsControllers.RemoveAll(x => x is not IQuestBoundariesController);
            _questsTracker = ServiceLocator.GetService<QuestsBoundariesTracker>();
            ServiceLocator.GetService<MusicManager>().PlayMusic(_music, 0.1f, 0.1f);

            foreach (IQuestBoundariesController questController in _questsControllers)
            {
                _questsTracker.AddQuestController(questController);
            }

            _questsTracker.OnSceneEntry();
        }

        private void OnDestroy()
        {
            foreach (IQuestBoundariesController questController in _questsControllers)
            {
                _questsTracker.RemoveQuestController(questController);
            }
        }
    }
}
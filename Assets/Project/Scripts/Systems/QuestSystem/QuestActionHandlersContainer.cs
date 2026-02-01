using BigProject.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static UnityEngine.Analytics.IAnalytic;
using static UnityEngine.CullingGroup;

namespace BigProject.Systems
{
    /// <summary>
    /// Контейнер для QuestActionHandler, обращение по ключу через оператор [].
    /// </summary>
    public class QuestActionHandlersContainer : MonoBehaviour
    {
        [Serializable]
        private class ActionData
        {
            [Tooltip("Name is unique key.")]
            public string name = "action";
            public int actionId;
        }

        [SerializeField]
        private int questId;
        [SerializeField]
        private List<ActionData> _questActions;

        private Dictionary<string, IQuestActionHandler> _questActionsDict = new();

        private void OnValidate()
        {
            if (_questActions == null || _questActions.Count < 2)
            {
                return;
            }

            HashSet<string> _uniqActions = _questActions.Select(x => x.name).ToHashSet();

            if (_uniqActions.Count != _questActions.Count)
            {
                Debug.LogWarning("Actions must have unique names!");
            }
        }

        [Inject]
        public void Construct(ProgressManager progressManager)
        {
            if (_questActions == null)
            {
                return;
            }

            foreach (ActionData actionData in _questActions)
            {
                if (progressManager.TryGetQuestActionHandler(questId, actionData.actionId, out var actionHandler))
                {
                    if (!_questActionsDict.TryAdd(actionData.name, actionHandler))
                    {
                        Debug.LogWarning($"Action handlers container of {gameObject.name} try to add actions with the same key: [{actionData.name}].");
                    }
                }
                else
                {
                    Debug.LogError($"{gameObject.name} failed to get quest activity [{actionData.actionId}].");
                }
            }

            _questActions.Clear();
        }

        public IQuestActionHandler this[string key] => _questActionsDict.GetValueOrDefault(key);
    }
}
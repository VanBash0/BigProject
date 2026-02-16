using BigProject.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BigProject.Systems.QuestSystem
{
    public class QuestActionsCaller : MonoBehaviour
    {
        [SerializeField]
        private int _questId;
        [SerializeField]
        private List<CallCondition> _callsConditions;

        private Dictionary<int, CallCondition> _callsConditionsDict = new();

        [Serializable]
        private class CallCondition
        {
            public int id;
            public QuestActionState stateWhenCall;
            public bool checkOnAwake;
            public UnityEvent questActions;

            [HideInInspector]
            public IQuestActionHandler actionHandler;
            [HideInInspector]
            public Action handler;
        }


        private void Awake()
        {
            if (!ServiceLocator.TryGetService(out ProgressManager pm))
            {
                string msg = $"{gameObject.name} events caller unable to get progress manager.";
                Debug.LogError(msg);
                GameLogManager.Error(msg);
                Destroy(gameObject);
                return;
            }

            foreach (var callCondition in _callsConditions)
            {
                if (!pm.TryGetQuestActionHandler(_questId, callCondition.id, out callCondition.actionHandler))
                {
                    string msg = $"{gameObject.name} unable to get action {callCondition.id}. It will be ignored.";
                    Debug.LogWarning(msg);
                    GameLogManager.Warning(msg);
                    continue;
                }

                try
                {
                    _callsConditionsDict.Add(callCondition.id, callCondition);
                }
                catch (ArgumentException e)
                {
                    string dictMsg = $"{gameObject.name} unable to add action {callCondition.id} to dictionary of calls {callCondition.id}. {e.Message}";
                    Debug.LogWarning(dictMsg);
                    GameLogManager.Warning(dictMsg);
                }

                callCondition.handler = () => OnStateChanged(callCondition.id);

                if (callCondition.checkOnAwake)
                {
                    callCondition.handler();
                }
            }

            _callsConditions.Clear();
        }
        private void OnStateChanged(int id)
        {
            CallCondition callCondition = _callsConditionsDict[id];

            if (callCondition.actionHandler.CurrentState == callCondition.stateWhenCall)
            {
                callCondition.questActions?.Invoke();
            }
        }

        private void OnEnable()
        {
            foreach (var callCondition in _callsConditionsDict.Values)
            {
                callCondition.actionHandler.StateChanged += callCondition.handler;
            }
        }

        private void OnDisable()
        {
            foreach (var callCondition in _callsConditionsDict.Values)
            {
                callCondition.actionHandler.StateChanged -= callCondition.handler;
            }
        }
    }
}
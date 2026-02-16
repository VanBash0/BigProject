using System;
using UnityEngine;
using BigProject.Systems.DialogueSystem;
using BigProject.Systems.QuestSystem;
using System.Collections.Generic;
using UnityEngine.Assertions;
using BigProject.Managers;

namespace BigProject.NPC
{
    /// <summary>
    /// Потенциально поменяется, пока эта обертка нужна для работы с DialogueSystem.
    /// </summary>
    public class DialogueSelector : MonoBehaviour
    {
        [SerializeField]
        private DialogNPC _dialogue;
        [SerializeField]
        private int _questId;
        [SerializeField]
        private List<DialogueCondition> _conditions;

        [Serializable]
        private class DialogueCondition
        {
            public int id;
            public QuestActionState state;
            public DialogueLine dialogue;
            public bool hasTransition;
            public int phraseIdToTransit;
            public int transitionId;

            [HideInInspector]
            public IQuestActionHandler actionHandler;
        }
        
        private void Awake()
        {
            ServiceLocator.TryGetService(out ProgressManager pm);
            Assert.IsNotNull(_dialogue, $"{gameObject.name} hasn't dialogue for select.");
            Assert.IsNotNull(pm, $"{gameObject.name} unable to get progress manager.");

            List<DialogueCondition> conditionsToRemove = new();

            foreach (DialogueCondition condition in _conditions)
            {
                if (pm.TryGetQuestActionHandler(_questId, condition.id, out condition.actionHandler))
                {
                    continue;
                }

                string msg = $"{gameObject.name} unable to get action {condition.id}. It will be ignored.";
                Debug.LogWarning(msg);
                conditionsToRemove.Add(condition);
            }

            _conditions.RemoveAll(x => conditionsToRemove.Contains(x));
        }

        private void Start()
        {
            StateChanged();
        }

        private void OnEnable()
        {
            foreach (DialogueCondition condition in _conditions)
            {
                condition.actionHandler.StateChanged += StateChanged;
            }

            DialogueManager.OnDialoguePhrase += OnDialoguePhrase;
        }

        private void OnDisable()
        {
            foreach (DialogueCondition condition in _conditions)
            {
                condition.actionHandler.StateChanged -= StateChanged;
            }

            DialogueManager.OnDialoguePhrase -= OnDialoguePhrase;
        }

        private void StateChanged()
        {
            foreach (DialogueCondition condition in _conditions)
            {
                if (condition.actionHandler.CurrentState == condition.state)
                {
                    _dialogue.StartDialogLine = condition.dialogue;
                    return;
                }
            }

            _dialogue.StartDialogLine = null;
        }

        private void OnDialoguePhrase(int phraseId)
        {
            DialogueCondition condition = _conditions.Find(x => x.hasTransition && x.phraseIdToTransit == phraseId);
            condition?.actionHandler.MakeTransition(condition.transitionId);

        }
    }
}

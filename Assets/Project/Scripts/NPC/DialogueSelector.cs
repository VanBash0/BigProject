using BigProject.Intercatable;
using System;
using UnityEngine;
using BigProject.Systems.DialogueSystem;
using BigProject.Systems;
using System.Collections.Generic;
using UnityEngine.Assertions;
using BigProject.Managers;
using System.Linq;

namespace BigProject.NPC
{
    /// <summary>
    /// Потенциально поменяется, пока эта обертка нужна для работы с DialogueSystem.
    /// </summary>
    public class DialogueSelector : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private DialogNPC _dialogue;
        [SerializeField]
        private int _questId;
        [SerializeField]
        private List<DialogueCondition> _conditions;

        private DialogueModeSwitch _modeSwitch;

        [Serializable]
        private class DialogueCondition
        {
            public int id;
            public QuestActionState state;
            public DialogueLine dialogue;
            public bool hasTransition;
            public int transitionId;

            [HideInInspector]
            public IQuestActionHandler actionHandler;
        }
        
        private void Awake()
        {
            ServiceLocator.TryGetService(out ProgressManager pm);
            ServiceLocator.TryGetService(out _modeSwitch);
            Assert.IsNotNull(_dialogue, $"{gameObject.name} hasn't dialogue for select.");
            Assert.IsNotNull(pm, $"{gameObject.name} unable to get progress manager.");
            Assert.IsNotNull(_modeSwitch, $"{gameObject.name} unable to get dialogue mode switch.");

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

        public void Interact()
        {
            _dialogue.Interact();

            if (_modeSwitch.gameObject.activeSelf)
            {
                _modeSwitch.DialogueСompleted += OnDialogueCompleted;
            }
        }

        private void OnEnable()
        {
            foreach (DialogueCondition condition in _conditions)
            {
                condition.actionHandler.StateChanged += StateChanged;
            }
        }

        private void OnDisable()
        {
            foreach (DialogueCondition condition in _conditions)
            {
                condition.actionHandler.StateChanged -= StateChanged;
            }
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

        private void OnDialogueCompleted()
        {
            DialogueCondition condition = _conditions.First(x => x.dialogue == _dialogue.StartDialogLine);
            condition?.actionHandler.MakeTransition(condition.transitionId);
            _modeSwitch.DialogueСompleted -= OnDialogueCompleted;
        }
    }
}

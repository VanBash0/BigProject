using BigProject.Systems.DialogueSystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigProject.Managers
{
    public class DialogueManager
    {
        // Событие срабатывает во время фразы NPC, если указан Id
        public static event Action<int> OnDialoguePhrase;

        //[SerializeField]
        //private float _speakerImageAlpha = 0.8f;
        //[SerializeField]
        //private float _speakerImageTone = 0.5f;

        private DialogueLine _currentDialogueLine;
        private int _currentDialoguePhraseIndex = 0;

        private DialogueView _dialogueView;

        public DialogueManager(DialogueView dialogueView)
        {
            _dialogueView.Init(this);
            _dialogueView = dialogueView;
            _dialogueView.HideDialogueWindow();
        }

        public void StartDialogue(DialogueLine dialogueLine)
        {
            if (ServiceLocator.TryGetService(out GameplayManager gameplayManager))
            {
                gameplayManager.ChangeState(GameplayState.Dialogue);
            }

            if (dialogueLine == null)
            {
                Debug.LogWarning("Не проинициализировали диалог");
                return;
            }

            if (dialogueLine.DialogueNPCPhrases.Count == 0 && dialogueLine.DialogueAnswerOptions.Count == 0)
            {
                Debug.LogWarning("Не проинициализировали диалог");
                return;
            }

            _currentDialogueLine = dialogueLine;
            _dialogueView.ShowDialogueWindow();
            ShowNextStep();
        }

        public void ShowNextStep()
        {
            Debug.Log("ShowNextStep");
            if (!_currentDialogueLine)
            {
                // Нет продолжения диалога
                EndDialogue();
                return;
            }

            if (_currentDialogueLine.DialogueNPCPhrases.Count > _currentDialoguePhraseIndex)
            {
                //SetImageAlpha(_leftCharacterImage, _speakerImageTone, _speakerImageAlpha);
                //SetImageAlpha(_rightCharacterImage, 1f, 1f);
                // NPC ещё не договорил - показываем следующую фразу
                ShowNextPhrase();
            }
            else if (_currentDialogueLine.DialogueAnswerOptions.Count > 0)
            {
                //SetImageAlpha(_rightCharacterImage, _speakerImageTone, _speakerImageAlpha);
                //SetImageAlpha(_leftCharacterImage, 1f, 1f);
                // NPC договорил и игроку есть что сказать - отображаем варианты ответов
                _dialogueView.ShowAnswerOptions(_currentDialogueLine);
            }
            else
            {
                // Диалог окончен
                EndDialogue();
            }
        }

        private void ShowNextPhrase()
        {
            // Включаем отображение кнопки продолжить и текст NPC
            DialogueNPCPhrase dialogueNPCPhrase =
                _currentDialogueLine.DialogueNPCPhrases[_currentDialoguePhraseIndex];

            _dialogueView.ShowNPCPhrase(dialogueNPCPhrase);
            _currentDialoguePhraseIndex++;

            if (dialogueNPCPhrase.Id > 0)
            {
                // Есть идентификатор фразы - уведомляем о том, что сейчас была сказана эта фраза
                OnDialoguePhrase.Invoke(dialogueNPCPhrase.Id);
            }
        }

        public void SelectAnswerOption(int answerOptionIndex)
        {
            Debug.Log(answerOptionIndex);
            _currentDialogueLine =
                _currentDialogueLine.DialogueAnswerOptions[answerOptionIndex].DialogueLine;
            _currentDialoguePhraseIndex = 0;
            _dialogueView.HideAnswerOptions();
            ShowNextStep();
        }

        private void EndDialogue()
        {
            _dialogueView.HideAnswerOptions();
            _dialogueView.HideDialogueWindow();
            _currentDialogueLine = null;
            _currentDialoguePhraseIndex = 0;

            if (ServiceLocator.TryGetService(out GameplayManager gameplayManager))
            {
                gameplayManager.ChangeState(GameplayState.Play);
            }
        }

        //private void SetImageAlpha(Image image, float tone, float alpha)
        //{
        //    Color color = new(tone, tone, tone, alpha);
        //    image.color = color;
        //}
    }
}

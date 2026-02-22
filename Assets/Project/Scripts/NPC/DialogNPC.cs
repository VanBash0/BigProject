using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems.DialogueSystem;
using UnityEngine;

namespace BigProject.NPC
{
    // Для старта диалога с NPC
    public class DialogNPC : MonoBehaviour, IInteractable
    {
        // Диалоговая фраза, с которой начинается общение
        public DialogueLine StartDialogLine;

        private DialogueManager _dialogueManager;

        public void Interact()
        {
            StartDialog();
        }

        public void Init(DialogueManager dialogueManager)
        {
            _dialogueManager = dialogueManager;
        }

        private void StartDialog()
        {
            Debug.Log(StartDialogLine.DialogueNPCPhrases[0].Text);
            if (StartDialogLine)
            {
                // Переходим в режим диалога, только если есть, что сказать
                _dialogueManager.StartDialogue(StartDialogLine);
            }
        }
    }
}

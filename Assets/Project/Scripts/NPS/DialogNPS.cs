using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems.DialogueSystem;
using UnityEngine;

namespace BigProject.NPS 
{
    // Тестовый скрипт на диалоговых NPS - позже переделать или удалить и создать новый
    public class DialogNPS : MonoBehaviour, IInteractable
    {
        // Диалоговая фраза, с которой начинается общение
        public DialogueLine StartDialogLine;

        public void Interact()
        {
            StartDialog();
        }

        private void StartDialog()
        {
            if (StartDialogLine)
            {
                // Переходим в режим диалога, только если есть, что сказать
                DialogueManager.Instance.StartDialogue(StartDialogLine);
            }
        }
    }
}

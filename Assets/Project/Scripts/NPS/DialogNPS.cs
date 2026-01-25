using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems.DialogSystem;
using UnityEngine;

namespace BigProject.NPS 
{
    // Тестовый скрипт на диалоговых NPS - позже переделать или удалить и создать новый
    public class DialogNPS : MonoBehaviour, IInteractable
    {
        // Диалоговая фраза, с которой начинается общение
        public DialogNpsPhrase StartDialogPhrase;

        public void Interact()
        {
            StartDialog();
        }

        private void StartDialog()
        {
            if (StartDialogPhrase)
            {
                // Переходим в режим диалога, только если есть, что сказать
                DialogManager.Instance.StartDialog(StartDialogPhrase);
            }
        }
    }
}

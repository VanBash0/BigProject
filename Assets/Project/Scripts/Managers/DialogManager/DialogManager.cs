using BigProject.Systems.DialogSystem;
using UnityEngine;

namespace BigProject.Managers
{
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance { get; private set; }
        // Строковое поле для NPS
        [SerializeField]
        private TMPro.TextMeshProUGUI _dialogTextUI;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        public void StartDialog(DialogNpsPhrase dialogNPSPhrase)
        {
            // @todo когда появится нормальный интерфейс - поменять
            _dialogTextUI.enabled = true;
            _dialogTextUI.text = dialogNPSPhrase.Text;
        }
    }
}

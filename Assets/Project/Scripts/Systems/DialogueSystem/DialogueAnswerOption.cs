using UnityEngine;

namespace BigProject.Systems.DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueAnswerOption", menuName = "Scriptable Objects/DialogueAnswerOption")]
    public class DialogueAnswerOption : ScriptableObject
    {
        public string Text;
        public DialogueLine DialogueLine;
        public bool IsStoryOption = false;
    }
}

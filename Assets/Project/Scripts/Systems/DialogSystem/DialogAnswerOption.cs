using UnityEngine;

namespace BigProject.Systems.DialogSystem
{
    [CreateAssetMenu(fileName = "DialogAnswerOption", menuName = "Scriptable Objects/DialogAnswerOption")]
    public class DialogAnswerOption : ScriptableObject
    {
        public string answerText;
        public DialogNpsPhrase NextDialogPhrase;
    }
}

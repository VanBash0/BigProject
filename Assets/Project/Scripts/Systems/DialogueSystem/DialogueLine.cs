using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems.DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueLine", menuName = "Scriptable Objects/DialogueLine")]
    public class DialogueLine : ScriptableObject
    {
        public List<DialogueNPSPhrase> DialogueNPSPhrases = new List<DialogueNPSPhrase>();
        public List<DialogueAnswerOption> DialogueAnswerOptions = new List<DialogueAnswerOption>();
    }
}


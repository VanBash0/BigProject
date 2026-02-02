using UnityEngine;

namespace BigProject.Systems.DialogueSystem
{
    [CreateAssetMenu(fileName = "DialogueNPCPhrase", menuName = "Scriptable Objects/DialogueNPCPhrase")]
    public class DialogueNPCPhrase : ScriptableObject
    {
        public string Text;
    }
}

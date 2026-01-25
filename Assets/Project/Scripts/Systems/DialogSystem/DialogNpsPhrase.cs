using UnityEngine;


namespace BigProject.Systems.DialogSystem
{
    [CreateAssetMenu(fileName = "DialogNpsPhrase", menuName = "Scriptable Objects/DialogNpsPhrase")]
    public class DialogNpsPhrase : DialogPhrase
    {
        public string Text;
        public DialogPhrase NextDialogPhrase;
    }
}
   

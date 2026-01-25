using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems.DialogSystem
{
    [CreateAssetMenu(fileName = "DialogPlayerPhrase", menuName = "Scriptable Objects/DialogPlayerPhrase")]
    public class DialogPlayerPhrase : DialogPhrase
    {
        public List<DialogAnswerOption> DialogAnswerOptions;
    }
}

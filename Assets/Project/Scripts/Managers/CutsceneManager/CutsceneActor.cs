using UnityEngine;

namespace BigProject.Managers.CutsceneManager
{
    public class CutsceneActor : MonoBehaviour
    {
        [field: SerializeField]
        public string Name { get; private set; }
    }
}
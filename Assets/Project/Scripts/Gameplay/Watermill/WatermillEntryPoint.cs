using BigProject.Managers;
using UnityEngine;

namespace BigProject.Gameplay
{
    public class WatermillEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _music;

        public void Init()
        {
            if (ServiceLocator.TryGetService(out MusicManager musicManager))
            {
                musicManager.PlayMusic(_music, 0.1f, 0.1f);
            }
        }
    }
}
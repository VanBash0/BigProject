using UnityEngine;

namespace BigProject.Managers
{
    public class AudioTest : MonoBehaviour //Test scripts and sould be deleted later
    {
        [SerializeField] private SoundsManager _soundsManager;
        [SerializeField] private MusicManager _musicManager;
        [SerializeField] private AudioClip _musicClip1;
        [SerializeField] private AudioClip _musicClip2;
        [SerializeField] private AudioClip _musicClip3;
        [SerializeField] private AudioClip _soundClip1;
        [SerializeField] private AudioClip _soundClip2;
        [SerializeField] private Transform _testTransform;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 600));

            GUILayout.Label("=== MUSIC MANAGER ===", GUILayout.Height(30));

            if (GUILayout.Button("Play Music 1 (Instant)", GUILayout.Height(30)))
            {
                _musicManager.PlayMusic(_musicClip1, 0f, 0f);
            }

            if (GUILayout.Button("Play Music 2 (Instant)", GUILayout.Height(30)))
            {
                _musicManager.PlayMusic(_musicClip2, 0f, 0f);
            }

            if (GUILayout.Button("Fade to Music 1 (2s/1s)", GUILayout.Height(30)))
            {
                _musicManager.PlayMusic(_musicClip1, 2f, 1f);
            }

            if (GUILayout.Button("Fade to Music 2 (1s/3s)", GUILayout.Height(30)))
            {
                _musicManager.PlayMusic(_musicClip2, 1f, 3f);
            }

            if (GUILayout.Button("Stop Music (Instant)", GUILayout.Height(30)))
            {
                _musicManager.StopMusic(0f);
            }

            if (GUILayout.Button("Stop Music (Fade 2s)", GUILayout.Height(30)))
            {
                _musicManager.StopMusic(2f);
            }

            GUILayout.Space(20);
            GUILayout.Label("=== SOUNDS MANAGER ===", GUILayout.Height(30));

            if (GUILayout.Button("Play SFX Sound", GUILayout.Height(30)))
            {
                _soundsManager.PlaySound(_soundClip1, MixerType.SFX);
            }

            if (GUILayout.Button("Play with Random Pitch", GUILayout.Height(30)))
            {
                _soundsManager.PlaySound(_soundClip1, MixerType.SFX, 0.5f, 1.5f);
            }

            if (GUILayout.Button("Play at Transform", GUILayout.Height(30)))
            {
                if (_testTransform != null)
                {
                    _soundsManager.PlaySound(_soundClip2, MixerType.SFX, spawnPosition: _testTransform);
                }
            }

            if (GUILayout.Button("Play Quiet (vol=0.3)", GUILayout.Height(30)))
            {
                _soundsManager.PlaySound(_soundClip2, MixerType.SFX, volume: 0.3f);
            }

            if (GUILayout.Button("Play with Owner (Transform)", GUILayout.Height(30)))
            {
                if (_testTransform != null)
                {
                    _soundsManager.PlaySound(_soundClip2, MixerType.SFX, owner: _testTransform);
                }
            }

            GUILayout.EndArea();
        }
    }
}
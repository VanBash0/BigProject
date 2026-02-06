using UnityEngine;
using System.Collections;

namespace BigProject.Managers
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;

        private Coroutine _fadeCoroutine;

        public void Init(AudioSource audioSource)
        {
            _musicSource = audioSource;
        }

        /// <summary>
        /// Plays given music with fades
        /// <param name = "fadeOutDuration"> Duration of fading out the current music </param>
        /// <param name = "fadeInDuration"> Duration of fading int the given music </param>
        /// </summary>
        public void PlayMusic(AudioClip musicClip, float fadeOutDuration = 1f, float fadeInDuration = 1f)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (_musicSource.isPlaying)
            {
                _fadeCoroutine = StartCoroutine(FadeToNewMusic(musicClip, fadeOutDuration, fadeInDuration));
            }
            else if (!_musicSource.isPlaying)
            {
                _musicSource.clip = musicClip;
                _musicSource.volume = 0f;
                _musicSource.Play();
                _fadeCoroutine = StartCoroutine(FadeInMusic(fadeInDuration));
            }
        }

        public void StopMusic(float fadeDuration = 1f)
        {
            if (_musicSource.isPlaying)
            {
                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                }

                _fadeCoroutine = StartCoroutine(FadeOutMusic(fadeDuration));
            }
        }

        private IEnumerator FadeToNewMusic(AudioClip newClip, float fadeOutDuration, float fadeInDuration)
        {
            float startVolume = _musicSource.volume;

            for (float time = 0; time < fadeOutDuration; time += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutDuration);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.clip = newClip;
            _musicSource.volume = 0f;
            _musicSource.Play();

            for (float time = 0; time < fadeInDuration; time += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, 1f, time / fadeInDuration);
                yield return null;
            }

            _musicSource.volume = 1f;
        }

        private IEnumerator FadeInMusic(float fadeInDuration)
        {
            for (float time = 0; time < fadeInDuration; time += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, 1f, time / fadeInDuration);
                yield return null;
            }

            _musicSource.volume = 1f;
        }

        private IEnumerator FadeOutMusic(float fadeDuration)
        {
            float startVolume = _musicSource.volume;

            for (float time = 0; time < fadeDuration; time += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.volume = 1f;
        }
    }
}
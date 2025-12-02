using UnityEngine;
using System.Collections;

namespace BigProject.Managers
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicSource;

        private Coroutine _fadeCoroutine;

        /// <summary>
        /// Plays given music with fades
        /// </summary>
        public void PlayMusic(AudioClip musicClip, float fadeOutDuration = 1f, float fadeInDuration = 1f)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

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
                    StopCoroutine(_fadeCoroutine);

                _fadeCoroutine = StartCoroutine(FadeOutMusic(fadeDuration));
            }
        }

        private IEnumerator FadeToNewMusic(AudioClip newClip, float fadeOutDuration, float fadeInDuration)
        {
            float startVolume = _musicSource.volume;

            for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.clip = newClip;
            _musicSource.volume = 0f;
            _musicSource.Play();

            for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeInDuration);
                yield return null;
            }

            _musicSource.volume = 1f;
        }

        private IEnumerator FadeInMusic(float fadeInDuration)
        {
            for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeInDuration);
                yield return null;
            }

            _musicSource.volume = 1f;
        }

        private IEnumerator FadeOutMusic(float fadeDuration)
        {
            float startVolume = _musicSource.volume;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.volume = 1f;
        }
    }
}
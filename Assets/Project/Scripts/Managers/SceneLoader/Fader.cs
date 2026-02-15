using System;
using UnityEngine;

namespace BigProject.Managers
{
    [RequireComponent(typeof(Animator))]
    public class Fader : MonoBehaviour
    {
        private const string FADED_KEY = "Faded";

        private Action _fadedInCallback;
        private Action _fadedOutCallback;

        private Animator _animator;

        public bool isFading { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void FadeIn(Action fadedInCallback)
        {
            if (isFading)
                return;

            isFading = true;
            _fadedInCallback = fadedInCallback;
            _animator.SetBool(FADED_KEY, true);
        }

        public void FadeOut(Action fadedOutCallback)
        {
            if (isFading)
                return;

            isFading = true;
            _fadedOutCallback = fadedOutCallback;
            _animator.SetBool(FADED_KEY, false);
        }

        private void Handle_FadeInAnimationOver()
        {
            _fadedInCallback?.Invoke();
            _fadedInCallback = null;
            isFading = false;
        }

        private void Handle_FadeOutAnimationOver()
        {
            _fadedOutCallback?.Invoke();
            _fadedOutCallback = null;
            isFading = false;
        }
    }
}
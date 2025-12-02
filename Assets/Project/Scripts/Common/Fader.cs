using System;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private const string FaderPath = "Fader";
    private const string AnimatorBoolKey = "Faded";

    [SerializeField] private Animator _animator;
    
    private static Fader _instance;

    public static Fader Instance
    {
        get 
        { 
            if (_instance == null)
            {
                var faderPrefab = Resources.Load<Fader>(FaderPath);
                _instance = Instantiate(faderPrefab);

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public bool isFading {  get; private set; }

    private Action _fadedInCallback;
    private Action _fadedOutCallback;

    public void FadeIn(Action fadedInCallback)
    {
        if (isFading)
            return;

        isFading = true;
        _fadedInCallback = fadedInCallback;
        _animator.SetBool(AnimatorBoolKey, true);
    }

    public void FadeOut(Action fadedOutCallback)
    {
        if (isFading)
            return;

        isFading = true;
        _fadedOutCallback = fadedOutCallback;
        _animator.SetBool(AnimatorBoolKey, false);
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

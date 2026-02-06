using System;
using UnityEngine;

namespace BigProject.Systems
{
    [DefaultExecutionOrder(-1)]
    public class RunesSystem : MonoBehaviour
    {
        public static RunesSystem Instance; // public на запись лучше не делать - внешний код может написать RunesSystem.Instance = some_val...
        public Action<int> OnRuneAdded; // События обозначать как event, иначе внешний код может вызвать его, где захочет.

        private int _numberOfRunes;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddRune()
        {
            if (_numberOfRunes >= 3)
            {
                Debug.LogError("Руна не добавлена - достигнуто максимальное количество");
                return;
            }

            OnRuneAdded?.Invoke(_numberOfRunes);
            _numberOfRunes++;
        }

        public int GetNumberOfRunes()
        {
            return _numberOfRunes;
        }
    }
}
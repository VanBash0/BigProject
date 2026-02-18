using System;
using UnityEngine;

namespace BigProject.Utilities
{
    public static class Remover
    {
        public static void SafeDispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public static void SafeRelease(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            if (obj is MonoBehaviour monoBehaviour)
            {
                GameObject.Destroy(monoBehaviour.gameObject);
            }
        }
    }
}
using BigProject.Initializers;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.ForTest
{
    public class SavingLoadingTest : MonoBehaviour
    {
        private ProgressManager _progressManager;

        private void Start()
        {
            _progressManager = ServiceLocator.GetService<ProgressManager>();
        }

        private void Update()
        {
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                _progressManager.SaveProgress();
            }
            else if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                _progressManager.LoadProgress();
            }
        }
    }
}

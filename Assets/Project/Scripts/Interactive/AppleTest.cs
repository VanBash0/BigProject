using UnityEngine;

namespace BigProject.Interactive 
{
    public class AppleTest : MonoBehaviour, IInteractive
    {
        public void OnInteract()
        {
            Debug.Log("Что-то произошло с яблоком");
        }

        public bool RequiresProximity()
        {
            Debug.Log("Нужно приблизиться перед взаимодействием");
            return true;
        }
    }
}

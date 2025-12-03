using BigProject.Intercatable;
using UnityEngine;

namespace BigProject.Intercatable 
{
    public class AppleTest : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("Нажали на яблоко - какое-то действие");
        }
    }
}

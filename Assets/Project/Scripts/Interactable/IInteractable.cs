using UnityEngine;

namespace BigProject.Intercatable 
{
    public interface IInteractable
    {
        // Возвращает true, если для взаимодействия нужно подойти
        public bool NeedComeUp { get => true; }
        // Вызывается при взаимодействии с объектом
        public void Interact();
    }
}

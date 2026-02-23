using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class ElderBag : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private int _inventoryItemId;
        
        public void Interact()
        {
            InventorySystem inventory = ServiceLocator.GetService<InventorySystem>();
            if (inventory != null)
            {
                inventory.AddItemByItemID(_inventoryItemId);
                Debug.Log("Добавлена сумка");
                Destroy(this);
            }
        }
    }
}


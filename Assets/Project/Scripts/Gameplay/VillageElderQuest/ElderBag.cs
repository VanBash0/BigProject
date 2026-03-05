using BigProject.Intercatable;
using BigProject.Systems.Inventory;
using UnityEngine;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class ElderBag : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private int _inventoryItemId;

        private InventorySystem _inventory;

        public void Init(InventorySystem inventory)
        {
            _inventory = inventory;
        }
        
        public void Interact()
        {
            _inventory.AddItemByItemID(_inventoryItemId);
            Destroy(gameObject);
        }
    }
}


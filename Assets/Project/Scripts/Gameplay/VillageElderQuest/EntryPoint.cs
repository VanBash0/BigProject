using BigProject.Managers;
using BigProject.Systems.Inventory;
using UnityEngine;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private ElderBag _elderBag;
        public void Init()
        {
            InventorySystem inventory = ServiceLocator.GetService<InventorySystem>();
            _elderBag.Init(inventory);
        }
    }
}
using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.HUD;
using UnityEngine;

namespace BigProject.Gameplay.VillageElderQuest
{
    /// <summary>
    /// Доп. зависимости (по хорошему надо будет убирать, т. к. это обход нехватки функционала в ряде модулей).
    /// </summary>
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
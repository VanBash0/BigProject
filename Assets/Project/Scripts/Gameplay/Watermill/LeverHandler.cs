using BigProject.Systems;
using UnityEngine;

namespace BigProject.Gameplay.Watermill
{
    public class LeverHandler : MonoBehaviour, IUsesItem
    {
        [SerializeField]
        private string _itemName;
        [SerializeField]
        private ControlPanel _controlPanel;

        public bool DoesUseItem(Item item) => item._name == _itemName;

        public void UseItem(Item item)
        {
            _controlPanel?.ApplyItem(item);
        }
    }
}
using UnityEngine;

namespace BigProject.Systems.Inventory
{
    public interface IUsesItem
    {
        public bool DoesUseItem(Item item)
        {
            Debug.Log("DoesUseItem() is not overridden!");
            return false;
        }

        public void UseItem(Item item)
        {
            Debug.Log("UseItem() is not overridden!");   
        }
    }
}
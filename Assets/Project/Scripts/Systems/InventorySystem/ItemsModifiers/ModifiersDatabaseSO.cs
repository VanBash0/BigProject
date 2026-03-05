using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigProject.Systems.Inventory.ItemsModifiers
{
    [CreateAssetMenu(fileName = "ModifiersDatabaseSO", menuName = "Inventory/ModifiersDatabase")]
    public class ModifiersDatabaseSO : ScriptableObject
    {
        [SerializeField]
        private List<ItemModifier> _modifiers;

        public bool TryGetModifier(string name, out ItemModifier itemModifier)
        {
            itemModifier = _modifiers.FirstOrDefault(x => x.ModifierName.Equals(name));
            return itemModifier != null;
        }
    }
}
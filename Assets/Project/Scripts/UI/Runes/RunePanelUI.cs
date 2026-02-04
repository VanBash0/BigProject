using BigProject.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.UI
{
    public class RunePanelUI : MonoBehaviour
    {
        [SerializeField] private List<RuneSlotUI> _runeSlots;

        private void OnEnable()
        {
            RunesSystem.Instance.OnRuneAdded += AddRune;
        }

        private void OnDisable()
        {
            RunesSystem.Instance.OnRuneAdded -= AddRune;
        }

        private void AddRune(int runeId)
        {
            _runeSlots[runeId].ShowRune();
        }
    }
}
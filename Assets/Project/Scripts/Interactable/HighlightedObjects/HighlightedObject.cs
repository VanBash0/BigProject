using UnityEngine;
using System.Collections.Generic;

namespace BigProject.Intercatable.HighlightedObjects
{
    //Требуется на любом подсвечиваемом предмете. Имеет список эффектов, которые накладывает при наведении на себя
    public class HighlightedObject : MonoBehaviour
    {
        [SerializeField] private List<HighlightEffect> _highlightEffects;

        public void Highlight()
        {
            foreach (HighlightEffect effect in _highlightEffects)
            {
                effect.EnableEffect();
            }
        }

        public void Unhighlight()
        {
            foreach (HighlightEffect effect in _highlightEffects)
            {
                effect.DisableEffect();
            }
        }
    }
}
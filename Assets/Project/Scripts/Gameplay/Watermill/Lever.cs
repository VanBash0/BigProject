using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Gameplay.Watermill
{
    [Serializable]
    public class Lever
    {
        [field: SerializeField]
        public Transform Transform { get; private set; }
        [field: SerializeField]
        public int PointId { get; set; }

        // Lever can have several target points.
        [SerializeField]
        private List<int> _targetIds = new();

        public bool InTargetPoint() => _targetIds.Contains(PointId);
    }
}
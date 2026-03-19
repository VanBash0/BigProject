using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigProject.Gameplay.Common
{
    public class SpawnPointsHandler : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _points;

        public bool TryGetSpawnTransform(int pointId, out Transform spawnTransform)
        {
            spawnTransform = _points.ElementAtOrDefault(pointId);
            return spawnTransform != null;
        }
    }
}

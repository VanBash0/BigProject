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

        public bool TryGetSpawnPosition(int pointId, out Vector3 position)
        {
            Transform spawnTranform = _points.ElementAtOrDefault(pointId);
            bool isFound = spawnTranform != null;
            position = isFound ? spawnTranform.position : Vector3.zero;
            return isFound;
        }
    }
}

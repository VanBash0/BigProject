using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Gameplay.Watermill
{
    public class GearsHandler : MonoBehaviour
    {
        [SerializeField]
        private List<GearSettings> _gears;
        [SerializeField]
        private float _rotateSpeed;

        [Serializable]
        private class GearSettings
        {
            public Transform transform;
            public Vector3 rotateAxis;
        }

        private void Update()
        {
            foreach (GearSettings gear in _gears)
            {
                gear.transform.Rotate(gear.rotateAxis, _rotateSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
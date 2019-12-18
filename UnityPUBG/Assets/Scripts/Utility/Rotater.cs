using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Utilities
{
    public class Rotater : MonoBehaviour
    {
        public Vector3 Speed;

        private void Update()
        {
            transform.Rotate(Speed);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Logic
{
    public class FollowCamera : MonoBehaviour
    {
        public Transform follow;
        public Vector3 followOffset = new Vector3(0, 20, 0);

        private void LateUpdate()
        {
            if (follow == null)
            {
                return;
            }

            transform.position = follow.position + followOffset;
        }
    }
}

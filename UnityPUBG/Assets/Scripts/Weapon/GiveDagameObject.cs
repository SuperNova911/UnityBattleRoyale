using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts
{
    public class GiveDagameObject : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "MyPlayer")
            {
                return;
            }
            else if (other.tag == "Enemy")
            {

            }
        }
    }
}
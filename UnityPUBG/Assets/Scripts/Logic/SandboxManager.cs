using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UnityPUBG.Scripts.Logic
{
    public class SandboxManager : MonoBehaviour
    {
        private void Awake()
        {
            PhotonNetwork.offlineMode = true;
            PhotonNetwork.CreateRoom("Sandbox");
        }

        private void Start()
        {

        }
    }
}

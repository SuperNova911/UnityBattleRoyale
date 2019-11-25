using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityPUBG.Scripts.Entities;

namespace UnityPUBG.Scripts.Logic
{
    public class SandboxManager : MonoBehaviour
    {
        public Player testPlayerPrefab;
        public Vector3 testPlayerSpawnPosition;

        private void Awake()
        {
            PhotonNetwork.offlineMode = true;
            PhotonNetwork.CreateRoom("Sandbox");
        }

        private void Start()
        {
            if (testPlayerPrefab != null)
            {
                PhotonNetwork.Instantiate(testPlayerPrefab.name, testPlayerSpawnPosition, Quaternion.identity, 0);
            }
        }
    }
}

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
        public GameObject fpsCounter;
        public GameObject dynamicCanvas;
        public GameObject fixedCanvas;
        public GameObject cameras;
        public GameObject minimapUI;
        public GameObject minimapCamera;
        public GameObject terrain;
        public GameObject postProcess;
        public GameObject dummy;
        public Player testPlayerPrefab;
        public Vector3 testPlayerSpawnPosition;

        [Header("Debug")]
        public GameObject debugButtonCanvas;
        public bool debugMode = false;

        private void Awake()
        {
            PhotonNetwork.offlineMode = true;
            PhotonNetwork.CreateRoom("Sandbox");

            if (debugMode)
            {
                debugButtonCanvas.SetActive(true);

                dynamicCanvas.SetActive(false);
                fixedCanvas.SetActive(false);
                terrain.SetActive(false);
                postProcess.SetActive(false);
                dummy.SetActive(false);
            }
            else
            {

            }
        }

        private void Start()
        {
            if (debugMode)
            {

            }
            else
            {
                SpawnItem();
                SpawnPlayer();
                StartRingSystem();
            }
        }

        public void ToggleFPS()
        {
            fpsCounter.SetActive(!fpsCounter.activeSelf);
        }

        public void SpawnItem()
        {
            ItemSpawnManager.Instance.SpawnRandomItemsAtSpawnPoints();
        }

        public void StartRingSystem()
        {
            RingSystem.Instance.GenerateRoundDatas();
            RingSystem.Instance.StartRingSystem();
        }

        public void DeployDropShip()
        {
            GameController.Instance.DeployDropShip();
        }

        public void ToggleDynamicCanvas()
        {
            dynamicCanvas.SetActive(!dynamicCanvas.activeSelf);
        }

        public void ToggleFixedCanvas()
        {
            fixedCanvas.SetActive(!fixedCanvas.activeSelf);
        }

        public void ToggleCamera()
        {
            cameras.SetActive(!cameras.activeSelf);
        }

        public void ToggleMinimap()
        {
            minimapUI.SetActive(!minimapUI.activeSelf);
            minimapCamera.SetActive(!minimapCamera.activeSelf);
        }

        public void ToggleTerrain()
        {
            terrain.SetActive(!terrain.activeSelf);
        }

        public void TogglePostProcess()
        {
            postProcess.SetActive(!postProcess.activeSelf);
        }

        public void ToggleDummy()
        {
            dummy.SetActive(!dummy.activeSelf);
        }

        public void SpawnPlayer()
        {
            PhotonNetwork.Instantiate(testPlayerPrefab.name, testPlayerSpawnPosition, Quaternion.identity, 0);
        }
    }
}

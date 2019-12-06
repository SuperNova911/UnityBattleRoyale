using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityPUBG.Scripts.UI;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class GameController : Singleton<GameController>
    {
        //public DropShip dropShipPrefab;
        public float dropShipLaunchDelay = 10f;

        [Header("Photon Networks")]
        public PhotonView photonView;

        private void Awake()
        {
            SetGraphicOptions();

            if (photonView == null)
            {
                photonView.GetComponent<PhotonView>();
            }
        }

        private void Start()
        {
            if (PhotonNetwork.isMasterClient)
            {
                ItemSpawnManager.Instance.SpawnRandomItemsAtSpawnPoints();
                ItemSpawnManager.Instance.DestroyItemSpawnGroups();

                StartCoroutine(StartRingSystem(20f));
                StartCoroutine(LaunchDropShipWithDelay(dropShipLaunchDelay));
            }
            else
            {
                ItemSpawnManager.Instance.DestroyItemSpawnGroups();
            }
        }

        public void DeployDropShip()
        {
            var dropShip = FindObjectOfType<DropShip>();
            dropShip.LaunchDropShip();
        }

        private void SetGraphicOptions()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        //10초 텀을 두고 드랍쉽 출발
        private IEnumerator LaunchDropShipWithDelay(float delay)
        {
            GameObject dropShip = PhotonNetwork.Instantiate("DropShip", Vector3.zero, Quaternion.identity, 0);
            yield return new WaitForSeconds(delay);

            dropShip.GetComponent<DropShip>().LaunchDropShip();
            yield break;
        }

        private IEnumerator StartRingSystem(float delay)
        {
            yield return new WaitForSeconds(delay);

            RingSystem.Instance.GenerateRoundDatas();
            RingSystem.Instance.StartRingSystem();
        }
    }
}

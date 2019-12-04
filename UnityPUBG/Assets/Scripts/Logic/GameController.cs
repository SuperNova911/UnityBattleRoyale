using System;
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
                //ItemSpawnManager.Instance.SpawnRandomItemsAtSpawnPoints();

                //RingSystem.Instance.GenerateRoundDatas();
                //RingSystem.Instance.StartRingSystem();

                //dropShipPrefab.LaunchDropShip();

                StartCoroutine(LaunchDropShipWithDelay());
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
        private System.Collections.IEnumerator LaunchDropShipWithDelay()
        {
            GameObject dropShip =  PhotonNetwork.Instantiate("DropShip", Vector3.zero, Quaternion.identity, 0);
            
            yield return new WaitForSeconds(10f);

            dropShip.GetComponent<DropShip>().LaunchDropShip();

            yield break;
        }
    }
}

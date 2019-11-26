using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityPUBG.Scripts.UI;

namespace UnityPUBG.Scripts.Logic
{
    public class GameController : Singleton<GameController>
    {
        [Header("Photon Networks")]
        public PhotonView photonView;

        private void Awake()
        {
            if (photonView == null)
            {
                photonView.GetComponent<PhotonView>();
            }
        }

        private void Start()
        {
            QualitySettings.vSyncCount = 0; // 수직동기화 OFF
            Application.targetFrameRate = 60; // 60프레임

            if (PhotonNetwork.isMasterClient)
            {
                ItemSpawnManager.Instance.SpawnRandomItemsAtSpawnPoints();

                RingSystem.Instance.GenerateRoundDatas();
                RingSystem.Instance.StartRingSystem();
            }
        }
    }
}

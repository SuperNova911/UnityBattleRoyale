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
            }
        }

        private void SetGraphicOptions()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
    }
}

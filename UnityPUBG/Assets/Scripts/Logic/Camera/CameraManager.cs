using Cinemachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class CameraManager : Singleton<CameraManager>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera minimapCamera;
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        [SerializeField] private CinemachineVirtualCamera dropShipCamera;

        private CinemachineVirtualCamera currentCamera;
        private ReadOnlyCollection<CinemachineVirtualCamera> virtualCameras;

        public Camera MainCamera => mainCamera;
        public CinemachineVirtualCamera PlayerCamera => playerCamera;
        public CinemachineVirtualCamera DropShipCamera => dropShipCamera;
        public CinemachineVirtualCamera CurrentCamera
        {
            get { return currentCamera; }
            set
            {
                currentCamera = value;

                foreach (var virtualCamera in virtualCameras)
                {
                    virtualCamera.enabled = false;
                }
                currentCamera.enabled = true;
            }
        }

        private void Awake()
        {
            if (mainCamera == null)
            {
                Debug.LogError($"{nameof(mainCamera)}가 할당되지 않았습니다");
            }
            if (playerCamera == null)
            {
                Debug.LogError($"{nameof(playerCamera)}가 할당되지 않았습니다");
            }
            if (dropShipCamera == null)
            {
                Debug.LogError($"{nameof(dropShipCamera)}가 할당되지 않았습니다");
            }

            virtualCameras = new ReadOnlyCollection<CinemachineVirtualCamera>(new List<CinemachineVirtualCamera>
            {
                playerCamera, dropShipCamera
            });

            CurrentCamera = PlayerCamera;
            EntityManager.Instance.OnMyPlayerSpawn += SetupPlayerCameras;
        }

        private void SetupPlayerCameras(object sender, EventArgs e)
        {
            var targetPlayer = EntityManager.Instance.MyPlayer;
            PlayerCamera.Follow = targetPlayer.transform;

            var followCamera = minimapCamera.GetComponent<FollowCamera>();
            followCamera.follow = targetPlayer.transform;

            CurrentCamera = PlayerCamera;
        }
    }
}

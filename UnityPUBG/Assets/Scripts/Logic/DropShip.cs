using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    [RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
    public class DropShip : MonoBehaviour
    {
        [Header("DropShip Settings")]
        public float altitude = 50f;
        public float speed = 15f;

        [Header("Path Settings")]
        public Vector2 mapCenter;
        public float pathRadius;
        public float allowDropRadius;
        public Vector2 startPosition;
        public Vector2 destinationPosition;

        [Header("Debug")]
        public bool showDebugGizmos;

        private float totalDistance;
        private Rigidbody dropShipRigidbody;
        private PhotonView photonView;

        public int OnBoardPlayerCount { get; private set; }
        public bool MyPlayerIsOnBoard { get; private set; }

        #region 유니티 메시지
        private void Awake()
        {
            totalDistance = (startPosition - destinationPosition).sqrMagnitude;
            dropShipRigidbody = GetComponent<Rigidbody>();
            dropShipRigidbody.useGravity = false;
            photonView = GetComponent<PhotonView>();

            transform.position = new Vector3(startPosition.x, altitude, startPosition.y);
            transform.LookAt(new Vector3(destinationPosition.x, altitude, destinationPosition.y));

            OnBoardPlayerCount = 0;
            MyPlayerIsOnBoard = false;

            CameraManager.Instance.DropShipCamera.Follow = transform;
            CameraManager.Instance.DropShipCamera.LookAt = transform;
        }

        private void FixedUpdate()
        {
            // 목적지에 도착했는지 검사
            float travelDistance = (startPosition - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude;
            bool arrivedAtDestination = totalDistance < travelDistance;

            if (arrivedAtDestination)
            {
                if (MyPlayerIsOnBoard)
                {
                    DropPlayer();
                }
                Destroy(gameObject);
            }

            Vector3 direction = transform.forward * speed * Time.fixedDeltaTime;
            dropShipRigidbody.MovePosition(transform.position + direction);
        }

        private void OnDrawGizmos()
        {
            if (showDebugGizmos)
            {
                DebugExtension.DrawCircle(new Vector3(mapCenter.x, altitude, mapCenter.y), Color.red, pathRadius);
                DebugExtension.DrawCircle(new Vector3(mapCenter.x, altitude, mapCenter.y), Color.green, allowDropRadius);
                Gizmos.DrawLine(new Vector3(startPosition.x, altitude, startPosition.y), new Vector3(destinationPosition.x, altitude, destinationPosition.y));
            }
        }
        #endregion

        public void GeneratePath()
        {

        }

        public void AddMyPlayer()
        {
            if (MyPlayerIsOnBoard)
            {
                Debug.LogWarning($"{nameof(Player)}가 이미 탑승중입니다");
                return;
            }
            MyPlayerIsOnBoard = true;

            var player = EntityManager.Instance.MyPlayer;
            if (player == null)
            {
                Debug.LogError($"");
                return;
            }

            player.transform.position = new Vector3(0, -10, 0);
            var playerRigidBody = player.GetComponent<Rigidbody>();
            if (playerRigidBody != null)
            {
                playerRigidBody.useGravity = false;
            }

            CameraManager.Instance.CurrentCamera = CameraManager.Instance.DropShipCamera;

            photonView.RPC(nameof(IncreaseOnBoardPlayerCount), PhotonTargets.All);
        }

        public void DropPlayer()
        {
            if (MyPlayerIsOnBoard == false)
            {
                Debug.LogWarning($"{nameof(Player)}가 탑승중이 아닙니다");
                return;
            }
            MyPlayerIsOnBoard = false;

            var player = EntityManager.Instance.MyPlayer;
            if (player == null)
            {
                Debug.LogError($"");
                return;
            }

            player.transform.position = transform.position;
            var playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.useGravity = true;
            }

            CameraManager.Instance.CurrentCamera = CameraManager.Instance.PlayerCamera;

            photonView.RPC(nameof(DecreaseOnBoardPlayerCount), PhotonTargets.All);
        }

        [PunRPC]
        private void IncreaseOnBoardPlayerCount()
        {
            OnBoardPlayerCount++;
        }

        [PunRPC]
        private void DecreaseOnBoardPlayerCount()
        {
            OnBoardPlayerCount--;
        }
    }
}

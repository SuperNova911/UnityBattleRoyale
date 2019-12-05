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
        [Range(10f, 100f)] public float altitude = 50f;
        [Range(1f, 50f)] public float speed = 15f;

        [Header("Path Settings")]
        public Vector2 mapCenter;
        [Range(50f, 200f)] public float centerRadius = 100f;
        [Range(51f, 500f)] public float allowDropRadius = 300f;
        [Range(5f, 10f)] public float preparationTime = 8f;
        [Range(5f, 10f)] public float finishingTime = 8f;
        [Space]
        [ReadOnly] public Vector2 launchPosition;
        [ReadOnly] public Vector2 destinationPosition;
        [ReadOnly] public Vector2 startDropPosition;
        [ReadOnly] public Vector2 endDropPosition;

        [Header("Debug")]
        public bool showDebugGizmos;

        private Rigidbody dropShipRigidbody;
        private PhotonView photonView;

        public event EventHandler OnStartDrop;
        public event EventHandler OnEndDrop;

        public enum DropShipStatus
        {
            WaitForLaunch, Preparation, Drop, Finishing
        }

        public DropShipStatus Status { get; private set; }
        public int OnBoardPlayerCount { get; private set; }
        public bool MyPlayerIsOnBoard { get; private set; }
        public float TimeBeforeDrop { get; private set; }

        #region 유니티 메시지
        private void Awake()
        {
            dropShipRigidbody = GetComponent<Rigidbody>();
            dropShipRigidbody.useGravity = false;
            photonView = GetComponent<PhotonView>();

            if (PhotonNetwork.isMasterClient)
            {
                GeneratePath();
                photonView.RPC(nameof(NotifyPathData), PhotonTargets.OthersBuffered, launchPosition, destinationPosition, startDropPosition, endDropPosition);
            }

            transform.position = new Vector3(launchPosition.x, altitude, launchPosition.y);
            transform.LookAt(new Vector3(destinationPosition.x, altitude, destinationPosition.y));

            Status = DropShipStatus.WaitForLaunch;
            OnBoardPlayerCount = 0;
            MyPlayerIsOnBoard = false;
            TimeBeforeDrop = preparationTime;

            CameraManager.Instance.DropShipCamera.Follow = transform;
            CameraManager.Instance.DropShipCamera.LookAt = transform;
        }

        private void FixedUpdate()
        {
            //출발 대기라면 아무것도 안함
            if (Status == DropShipStatus.WaitForLaunch)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                DropPlayer();
            }

            //마스터 클라이언트만 드랍쉽을 움직임
            if (PhotonNetwork.isMasterClient)
            {
                MoveDropShip();
            }
            CalculateStatus();
        }

        private void OnDestroy()
        {
            // Safety option
            if (MyPlayerIsOnBoard)
            {
                DropPlayer();
            }
        }

        private void OnDrawGizmos()
        {
            if (showDebugGizmos)
            {
                DebugExtension.DrawCircle(new Vector3(mapCenter.x, altitude, mapCenter.y), Color.cyan, centerRadius);
                DebugExtension.DrawCircle(new Vector3(mapCenter.x, altitude, mapCenter.y), Color.magenta, allowDropRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(startDropPosition.x, altitude, startDropPosition.y), new Vector3(endDropPosition.x, altitude, endDropPosition.y));
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(launchPosition.x, altitude, launchPosition.y), new Vector3(startDropPosition.x, altitude, startDropPosition.y));
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(endDropPosition.x, altitude, endDropPosition.y), new Vector3(destinationPosition.x, altitude, destinationPosition.y));
            }
        }

        private void OnValidate()
        {
            if (centerRadius >= allowDropRadius)
            {
                centerRadius = allowDropRadius - 1f;
            }
        }
        #endregion

        public void GeneratePath()
        {
            var randomPoint = UnityEngine.Random.insideUnitCircle;
            randomPoint = (randomPoint * centerRadius) + mapCenter;
            var randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

            var intersections = FindIntersections(mapCenter, allowDropRadius, randomPoint, randomDirection + randomPoint);
            startDropPosition = intersections[0];
            endDropPosition = intersections[1];
            launchPosition = startDropPosition + (randomDirection * speed * preparationTime);
            destinationPosition = endDropPosition + (-randomDirection * speed * finishingTime);
        }

        public void LaunchDropShip()
        {
            if (PhotonNetwork.isMasterClient == false)
            {
                Debug.LogWarning($"MasterClient가 직접 실행해야 합니다");
                return;
            }

            photonView.RPC(nameof(NotifyLaunchDropShip), PhotonTargets.All);
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

            //player.transform.position = new Vector3(0, -10, 0);
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
            if (Status != DropShipStatus.Drop)
            {
                return;
            }

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
            player.StartPlayOnGround();

            CameraManager.Instance.CurrentCamera = CameraManager.Instance.PlayerCamera;

            photonView.RPC(nameof(DecreaseOnBoardPlayerCount), PhotonTargets.All);
        }

        private void CalculateStatus()
        {
            float progress;
            switch (Status)
            {
                case DropShipStatus.Preparation:
                    var leftDistance = Vector2.Distance(startDropPosition, new Vector2(transform.position.x, transform.position.z));
                    TimeBeforeDrop = leftDistance / speed;

                    progress = Mathf.InverseLerp(0, (startDropPosition - launchPosition).sqrMagnitude, (new Vector2(transform.position.x, transform.position.z) - launchPosition).sqrMagnitude);
                    if (progress >= 1)
                    {
                        Status = DropShipStatus.Drop;
                        TimeBeforeDrop = 0f;

                        OnStartDrop?.Invoke(this, EventArgs.Empty);
                    }
                    break;

                case DropShipStatus.Drop:
                    progress = Mathf.InverseLerp(0, (endDropPosition - startDropPosition).sqrMagnitude, (new Vector2(transform.position.x, transform.position.z) - startDropPosition).sqrMagnitude);
                    if (progress >= 1)
                    {
                        if (MyPlayerIsOnBoard)
                        {
                            DropPlayer();
                        }
                        Status = DropShipStatus.Finishing;

                        OnEndDrop?.Invoke(this, EventArgs.Empty);
                    }
                    break;

                case DropShipStatus.Finishing:
                    progress = Mathf.InverseLerp(0, (destinationPosition - endDropPosition).sqrMagnitude, (new Vector2(transform.position.x, transform.position.z) - endDropPosition).sqrMagnitude);
                    if (progress >= 1)
                    {
                        Destroy(gameObject);
                    }
                    break;

                default:
                    Debug.LogError($"예상 범위를 벗어난 {nameof(DropShipStatus)}입니다, {nameof(Status)}: {Status}");
                    break;
            }
        }

        private void MoveDropShip()
        {
            Vector3 direction = transform.forward * speed * Time.fixedDeltaTime;
            dropShipRigidbody.MovePosition(transform.position + direction);
        }

        private Vector2[] FindIntersections(Vector2 circleCenter, float circleRadius, Vector2 pointA, Vector2 pointB)
        {
            Vector2 distance = new Vector2();
            Vector2[] intersectionPoints;
            float a, b, c;
            float bb4ac;
            float mu1;
            float mu2;

            distance.x = pointB.x - pointA.x;
            distance.y = pointB.y - pointA.y;

            a = distance.x * distance.x + distance.y * distance.y;
            b = 2 * (distance.x * (pointA.x - circleCenter.x) + distance.y * (pointA.y - circleCenter.y));
            c = circleCenter.x * circleCenter.x + circleCenter.y * circleCenter.y;
            c += pointA.x * pointA.x + pointA.y * pointA.y;
            c -= 2 * (circleCenter.x * pointA.x + circleCenter.y * pointA.y);
            c -= circleRadius * circleRadius;
            bb4ac = b * b - 4 * a * c;

            if (Mathf.Abs(a) < float.Epsilon || bb4ac < 0)
            {
                // line does not intersect
                // 이 경우로 오면 안됨
                Debug.LogWarning($"입력이 잘못 됨");
                return new Vector2[] { Vector2.zero, Vector2.zero };
            }

            mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2 * a);
            mu2 = (-b - Mathf.Sqrt(bb4ac)) / (2 * a);
            intersectionPoints = new Vector2[2];
            intersectionPoints[0] = new Vector2(pointA.x + mu1 * (pointB.x - pointA.x), pointA.y + mu1 * (pointB.y - pointA.y));
            intersectionPoints[1] = new Vector2(pointA.x + mu2 * (pointB.x - pointA.x), pointA.y + mu2 * (pointB.y - pointA.y));

            return intersectionPoints;
        }

        [PunRPC]
        private void NotifyPathData(Vector2 launchPosition, Vector2 destinationPosition, Vector2 startDropPosition, Vector2 endDropPosition)
        {
            this.launchPosition = launchPosition;
            this.destinationPosition = destinationPosition;
            this.startDropPosition = startDropPosition;
            this.endDropPosition = endDropPosition;
        }

        [PunRPC]
        private void NotifyLaunchDropShip()
        {
            AddMyPlayer();
            Status = DropShipStatus.Preparation;
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

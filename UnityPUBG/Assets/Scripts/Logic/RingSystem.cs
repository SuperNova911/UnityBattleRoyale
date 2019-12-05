using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class RingSystem : Singleton<RingSystem>
    {
        [Header("Settings")]
        public Vector2 initialRingCenter = new Vector2(250, 250);
        [Range(0f, 1000f)] public float initialRingRadius = 350;
        [Range(0f, 100f)] public float initialTickDamage = 1f;
        public Vector2 finalPositionCenter = new Vector2(250, 250);
        [Range(0f, 400f)] public float finalPositionRadius = 200;
        [Space]
        public RoundSetttings roundSettings = new RoundSetttings();

        [Header("Ring Mesh Settings")]
        public Material ringMaterial;
        [Range(10, 30)]
        public float ringHeight = 20f;
        [Range(3, 50)]
        public int resolutionX = 50;
        [Range(3, 50)]
        public int resolutionY = 3;

        [Header("Debug")]
        public bool showRingGizmos;

        private PhotonView photonView;
        private GameObject ringObject;
        private RingMeshGenerator ringMeshGenerator;

        public event EventHandler<RoundData> OnRoundStart;
        public event EventHandler<RoundData> OnRingCloseStart;

        public RoundData[] RoundDatas { get; private set; }
        public Vector2 CurrentRingCenter { get; private set; }
        public float CurrentRingTickDamage { get; private set; }
        public float CurrentRingRadius { get; private set; }

        #region 유니티 메시지
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            CreateRingObject();
        }

        private void OnDrawGizmos()
        {
            if (showRingGizmos && RoundDatas != null)
            {
                DebugExtension.DrawCircle(new Vector3(initialRingCenter.x, 0, initialRingCenter.y), Color.cyan, initialRingRadius);
                DebugExtension.DrawCircle(new Vector3(finalPositionCenter.x, 0, finalPositionCenter.y), Color.magenta, finalPositionRadius);
                if (RoundDatas != null)
                {
                    DrawDebugRingGizmos(RoundDatas);
                }
            }
        }

        private void OnValidate()
        {
            roundSettings.Validate();
        }
        #endregion

        /// <summary>
        /// RoundSettings을 기반으로 RoundData를 새로 생성
        /// </summary>
        public void GenerateRoundDatas()
        {
            roundSettings.Validate();
            RoundDatas = new RoundData[roundSettings.roundNumber];

            // 마지막 라운드
            RoundDatas[roundSettings.roundNumber - 1] = new RoundData(
                roundSettings.roundNumber,
                SelectFinalPosition(),
                roundSettings.diametersAfterClosing[roundSettings.roundNumber - 1],
                roundSettings.waitPeriods[roundSettings.roundNumber - 1],
                roundSettings.timeToCloses[roundSettings.roundNumber - 1],
                roundSettings.tickDamages[roundSettings.roundNumber - 1]);

            // 마지막에서 두 번째 라운드부터 거꾸로 계산
            for (int round = roundSettings.roundNumber - 2; round >= 0; round--)
            {
                Vector2 randomValue = UnityEngine.Random.insideUnitCircle;
                randomValue *= roundSettings.diametersAfterClosing[round] / 2f - roundSettings.diametersAfterClosing[round + 1] / 2f;

                RoundDatas[round] = new RoundData(
                    round + 1,
                    RoundDatas[round + 1].Center + randomValue,
                    roundSettings.diametersAfterClosing[round],
                    roundSettings.waitPeriods[round],
                    roundSettings.timeToCloses[round],
                    roundSettings.tickDamages[round]);
            }

            photonView.RPC(nameof(SyncRoundDatas), PhotonTargets.OthersBuffered,
                RoundDatas.Length,
                RoundDatas.Select(e => e.Center).ToArray(),
                RoundDatas.Select(e => e.DiameterAfterClosing).ToArray(),
                RoundDatas.Select(e => e.WaitPeriod).ToArray(),
                RoundDatas.Select(e => e.TimeToClose).ToArray(),
                RoundDatas.Select(e => e.TickDamage).ToArray());
        }

        public void StartRingSystem()
        {
            photonView.RPC(nameof(NotifyStartRingSystem), PhotonTargets.AllBuffered);
        }

        // TODO: Player가 이동할 수 있는 무작위 좌표를 선정
        private Vector2 SelectFinalPosition()
        {
            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle;
            return finalPositionCenter + (randomPosition * finalPositionRadius);
        }

        private void CreateRingObject()
        {
            ringObject = new GameObject("Ring Object");
            ringObject.transform.parent = transform;
            ringObject.transform.localPosition = Vector3.zero;

            var meshRenderer = ringObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = ringMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var meshFilter = ringObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();

            ringMeshGenerator = new RingMeshGenerator(meshFilter.sharedMesh, resolutionX, resolutionY, initialRingRadius, ringHeight);
        }

        private void UpdateRingObject()
        {
            // Update Mesh
            ringMeshGenerator.Radius = CurrentRingRadius;
            ringMeshGenerator.GenerateMesh();

            // Update Position
            ringObject.transform.position = new Vector3(CurrentRingCenter.x, 0, CurrentRingCenter.y);
        }

        private void DrawDebugRingGizmos(RoundData[] roundDatas)
        {
            for (int i = 0; i < roundDatas.Length; i++)
            {
                var colorVector = Vector3.Lerp(new Vector3(0, 1, 0), new Vector3(1, 0, 0), i / (float)(roundDatas.Length - 1));
                Color color = new Color(colorVector.x, colorVector.y, colorVector.z);
                Vector3 roundCenter = new Vector3(roundDatas[i].Center.x, 0, roundDatas[i].Center.y);

                DebugExtension.DebugCylinder(roundCenter + new Vector3(0, 1, 0), roundCenter - new Vector3(0, 1, 0), color, roundDatas[i].DiameterAfterClosing / 2f);
            }
        }

        private IEnumerator RingCountDown(RoundData[] roundDatas)
        {
            // Initial Settings
            CurrentRingCenter = initialRingCenter;
            CurrentRingRadius = initialRingRadius;
            CurrentRingTickDamage = initialTickDamage;
            UpdateRingObject();

            // Each Round
            for (int i = 0; i < roundDatas.Length; i++)
            {
                OnRoundStart?.Invoke(this, roundDatas[i]);

                // Ring Countdown
                float startTime = Time.time;
                float endTime = startTime + roundDatas[i].WaitPeriod;
                while (Time.time <= endTime)
                {
                    float progress = (Time.time - startTime) / roundDatas[i].WaitPeriod;

                    yield return new WaitForSeconds(1f);
                }

                // Ring Closing
                OnRingCloseStart?.Invoke(this, roundDatas[i]);
                CurrentRingTickDamage = roundDatas[i].TickDamage;

                Vector2 startCenter = CurrentRingCenter;
                float startRadius = CurrentRingRadius;
                startTime = Time.time;
                endTime = startTime + roundDatas[i].TimeToClose;
                while (Time.time <= endTime)
                {
                    float progress = (Time.time - startTime) / roundDatas[i].TimeToClose;
                    CurrentRingCenter = Vector2.Lerp(startCenter, roundDatas[i].Center, progress);
                    CurrentRingRadius = Mathf.Lerp(startRadius, roundDatas[i].DiameterAfterClosing / 2f, progress);

                    UpdateRingObject();

                    yield return null;
                    //yield return new WaitForSeconds(0.02f);
                }

                // After Closing
                // Do something
            }
        }

        private IEnumerator OnRingTick(float tickPeriod)
        {
            while (true)
            {
                foreach (var damageable in EntityManager.Instance.Damageables)
                {
                    var entity = (Entity)damageable;
                    if ((new Vector2(entity.transform.position.x, entity.transform.position.z) - CurrentRingCenter).sqrMagnitude > CurrentRingRadius * CurrentRingRadius)
                    {
                        damageable.OnTakeDamage(CurrentRingTickDamage, DamageType.IgnoreShield);
                    }
                }

                yield return new WaitForSeconds(tickPeriod);
            }
        }

        [PunRPC]
        private void SyncRoundDatas(int totalRound, Vector2[] centers, float[] diametersAfterClosing, float[] waitPeriods, float[] timeToCloses, float[] tickDamages)
        {
            RoundDatas = new RoundData[totalRound];
            for (int round = 0; round < totalRound; round++)
            {
                RoundDatas[round] = new RoundData(round + 1, centers[round], diametersAfterClosing[round], waitPeriods[round], timeToCloses[round], tickDamages[round]);
            }
        }

        [PunRPC]
        private void NotifyStartRingSystem()
        {
            if (RoundDatas == null)
            {
                Debug.LogWarning($"{nameof(RoundDatas)}를 먼저 생성해야 합니다");
                return;
            }

            StartCoroutine(RingCountDown(RoundDatas));

            float tickPeriod = 1f;
            StartCoroutine(OnRingTick(tickPeriod));
        }

        [Serializable]
        public class RoundSetttings
        {
            [Range(1, 6)]
            public int roundNumber = 5;
            [Range(1f, 120f)]
            public float[] waitPeriods = new float[] { 60f, 50f, 40f, 30f, 10f };
            [Range(1f, 60f)]
            public float[] timeToCloses = new float[] { 30f, 25f, 20f, 15f, 5f };
            [Range(0f, 500f)]
            public float[] diametersAfterClosing = new float[] { 400f, 150f, 50f, 10f, 0f };
            [Range(1f, 100f)]
            public float[] tickDamages = new float[] { 2.5f, 5f, 10f, 15f, 20f };

            public void Validate()
            {
                Array.Resize(ref waitPeriods, roundNumber);
                Array.Resize(ref timeToCloses, roundNumber);
                Array.Resize(ref diametersAfterClosing, roundNumber);
                Array.Resize(ref tickDamages, roundNumber);

                float previousWaitPeriod = waitPeriods[0];
                float previousTimeToClose = timeToCloses[0];
                float previousDiameterAfterClosing = diametersAfterClosing[0];
                float previousTickDamage = tickDamages[0];

                for (int i = 1; i < roundNumber; i++)
                {
                    if (waitPeriods[i] > previousWaitPeriod)
                    {
                        waitPeriods[i] = previousWaitPeriod;
                    }
                    if (timeToCloses[i] > previousTimeToClose)
                    {
                        timeToCloses[i] = previousTimeToClose;
                    }
                    if (diametersAfterClosing[i] > previousDiameterAfterClosing)
                    {
                        diametersAfterClosing[i] = previousDiameterAfterClosing;
                    }
                    if (tickDamages[i] < previousTickDamage)
                    {
                        tickDamages[i] = previousTickDamage;
                    }

                    previousWaitPeriod = waitPeriods[i];
                    previousTimeToClose = timeToCloses[i];
                    previousDiameterAfterClosing = diametersAfterClosing[i];
                    previousTickDamage = tickDamages[i];
                }
            }
        }

        public class RoundData
        {
            public RoundData(int roundNumber, Vector2 center, float diameterAfterClosing, float waitPeriod, float timeToClose, float tickDamage)
            {
                RoundNumber = roundNumber;
                Center = center;
                WaitPeriod = waitPeriod;
                TimeToClose = timeToClose;
                DiameterAfterClosing = diameterAfterClosing;
                TickDamage = tickDamage;
            }

            public int RoundNumber { get; }
            public Vector2 Center { get; }
            public float DiameterAfterClosing { get; }
            public float WaitPeriod { get; }
            public float TimeToClose { get; }
            public float TickDamage { get; }
        }

        public class RingMeshGenerator
        {
            public Mesh Mesh { get; }
            public float Height { get; set; }
            public int ResolutionX { get; set; }
            public int ResolutionY { get; set; }
            public float Radius { get; set; }

            public RingMeshGenerator(Mesh mesh, int resolutionX, int resolutionY, float radius, float height)
            {
                Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
                ResolutionX = resolutionX;
                ResolutionY = resolutionY;
                Radius = radius;
                Height = height;
            }

            public void GenerateMesh()
            {
                Vector3[] vertices = new Vector3[ResolutionX * ResolutionY];
                int[] triangles = new int[(ResolutionX - 1) * (ResolutionY - 1) * 6];
                Vector2[] uv = new Vector2[vertices.Length];

                int triangleIndex = 0;

                for (int y = 0; y < ResolutionY; y++)
                {
                    for (int x = 0; x < ResolutionX; x++)
                    {
                        int i = x + y * ResolutionX;
                        Vector2 percent = new Vector2(x / (float)(ResolutionX - 1), y / (float)(ResolutionY - 1));
                        Vector3 pointOnUnitCylinder = (Quaternion.Euler(0, Mathf.Lerp(0, 360, percent.x), 0) * Vector3.forward).normalized * Radius;
                        pointOnUnitCylinder.y = Mathf.Lerp(0, Height, percent.y);
                        vertices[i] = pointOnUnitCylinder;
                        uv[i] = pointOnUnitCylinder;

                        if (x != ResolutionX - 1 && y != ResolutionY - 1)
                        {
                            triangles[triangleIndex] = i;
                            triangles[triangleIndex + 1] = i + ResolutionX + 1;
                            triangles[triangleIndex + 2] = i + ResolutionX;

                            triangles[triangleIndex + 3] = i;
                            triangles[triangleIndex + 4] = i + 1;
                            triangles[triangleIndex + 5] = i + ResolutionX + 1;
                            triangleIndex += 6;
                        }
                    }
                }

                Mesh.Clear();
                Mesh.vertices = vertices;
                Mesh.triangles = triangles;
                Mesh.RecalculateNormals();
                Mesh.uv = uv;
            }
        }
    }
}

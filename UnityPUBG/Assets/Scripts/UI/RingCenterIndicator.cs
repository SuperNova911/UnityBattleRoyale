using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Logic;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.UI
{
    public class RingCenterIndicator : Singleton<RingCenterIndicator>
    {
        public SpriteRenderer spriteRenderer;
        public Transform playerTransform;
        public Vector2 ringCenterPosition;
        [Space]
        public float altitude = 14f;

        private RingSystem.RoundData currentRoundData;

        private void Awake()
        {
            RingSystem.Instance.OnRoundStart += OnRoundStart;
        }

        private void Update()
        {
            if (playerTransform == null || ringCenterPosition == null)
            {
                spriteRenderer.enabled = false;
                return;
            }

            float distance = Vector2.Distance(new Vector2(playerTransform.position.x, playerTransform.position.z), ringCenterPosition);
            if (distance <= currentRoundData.DiameterAfterClosing / 2f)
            {
                spriteRenderer.enabled = false;
                return;
            }
            else
            {
                spriteRenderer.enabled = true;
            }

            var middlePosition = (new Vector2(playerTransform.position.x, playerTransform.position.z) + ringCenterPosition) / 2f;
            transform.position = new Vector3(middlePosition.x, altitude, middlePosition.y);
            transform.LookAt(new Vector3(ringCenterPosition.x, altitude, ringCenterPosition.y));

            var size = spriteRenderer.size;
            size.x = distance / 4f;
            spriteRenderer.size = size;
        }

        private void OnRoundStart(object sender, RingSystem.RoundData roundData)
        {
            currentRoundData = roundData;
            ringCenterPosition = roundData.Center;

            var targetPlayer = EntityManager.Instance.MyPlayer;
            if (targetPlayer != null)
            {
                playerTransform = targetPlayer.transform;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Items
{
    public class LootAnimator : MonoBehaviour
    {
        #region 필드
        [SerializeField] private Transform looter;
        [SerializeField] private GameObject lootItem;
        [SerializeField] private LootAnimationSettings settings;

        private float phase1Progress = 0f;
        private float phase2Progress = 0f;
        private float phase3Progress = 0f;

        private Vector3 startPosition;
        private Vector3 startScale;
        private Vector3 randomFloatingOffset;

        private bool isReady = false;
        #endregion

        #region 유니티 메시지
        private void LateUpdate()
        {
            if (isReady)
            {
                PlayAnimation();
            }
        }
        #endregion

        #region 메서드
        public static LootAnimator InstantiateAnimation(Transform looter, GameObject lootItem, LootAnimationSettings settings)
        {
            if (looter == null)
            {
                Debug.LogError($"{nameof(looter)}의 값이 null일 수는 없습니다");
                return null;
            }

            if (lootItem == null)
            {
                Debug.LogError($"{nameof(lootItem)}의 값이 null일 수는 없습니다");
                return null;
            }

            if (settings == null)
            {
                Debug.LogError($"{nameof(settings)}의 값이 null일 수는 없습니다");
                return null;
            }

            GameObject emptyObject = new GameObject($"{nameof(LootAnimator)} {lootItem.name}");
            LootAnimator lootAnimator = emptyObject.AddComponent<LootAnimator>();

            var cloneLootItem = Instantiate(lootItem);
            cloneLootItem.transform.parent = emptyObject.transform;

            var colider = cloneLootItem.GetComponent<SphereCollider>();
            if (colider != null)
            {
                colider.enabled = false;
            }

            lootAnimator.looter = looter;
            lootAnimator.lootItem = cloneLootItem;
            lootAnimator.settings = settings;

            lootAnimator.phase1Progress = lootAnimator.phase2Progress = lootAnimator.phase3Progress = 0f;

            lootAnimator.startPosition = lootItem.transform.position;
            lootAnimator.startScale = lootItem.transform.localScale;
            lootAnimator.randomFloatingOffset = new Vector3(UnityEngine.Random.Range(-settings.RandomRange.x, settings.RandomRange.x), UnityEngine.Random.Range(-settings.RandomRange.y, settings.RandomRange.y), UnityEngine.Random.Range(-settings.RandomRange.z, -settings.RandomRange.z));

            lootAnimator.isReady = true;

            return lootAnimator;
        }

        private void PlayAnimation()
        {
            if (phase1Progress < 1f)
            {
                lootItem.transform.position = Vector3.Lerp(startPosition, looter.position + settings.FloatingOffset + randomFloatingOffset, phase1Progress);
                phase1Progress += settings.Phase1Speed * Time.deltaTime;
            }
            else if (phase2Progress < 1f)
            {
                lootItem.transform.position = looter.position + settings.FloatingOffset + randomFloatingOffset;
                phase2Progress += settings.Phase2Speed * Time.deltaTime;
            }
            else if (phase3Progress < 1f)
            {
                lootItem.transform.position = Vector3.Lerp(looter.position + settings.FloatingOffset + randomFloatingOffset, looter.position + settings.EndOffset, phase3Progress);
                lootItem.transform.localScale = Vector3.Lerp(startScale, settings.EndScale, phase3Progress);
                phase3Progress += settings.Phase3Speed * Time.deltaTime;
            }
            else
            {
                Destroy(lootItem.gameObject);
                Destroy(gameObject);
            }
        }
        #endregion
    }
}

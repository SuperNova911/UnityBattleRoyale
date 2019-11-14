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
    public class LootAnimation : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Transform looter;
        [SerializeField, ReadOnly] private GameObject lootItem;
        [SerializeField, ReadOnly] private LootAnimationSettings settings;

        private float phase1Progress = 0f;
        private float phase2Progress = 0f;
        private float phase3Progress = 0f;

        private Vector3 startPosition;
        private Vector3 startScale;
        private Vector3 randomFloatingOffset;

        #region 유니티 메시지
        private void LateUpdate()
        {
            PlayAnimation();
        }
        #endregion

        public static LootAnimation InstantiateAnimation(Transform looter, GameObject lootItemModel, Vector3 lootItemPosition, LootAnimationSettings settings)
        {
            if (looter == null)
            {
                Debug.LogError($"{nameof(looter)}의 값이 null일 수는 없습니다");
                return null;
            }

            if (lootItemModel == null)
            {
                Debug.LogError($"{nameof(lootItemModel)}의 값이 null일 수는 없습니다");
                return null;
            }

            if (settings == null)
            {
                Debug.LogError($"{nameof(settings)}의 값이 null일 수는 없습니다");
                return null;
            }

            GameObject emptyObject = new GameObject($"{nameof(LootAnimation)} {lootItemModel.name}");
            LootAnimation lootAnimation = emptyObject.AddComponent<LootAnimation>();

            var cloneLootItem = Instantiate(lootItemModel, lootItemPosition, Quaternion.identity, emptyObject.transform);
            var colider = cloneLootItem.GetComponent<SphereCollider>();
            if (colider != null)
            {
                colider.enabled = false;
            }

            lootAnimation.looter = looter;
            lootAnimation.lootItem = cloneLootItem;
            lootAnimation.settings = settings;

            lootAnimation.phase1Progress = lootAnimation.phase2Progress = lootAnimation.phase3Progress = 0f;

            lootAnimation.startPosition = lootItemPosition;
            lootAnimation.startScale = lootItemModel.transform.localScale;
            lootAnimation.randomFloatingOffset = new Vector3(UnityEngine.Random.Range(-settings.RandomRange.x, settings.RandomRange.x), UnityEngine.Random.Range(-settings.RandomRange.y, settings.RandomRange.y), UnityEngine.Random.Range(-settings.RandomRange.z, -settings.RandomRange.z));

            return lootAnimation;
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
                Destroy(gameObject);
            }
        }
    }
}

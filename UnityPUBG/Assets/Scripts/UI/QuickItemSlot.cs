using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class QuickItemSlot : MonoBehaviour
    {
        /// <summary>
        /// 퀵슬롯 인덱스
        /// 시블링 인덱스와 같다.
        /// </summary>
        private int quickSlotIndex;
        private Image iconImage;

        /// <summary>
        /// 빈 슬롯 이미지
        /// </summary>
        [SerializeField]
        private Sprite emptySlotImage;

        #region Unity 콜백
        private void Awake()
        {
            iconImage = transform.GetChild(0).GetComponent<Image>();
        }

        private void Start()
        {
            quickSlotIndex = transform.GetSiblingIndex();
        }
        #endregion

        /// <summary>
        /// 퀵슬롯 이미지, 인덱스 업데이트
        /// </summary>
        public void UpdateQuickItemSlot()
        {
            quickSlotIndex = transform.GetSiblingIndex();
            Item quickSlotItem = EntityManager.Instance.MyPlayer.ItemQuickBar[quickSlotIndex];
            //Sprite icon = Logic.EntityManager.Instance.MyPlayer.ItemQuickBar[quickslotIndex].Data.Icon;

            iconImage.sprite = (quickSlotItem.IsStackEmpty == false) ? quickSlotItem.Data.Icon : emptySlotImage;
        }
    }
}
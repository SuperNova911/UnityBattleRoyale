using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.UI
{
    public class QuickItemSlot : MonoBehaviour
    {
        /// <summary>
        /// 퀵슬롯 인덱스
        /// 시블링 인덱스와 같다.
        /// </summary>
        private int quickslotIndex;
        private UnityEngine.UI.Image iconImage;

        /// <summary>
        /// 빈 슬롯 이미지
        /// </summary>
        [SerializeField]
        private Sprite emptySlotImage;

        #region Unity 콜백

        private void Awake()
        {
            iconImage = transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        }

        private void Start()
        {
            quickslotIndex = transform.GetSiblingIndex();
        }

        #endregion

        /// <summary>
        /// 퀵슬롯 이미지, 인덱스 업데이트
        /// </summary>
        public void UpdateQuickItemSlot()
        {
            quickslotIndex = transform.GetSiblingIndex();
            Items.Item item = Logic.EntityManager.Instance.MyPlayer.ItemQuickBar[quickslotIndex];
            //Sprite icon = Logic.EntityManager.Instance.MyPlayer.ItemQuickBar[quickslotIndex].Data.Icon;

            if (item.Data.Icon != null && item.CurrentStack != 0)
                iconImage.sprite = item.Data.Icon;
            else
            {
                item = Items.Item.EmptyItem;
                iconImage.sprite = emptySlotImage;
            }
        }
    }
}
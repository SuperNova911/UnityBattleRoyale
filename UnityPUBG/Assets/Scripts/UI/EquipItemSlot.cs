using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class EquipItemSlot : MonoBehaviour
    {
        //아이템 종류 가방 or 실드
        [SerializeField] private string itemType;

        private Image itemIcon;

        #region 유니티 콜백
        private void Awake()
        {
            itemIcon = transform.GetChild(0).GetComponent<Image>();
        }

        private void OnEnable()
        {
            UpdateEquipItemSlot();
        }
        #endregion

        public void UpdateEquipItemSlot()
        {
            var myPlayer = EntityManager.Instance.MyPlayer;
            if (myPlayer == null)
            {
                return;
            }

            switch (itemType)
            {
                case "Armor":
                    if (!myPlayer.EquipedArmor.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedArmor.Data.Icon;
                    }
                    break;
                case "Backpack":
                    if (!myPlayer.EquipedBackpack.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedBackpack.Data.Icon;
                    }
                    break;
                case "Weapon":
                    if (!myPlayer.EquipedWeapon.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedWeapon.Data.Icon;
                    }
                    break;
                default:
                    Debug.LogError("장착 아이템이 아닙니다.");
                    break;
            }
        }
    }
}
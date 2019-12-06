using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class EquipItemSlot : MonoBehaviour
    {
        //아이템 종류 가방 or 실드
        public ItemType itemType;
        public Image itemImage = null;

        public enum ItemType { Backpack, Weapon, SecondaryWeapon, Armor }

        #region 유니티 콜백
        private void OnEnable()
        {
            UpdateEquipItemSlot();
        }
        #endregion

        public void UpdateEquipItemSlot()
        {
            Player myPlayer = EntityManager.Instance.MyPlayer;
            if (myPlayer == null)
            {
                return;
            }

            switch (itemType)
            {
                case ItemType.Armor:
                    if (!myPlayer.EquipedArmor.IsStackEmpty)
                    {
                        itemImage.sprite = myPlayer.EquipedArmor.Data.Icon;
                    }
                    break;
                case ItemType.Backpack:
                    if (!myPlayer.EquipedBackpack.IsStackEmpty)
                    {
                        itemImage.sprite = myPlayer.EquipedBackpack.Data.Icon;
                    }
                    break;
                case ItemType.Weapon:
                    if (!myPlayer.EquipedPrimaryWeapon.IsStackEmpty)
                    {
                        itemImage.sprite = myPlayer.EquipedPrimaryWeapon.Data.Icon;
                    }
                    break;
                case ItemType.SecondaryWeapon:
                    if(!myPlayer.EquipedSecondaryWeapon.IsStackEmpty)
                    {
                        itemImage.sprite = myPlayer.EquipedSecondaryWeapon.Data.Icon;
                    }
                    break;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class EquipItemSlot : MonoBehaviour
    {
        private enum ItemType { Backpack, Weapon, SecondaryWeapon, Armor }
        //아이템 종류 가방 or 실드
        [SerializeField] private ItemType itemType;

        private Image itemIcon = null;

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
            if (itemIcon == null)
            {
                Awake();
            }

            Entities.Player myPlayer = Logic.EntityManager.Instance.MyPlayer;

            switch (itemType)
            {
                case ItemType.Backpack:
                    if (!myPlayer.EquipedBackpack.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedBackpack.Data.Icon;
                    }
                    break;
                case ItemType.Weapon:
                    if (!myPlayer.EquipedWeapon.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedWeapon.Data.Icon;
                    }
                    break;
                case ItemType.SecondaryWeapon:
                    //TODO: 두번째 무기 아이콘 매핑
                    /*
                    if(!myPlayer.EquipedSecondaryWeapon.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedSecondaryWeapon.Data.Icon;
                    }
                    */
                    break;
                case ItemType.Armor:
                    if (!myPlayer.EquipedArmor.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedArmor.Data.Icon;
                    }
                    break;
            }
        }
    }
}
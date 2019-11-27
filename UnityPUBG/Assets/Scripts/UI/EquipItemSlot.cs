using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        private void Start()
        {
            UpdateEquipItemSlot();
        }

        private void OnEnable()
        {
            UpdateEquipItemSlot();
        }

        private void OnDisable()
        {
            UpdateEquipItemSlot();
        }
        #endregion

        public void UpdateEquipItemSlot()
        {
            Entities.Player myPlayer = Logic.EntityManager.Instance.MyPlayer;

            switch (itemType)
            {
                case "BackPack":
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
                case "Armor":
                    if (!myPlayer.EquipedArmor.IsStackEmpty)
                    {
                        itemIcon.sprite = myPlayer.EquipedArmor.Data.Icon;
                    }
                    break;
                default:
                    Debug.LogError("장착 아이템이 아닙니다.");
                    break;
            }
        }
    }
}
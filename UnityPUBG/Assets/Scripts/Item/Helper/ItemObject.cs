using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Items
{
    public class ItemObject : MonoBehaviour
    {
        #region 필드
        private Item item = null;
        #endregion

        #region 속성
        public Item Item => item;
        #endregion

        #region 메서드
        public void AssignItem(Item item)
        {
            this.item = item;

            DestroyAllChild();
            SpawnItemModel(item);
        }

        private void DestroyAllChild()
        {
            foreach (Transform child in transform)
            {
                Destroy(child);
            }
        }

        private void SpawnItemModel(Item item)
        {
            if (item.Data == null || item.Data.Model == null)
            {
                return;
            }

            Instantiate(item.Data.Model, transform);
        }
        #endregion
    }
}

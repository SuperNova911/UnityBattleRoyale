using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class ItemObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int id;
        private Item item = null;
        private GameObject modelObject = null;

        /// <summary>
        /// ItemObjectManager에 의해 관리되는 ItemObject는 0 이상의 Id를 가지고 있음
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public Item Item
        {
            get { return item; }
            set
            {
                item = value;

                DestroyAllChild();
                SpawnItemModel(item);
            }
        }
        public GameObject ModelObject => modelObject;
        public bool allowAutoLoot { get; set; } = true;

        #region 유니티 메시지
        private void OnDestroy()
        {
            if (Id >= 0)
            {
                //ItemObjectManager.Instance.RemoveFromManageCollection(this);
            }
        }

        private void DestroyAllChild()
        {
            modelObject = null;
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

            modelObject = Instantiate(item.Data.Model, transform);
        }
        #endregion
    }
}

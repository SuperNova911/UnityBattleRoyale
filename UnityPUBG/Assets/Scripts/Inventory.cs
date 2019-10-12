using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts
{
    public class Inventory : MonoBehaviour
    {
        public SphereCollider collector;
        public ItemContainer container;

        private void Awake()
        {
            //container = ScriptableObject.CreateInstance<ItemContainer>();
        }

        private void Update()
        {
            Item item = Item.EmptyItem;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                item = container.SubtrackItemAtSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                item = container.SubtrackItemAtSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                item = container.SubtrackItemAtSlot(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                item = container.SubtrackItemAtSlot(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                item = container.SubtrackItemAtSlot(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                item = container.SubtrackItemAtSlot(5);
            }

            if (item.IsStackEmpty == false)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100f, LayerMask.GetMask("Terrain")))
                {
                    ItemSpawnManager.Instance.SpawnItemAt(item, hit.point + new Vector3(0, 1, 0));
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var itemObject = other.GetComponent<ItemObject>();
            if (itemObject != null)
            {
                var leftItem = container.AddItem(itemObject.Item);
                if (leftItem.IsStackEmpty)
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }
}

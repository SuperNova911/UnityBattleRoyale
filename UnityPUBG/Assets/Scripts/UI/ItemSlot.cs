using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class ItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 내 시블링 인덱스
        /// </summary>
        private int siblingIndex;

        /// <summary>
        /// 아이템 슬롯 위치
        /// </summary>
        private Vector3 originPosition;

        private bool isDrag = false;

        private GraphicRaycaster graphicRaycaster;

        #region Unity 콜백
        void Start()
        {
            graphicRaycaster = transform.root.GetComponent<GraphicRaycaster>();
        }
        #endregion

        public void OnBeginDrag(PointerEventData eventData)
        {
            siblingIndex = transform.GetSiblingIndex();
            if (EntityManager.Instance.MyPlayer.ItemContainer.Count < siblingIndex + 1)
            {
                isDrag = false;
                return;
            }

            originPosition = transform.position;

            transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
            isDrag = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDrag)
            {
                transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isDrag)
            {
                transform.position = originPosition;
                transform.SetSiblingIndex(siblingIndex);
                transform.parent.GetComponent<GridLayoutGroup>().enabled = true;

                List<RaycastResult> results = new List<RaycastResult>();

                graphicRaycaster.Raycast(eventData, results);

                isDrag = false;
                
                if (results.Count <= 0)
                {
                    return;
                }
                //쓰레기통에 놓은 경우
                else
                {
                    if (results[0].gameObject.name == "TrashCanBackGround")
                    {
                        var dropItem = EntityManager.Instance.MyPlayer.ItemContainer.FindItem(siblingIndex);
                        EntityManager.Instance.MyPlayer.DropItemsAtSlot(siblingIndex, dropItem.CurrentStack);
                    }
                }
            }
        }
    }
}
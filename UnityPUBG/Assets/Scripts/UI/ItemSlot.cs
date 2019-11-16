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

        /// <summary>
        /// 빈 슬롯 이미지
        /// </summary>
        [SerializeField]
        private Sprite emptySlotImage;
        private GameObject slotObject = null;


        #region Unity 콜백
        private void Start()
        {
            graphicRaycaster = transform.root.GetComponent<GraphicRaycaster>();
            siblingIndex = transform.GetSiblingIndex();
        }

        private void OnEnable()
        {
            UpdateSlotObject();
        }
        #endregion

        /// <summary>
        /// 슬롯의 오브젝트 갱신
        /// </summary>
        private void UpdateSlotObject()
        {
            siblingIndex = transform.GetSiblingIndex();

            Items.Item item = EntityManager.Instance.MyPlayer.ItemContainer.FindItem(siblingIndex);

            if (slotObject != null)
                Destroy(slotObject);

            if (item == Items.Item.EmptyItem)
            {
                    return;
            }
            else
            {
                GameObject model = Instantiate(item.Data.Model, Camera.main.ScreenToWorldPoint(transform.position),
                    item.Data.Model.transform.rotation);
                model.transform.localScale = Vector3.one * 0.2f;
                //TODO: 모델을 UI 위에 보이도록 해야함
            }
        }

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
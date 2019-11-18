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
        /// 캔버스의 plane distance
        /// </summary>
        private float planeDistance;

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
        //private GameObject slotObject = null;

        /// <summary>
        /// 현재 슬롯 이미지
        /// </summary>
        private Image slotImage = null;


        #region Unity 콜백
        private void Awake()
        {
            graphicRaycaster = transform.root.GetComponent<GraphicRaycaster>();
            siblingIndex = transform.GetSiblingIndex();
            planeDistance = transform.root.GetComponent<Canvas>().planeDistance;

            slotImage = transform.GetChild(0).GetComponent<Image>();

            Debug.Log(slotImage.gameObject);
        }

        private void Start()
        {
            slotImage.sprite = emptySlotImage;
        }

        private void OnEnable()
        {
            UpdateSlotObject();
        }

        private void OnDisable()
        {
            if (slotImage.sprite != emptySlotImage)
                slotImage.sprite = emptySlotImage;
        }
        #endregion

        /// <summary>
        /// 슬롯의 오브젝트 갱신
        /// </summary>
        public void UpdateSlotObject()
        {
            siblingIndex = transform.GetSiblingIndex();

            Items.Item item = EntityManager.Instance.MyPlayer.ItemContainer.FindItem(siblingIndex);

            if (slotImage.sprite != emptySlotImage)
                slotImage.sprite = emptySlotImage;

            if (item == Items.Item.EmptyItem)
            {
                    return;
            }
            else
            {
                slotImage.sprite = item.Data.Icon;
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

                PointerEventData pointerEventData = new PointerEventData(GetComponent<EventSystem>());
#if !UNITY_ANDRIOD
                pointerEventData.position = Input.mousePosition;
#else
                pointerEventData.position = Input.touches[0].position;
#endif
                graphicRaycaster.Raycast(pointerEventData, results);

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

                        UIManager.Instance.UpdateInventorySlots();
                    }
                }
            }
        }
    }
}
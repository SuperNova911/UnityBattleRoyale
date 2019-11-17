using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class ItemSlot : MonoBehaviour
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
        private GameObject slotObject = null;


        #region Unity 콜백
        private void Awake()
        {
            graphicRaycaster = transform.root.GetComponent<GraphicRaycaster>();
            siblingIndex = transform.GetSiblingIndex();
            planeDistance = transform.root.GetComponent<Canvas>().planeDistance;
        }

        private void Update()
        {
            
            if (!isDrag && Input.GetMouseButtonDown(0))
                BeginDrag();
            else if (isDrag && Input.GetMouseButton(0))
                DoDrag();
            else if (isDrag && Input.GetMouseButtonUp(0))
                EndDrag();
        }

        private void LateUpdate()
        {
            UpdateSlotObjectPosition();
        }

        private void OnEnable()
        {
            UpdateSlotObject();
        }

        private void OnDisable()
        {
            if (slotObject != null)
                Destroy(slotObject);
        }
        #endregion

        /// <summary>
        /// 슬롯의 오브젝트 갱신
        /// </summary>
        public void UpdateSlotObject()
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
                slotObject = Instantiate(item.Data.Model, transform.position,
                    Quaternion.identity);
                slotObject.transform.localScale = Vector3.one * 1f;

                UpdateSlotObjectPosition();
            }
        }

        /// <summary>
        /// 슬롯 오브젝트의 위치 갱신
        /// </summary>
        private void UpdateSlotObjectPosition()
        {
            if(slotObject!=null)
            {
                slotObject.transform.position = transform.position;

                //카메라가 보는 방향
                Vector3 cameraDirection = Camera.main.transform.localRotation * Vector3.forward;

                slotObject.transform.position -= cameraDirection.normalized * 0.1f;
            }
        }

        public void BeginDrag()
        {
            siblingIndex = transform.GetSiblingIndex();
            if (EntityManager.Instance.MyPlayer.ItemContainer.Count < siblingIndex + 1)
            {
                isDrag = false;
                return;
            }
            
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData pointerEventData = new PointerEventData(GetComponent<EventSystem>());
#if !UNITY_ANDRIOD
            pointerEventData.position = Input.mousePosition;
#else
                pointerEventData.position = Input.touches[0].position;
#endif
            graphicRaycaster.Raycast(pointerEventData, results);

            if (results.Count <= 0)
            {
                return;
            }
            else
            {
                //터치 혹은 마우스 포인터가 올라갔는가
                bool isPointerOver = results[0].gameObject.name == gameObject.name
                    || results[0].gameObject.transform.parent.name == gameObject.name;

                if (!isPointerOver)
                {
                    return;
                }
            }

            originPosition = transform.position;
            
            transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
            isDrag = true;
        }

        public void DoDrag()
        {
            if (isDrag)
            {
                Vector3 screenPoint;
#if !UNITY_ANDRIOD
                screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, planeDistance);
#else
                screenPoint = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, planeDistance);
#endif
                transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
               // UpdateSlotObjectPosition();
            }
        }

        public void EndDrag()
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
                        Destroy(slotObject);
                        UIManager.Instance.UpdateInventorySlots();
                    }
                }
            }
        }
    }
}
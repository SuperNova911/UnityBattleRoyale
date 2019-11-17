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

        public void BeginDrag()
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
                    }
                }
            }
        }
    }
}
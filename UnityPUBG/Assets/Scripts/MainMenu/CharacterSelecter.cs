using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby
{
    public class CharacterSelecter : MonoBehaviour
    {
        /// <summary>
        /// 캐릭터 3D 모델 프리팹 리스트들
        /// </summary>
        public List<GameObject> CharacterPrefabList = new List<GameObject>();

        public int RotateSpeed;

        #region private values

        /// <summary>
        /// 캐릭터 리스트
        /// </summary>
        List<GameObject> characterList = new List<GameObject>();

        /// <summary>
        /// 각도
        /// </summary>
        float deg;

        /// <summary>
        /// 드래그 중인가
        /// </summary>
        bool isDrag = false;

        /// <summary>
        /// 마우스 최근 위치
        /// </summary>
        Vector3 lastpos;

        /// <summary>
        /// 최근 Selecter 회전율
        /// </summary>
        Vector3 lastrot;
        #endregion

        private void Start()
        {
            putCharacter();
        }

        private void Update()
        {
            Vector3 nowpos;

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    lastpos = Input.GetTouch(0).position;
                    lastrot = transform.rotation.eulerAngles;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    lastrot = transform.rotation.eulerAngles;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    nowpos = Input.GetTouch(0).position;

                    int x = (int)((nowpos - lastpos).x / Screen.width * RotateSpeed);

                    transform.rotation = Quaternion.Euler
                            (Vector3.up * deg * x + lastrot);
                }
            }
#else
            // 마우스가 눌림
            if (Input.GetMouseButtonDown(0))
            {
                isDrag = true;
                lastrot = transform.rotation.eulerAngles;
                lastpos = Input.mousePosition;
            }

            // 마우스가 떼짐
            if (Input.GetMouseButtonUp(0))
            {
                isDrag = false;
                lastrot = transform.rotation.eulerAngles;
                //Debug.Log(GetClickedObject());
            }

            if (isDrag)
            {
                nowpos = Input.mousePosition;

                int x = (int)((nowpos - lastpos).x / Screen.width * RotateSpeed);

                transform.rotation = Quaternion.Euler
                    (Vector3.up * deg * x + lastrot);
            }
#endif
        }

        /// <summary>
        /// 리스트에 있는 캐릭터 들을 원형으로 배치
        /// </summary>
        void putCharacter()
        {
            float num = CharacterPrefabList.Count;

            //원주
            float circlelen = 3 * num;

            //반지름
            float r = circlelen / (2 * Mathf.PI);

            //원의 중심
            Vector3 centerpos = transform.position - Vector3.forward * r;

            //중심 변경
            transform.position = centerpos;

            //1개 각도
            deg = 360f / num;

            //1개 각도(라디안)
            float rad = Mathf.Deg2Rad * (360f / num);

            for (int i = 0; i < num; i++)
            {
                GameObject tmp = Instantiate(CharacterPrefabList[i]);

                //위치 설정
                tmp.transform.position = centerpos
                    + Vector3.right * Mathf.Cos(rad * i) * r
                    + Vector3.back * Mathf.Sin(rad * i) * r;

                //회전 설정
                tmp.transform.rotation = Quaternion.Euler(Vector3.up * (-90f + deg * i));

                characterList.Add(tmp);

                tmp.transform.SetParent(transform);
            }

            Camera.main.transform.position = centerpos + Vector3.forward * 7f + Vector3.up;
        }

        private GameObject GetClickedObject()//전면의 GameObject return
        {
            RaycastHit hit;
            GameObject target = null;

            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(Vector3.zero));//마우스 포인터 근처 좌표 만듬

            if (true == (Physics.Raycast(ray.origin, ray.direction, out hit)))//마우스 근처에 오브젝트 있는지 확인
                target = hit.collider.gameObject;//있다면 오브젝트 저장

            return target;
        }

    }
}
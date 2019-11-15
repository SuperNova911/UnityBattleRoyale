using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.MainMenu
{
    public class CharacterSelecter : MonoBehaviour
    {
        /// <summary>
        /// 캐릭터 3D 모델 프리팹 리스트들
        /// </summary>
        public List<GameObject> CharacterPrefabList = new List<GameObject>();
        public int RotateSpeed;
        /// <summary>
        /// 플레이어 캐릭터 프리팹
        /// </summary>
        public GameObject PlayerCharacter;

        /// <summary>
        /// 캐릭터 리스트
        /// </summary>
        private List<GameObject> characterList = new List<GameObject>();
        /// <summary>
        /// 각도
        /// </summary>
        private float deg;
        /// <summary>
        /// 드래그 중인가
        /// </summary>
        private bool isDrag = false;
        /// <summary>
        /// 마우스 최근 위치
        /// </summary>
        private Vector3 lastpos;
        /// <summary>
        /// 최근 Selecter 회전율
        /// </summary>
        private Vector3 lastrot;
        /// <summary>
        /// 선택된 캐릭터 이름
        /// </summary>
        private string selectedCharacterName;

        #region 유니티 메시지
        private void Start()
        {
            putCharacter();
            setSelectedCharacter();

            //나중에 주석 해제할 것
            DontDestroyOnLoad(gameObject);
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

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    nowpos = Input.GetTouch(0).position;

                    int x = (int)((nowpos - lastpos).x / Screen.width * RotateSpeed);

                    transform.rotation = Quaternion.Euler
                            (Vector3.up * deg * x + lastrot);
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    lastrot = transform.rotation.eulerAngles;
                    setSelectedCharacter();
                }
            }
#else
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
                return;

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
                setSelectedCharacter();

                //Debug.Log(selectedCharacterName);
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
        #endregion

        /// <summary>
        /// 캐릭터를 스폰함.
        /// </summary>
        public void SpawnMyCharacter(Transform spawnPos)
        {
            spawnMyCharacter(spawnPos);
        }

        /// <summary>
        /// 리스트에 있는 캐릭터 들을 원형으로 배치
        /// </summary>
        private void putCharacter()
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

        /// <summary>
        /// 전면의 gameObject return
        /// </summary>
        /// <returns></returns>
        private GameObject getFrontObject()
        {
            RaycastHit hit;
            GameObject target = null;

            //화면 가운데 좌표에 레이캐스트
            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(Vector3.zero));

            //게임 오브젝트가 존재한다면 return
            if (true == (Physics.Raycast(ray.origin, ray.direction, out hit)))
                target = hit.collider.gameObject;//있다면 오브젝트 저장

            return target;
        }

        /// <summary>
        /// 선택된 캐릭터 이름 저장
        /// </summary>
        private void setSelectedCharacter()
        {
            GameObject selectedCharacter;
            selectedCharacter = getFrontObject();

            string[] splitedName = selectedCharacter.name.Split('_');

            selectedCharacterName = splitedName[0] + "_" + splitedName[1];
        }

        /// <summary>
        /// 내가 선택한 캐릭터 스폰
        /// </summary>
        private void spawnMyCharacter(Transform spawnPos)
        {
            GameObject playerCharacter = PhotonNetwork.Instantiate(PlayerCharacter.name, spawnPos.position, Quaternion.identity, 0);
            if (playerCharacter.GetComponent<PhotonView>().isMine)
            {
                CameraManager.Instance.PlayerCamera.Follow = playerCharacter.transform;
            }

            foreach (Transform child in playerCharacter.transform)
            {
                if (child.name.StartsWith("Character"))
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (selectedCharacterName != "Character_Random")
            {
                playerCharacter.transform.Find(selectedCharacterName).gameObject.SetActive(true);
            }
            else
            {
                int randomNum = Random.Range(0, 100) % playerCharacter.transform.childCount;

                playerCharacter.transform.GetChild(randomNum).gameObject.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
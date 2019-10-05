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

        /// <summary>
        /// 캐릭터 리스트
        /// </summary>
        List<GameObject> characterList = new List<GameObject>();

        private void Start()
        {
            putCharacter();
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
            float deg = 360f / num;

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
        }
    }
}
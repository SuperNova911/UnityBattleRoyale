using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.CharacterSpawn
{
    public class CharacterSpawner : MonoBehaviour
    {
        #region private 변수

        private MainMenu.CharacterSelecter characterSelecter;

        /// <summary>
        /// 내 캐릭터를 스폰했는가?
        /// </summary>
        private bool isSpawned = false;

        #endregion

        #region 유니티 콜백

        private void Start()
        {
            characterSelecter = FindObjectOfType<MainMenu.CharacterSelecter>().
                GetComponent<MainMenu.CharacterSelecter>();
        }

        private void Update()
        {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            if (Input.touchCount > 0 && !isSpawned)
            {
                characterSelecter.SpawnMyCharacter(transform);

                isSpawned = true;
            }
#else
            // 마우스가 눌림
            if (Input.anyKey && !isSpawned)
            {
                characterSelecter.SpawnMyCharacter(transform);

                isSpawned = true;
            }
#endif
        }

        #endregion
    }
}
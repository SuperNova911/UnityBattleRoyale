using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.MainMenu;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.CharacterSpawn
{
    public class CharacterSpawner : MonoBehaviour
    {
        private CharacterSelecter characterSelecter;

        // 내 캐릭터를 스폰했는가?
        private bool isSpawned = false;

        #region 유니티 메시지
        private void Awake()
        {
            characterSelecter = FindObjectOfType<CharacterSelecter>().
                                GetComponent<CharacterSelecter>();
        }

        private void Start()
        {
            characterSelecter.SpawnMyCharacter(transform.position);
            Destroy(gameObject);
        }

        /*
        private void Update()
        {
            //if (inputManager.CharacterSpawner.Spawn.ReadValue<bool>() && isSpawned == false)
            //{
            //    characterSelecter.SpawnMyCharacter(transform);
            //    isSpawned = true;
            //}

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
                        if (Input.touchCount > 0 && !isSpawned)
                        {
                            characterSelecter.SpawnMyCharacter(transform);

                            isSpawned = true;
                        }
#else
            if (Input.anyKey && !isSpawned)
            {
                characterSelecter.SpawnMyCharacter();

                isSpawned = true;
            }
#endif
        }
        */
        #endregion
    }
}
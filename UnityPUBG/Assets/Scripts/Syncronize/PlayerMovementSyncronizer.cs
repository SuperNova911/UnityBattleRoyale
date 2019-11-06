using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Syncronize
{
    public class PlayerMovementSyncronizer : Photon.MonoBehaviour
    {
        /// <summary>
        /// player 프리팹을 움직이는 함수를 조작하기 위해서
        /// 갖고온 컴포넌트
        /// </summary>
        private Entities.Player player;

        private void Awake()
        {
            player = GetComponent<Entities.Player>();
        }

        private void Update()
        {
            if (photonView.isMine)
            {
                player.ControlMyMovement();

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    player.MyMeleeAttack(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
                }
            }
        }
    }
}
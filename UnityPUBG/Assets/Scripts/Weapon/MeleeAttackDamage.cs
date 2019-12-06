using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts
{
    public class MeleeAttackDamage : MonoBehaviour
    {
        // 공격 소유자
        private Entities.Player EnemyOwner = null;

        private readonly static string enemyTag = "Enemy";
        private readonly static string myPlayerTag = "MyPlayer";
        private readonly static string enemyWeaponTag = "EnemyWeapon";
        private readonly static string myWeaponTag = "MyWeapon";

        private void Awake()
        {
            if(transform.root.tag == enemyTag)
            {
                EnemyOwner = transform.root.GetComponent<Entities.Player>();
                gameObject.tag = enemyWeaponTag;
            }
            else if(transform.root.tag == myPlayerTag)
            {
                gameObject.tag = myWeaponTag;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (EnemyOwner == null)
            {
                return;
            }

            if(gameObject.tag == enemyWeaponTag)
            {
                if (other.tag == myPlayerTag)
                {
                    if (EnemyOwner.IsPlayingAttackAnimation)
                    {
                        other.GetComponent<Entities.IDamageable>().OnTakeDamage
                            (Random.Range(0f, 100f), Utilities.DamageType.Normal);

                        //넉백
                        other.attachedRigidbody.AddForce(EnemyOwner.transform.forward * EnemyOwner.KnockBackPower);
                    }
                }
            }
        }
    }
}
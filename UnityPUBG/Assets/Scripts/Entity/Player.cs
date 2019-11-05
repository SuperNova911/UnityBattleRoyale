using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityPUBG.Scripts.Entities
{
    public class Player : Entity
    {
        #region 필드
        private InputManager inputManager;
        #endregion

        #region 속성
        public WeaponData EquipedWeapon { get; private set; }
        public ArmorData EquipedArmor { get; private set; }
        public BackpackData EquipedBackpack { get; private set; }
        public Vehicle RidingVehicle { get; private set; }
        #endregion

        #region 유니티 메시지
        protected override void Awake()
        {
            base.Awake();

            inputManager = new InputManager();
        }

        protected override void Update()
        {
            base.Update();

            //ControlMovement();
            //if (Input.GetKeyDown(KeyCode.Mouse0))
            //{
            //   MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
            //}
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void OnEnable()
        {
            inputManager.Enable();
        }

        private void OnDisable()
        {
            inputManager.Disable();
        }
        #endregion

        #region 메서드
        /// <summary>
        /// InputSystem으로부터 입력받은 값을 기반으로 Player의 움직임을 컨트롤
        /// </summary>
        private void ControlMovement()
        {
            Vector2 direction = inputManager.Player.Movement.ReadValue<Vector2>();
            movementDirection = new Vector3(direction.x, 0, direction.y);
        }

        private void MeleeAttackTest(float damage, DamageType damageType)
        {
            Vector3 attackOriginPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 attackDirection = transform.forward;

            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var mouseRayHit, 100f, LayerMask.GetMask("Terrain")))
            {
                attackDirection = (new Vector3(mouseRayHit.point.x, 0, mouseRayHit.point.z) - transform.position).normalized;
            }

            float attackRange = 2f;
            float attackAngle = 90f;
            int rayNumber = 10;

            Vector3 leftRayDirection = Quaternion.Euler(0, -attackAngle / 2, 0) * attackDirection;
            Vector3 rightRayDirection = Quaternion.Euler(0, attackAngle / 2, 0) * attackDirection;

            HashSet<IDamageable> hitObjects = new HashSet<IDamageable>();

            for (int i = 0; i < rayNumber; i++)
            {
                Vector3 rayDirection = Vector3.Lerp(leftRayDirection, rightRayDirection, i / (float)(rayNumber - 1)).normalized;
                if (Physics.Raycast(attackOriginPosition, rayDirection, out var hit, attackRange))
                {
                    var damageableObject = hit.transform.GetComponent<IDamageable>();
                    if (damageableObject != null)
                    {
                        hitObjects.Add(damageableObject);
                    }

                    Debug.DrawLine(attackOriginPosition, hit.point, Color.red, 0.1f);
                }
                else
                {
                    Debug.DrawRay(attackOriginPosition, rayDirection * attackRange, Color.yellow, 0.1f);
                }
            }

            foreach (var hitObject in hitObjects)
            {
                hitObject.OnTakeDamage(damage, damageType);
            }
        }
        #endregion

        #region public 함수

        /// <summary>
        /// PlayerMovementSyncronizer 스크립트에서
        /// 이 함수를 이용해서 플레이어를 움직임
        /// </summary>
        public void ControlMyMovement()
        {
            ControlMovement();
        }

        /// <summary>
        /// PlayerMovementSyncronizer 스크립트에서
        /// 이 함수를 이용해서 플레이어가 공격하도록 함
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="damageType"></param>
        public void MyMeleeAttack(float damage, DamageType damageType)
        {
            MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
        }

        #endregion
    }
}

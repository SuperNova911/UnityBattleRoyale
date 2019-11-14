using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.Entities
{
    [RequireComponent(typeof(PhotonView))]
    public class Player : Entity
    {
        private PhotonView photonView;
        private FixedJoystick joyStick;
        private InputManager inputManager;

        private Slider hpBar;
        private Slider shieldBar;

        private Text hpText;
        private Text shieldText;

        public WeaponData EquipedWeapon { get; private set; }
        public ArmorData EquipedArmor { get; private set; }
        public BackpackData EquipedBackpack { get; private set; }
        public Vehicle RidingVehicle { get; private set; }

        public int PhotonViewId => photonView.viewID;
        public bool IsMyPlayer => photonView.isMine;

        #region 유니티 메시지
        protected override void Awake()
        {
            base.Awake();

            photonView = GetComponent<PhotonView>();
            inputManager = new InputManager();
            if (photonView == null)
            {
                Debug.LogError($"{nameof(photonView)}가 없습니다");
            }

            if (IsMyPlayer)
            {
                EntityManager.Instance.MyPlayer = this;

                InitializeUIObjects();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (photonView.isMine)
            {
                ControlMovement();
#if !UNITY_ANDRIOD
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
                }
#endif
                hpBar.value = currentHealth;
                hpText.text = maximumHealth.ToString() + "/" + hpBar.value.ToString();
            }
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

        private void InitializeUIObjects()
        {
            var uiObjects = GameObject.FindGameObjectsWithTag("UI");

            //조이스틱 매핑
            var joyStick = uiObjects.FirstOrDefault(e => e.name == "Joystick").GetComponent<FixedJoystick>();
            if (joyStick == null)
            {
                Debug.LogError($"{nameof(FixedJoystick)}이 없습니다");
            }

#if !UNITY_ANDRIOD
            //공격 버튼 매핑
            Button attackButton = uiObjects.FirstOrDefault(e => e.name == "AttackButton").GetComponent<Button>();
            if (attackButton == null)
            {
                Debug.LogError($"{nameof(attackButton)}이 없습니다");
            }

            attackButton.onClick.AddListener(() => MeleeAttackTest(DamageType.Normal));
#endif

            hpBar = uiObjects.FirstOrDefault(e => e.name == "HPBar").GetComponent<Slider>();
            if (hpBar == null)
            {
                Debug.LogError($"{nameof(hpBar)}가 없습니다");
            }
            else
            {
                hpText = hpBar.transform.Find("HPText").GetComponent<Text>();
                if (hpText == null)
                {
                    Debug.LogError($"{nameof(hpText)}가 없습니다");
                }
            }

            shieldBar = uiObjects.FirstOrDefault(e => e.name == "ShieldBar").GetComponent<Slider>();
            if (shieldBar == null)
            {
                Debug.LogError($"{nameof(shieldBar)}가 없습니다");
            }
            else
            {
                shieldText = shieldBar.transform.Find("ShieldText").GetComponent<Text>();
                if (shieldText == null)
                {
                    Debug.LogError($"{nameof(shieldText)}가 없습니다");
                }
            }
        }

        /// <summary>
        /// InputSystem으로부터 입력받은 값을 기반으로 Player의 움직임을 컨트롤
        /// </summary>
        private void ControlMovement()
        {
            Vector2 direction;
#if !UNITY_ANDROID
            direction = inputManager.Player.Movement.ReadValue<Vector2>();
#else
            direction = joyStick.Direction;
#endif
            movementDirection = new Vector3(direction.x, 0, direction.y);
        }

        // 테스트 전용
        private void MeleeAttackTest(DamageType damageType)
        {
            MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), damageType);
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
    }
}

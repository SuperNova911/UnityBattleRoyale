using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Entities
{
    [RequireComponent(typeof(PhotonView))]
    public class Player : Entity, IPunObservable
    {
        [SerializeField, Range(0, 100)] private int maximumShield = 100;
        [SerializeField, Range(0f, 100f)] private float currentShield = 0f;

        [Header("ItemContainer")]
        [SerializeField, Range(2, 8)] private int defaultContainerCapacity = 6;

        [Header("QuickSlot")]
        [SerializeField, Range(0.1f, 1f)] private float speedMultiplyWhenUseItem = 0.5f;
        [SerializeField, Range(1, 6)] private int quickBarCapacity = 4;

        private PhotonView photonView;
        private FloatingJoystick movementJoystick;
        private FloatingJoystick attackJoystick;
        private InputManager inputManager;

        public EventHandler<float> OnCurrentShieldUpdate;

        public int MaximumShield
        {
            get { return maximumShield; }
            set { maximumShield = value; }
        }
        public float CurrentShield
        {
            get { return currentShield; }
            set
            {
                currentShield = value;
                currentShield = Mathf.Clamp(currentShield, 0f, MaximumShield);

                OnCurrentShieldUpdate?.Invoke(this, currentShield);
            }
        }
        public ItemContainer ItemContainer { get; private set; }
        public List<Item> ItemQuickBar { get; private set; }
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

            CurrentShield = MaximumShield / 2f;     // Test value
            ItemContainer = new ItemContainer(defaultContainerCapacity);
            ItemQuickBar = new List<Item>(quickBarCapacity);

            if (IsMyPlayer)
            {
                EntityManager.Instance.MyPlayer = this;
                InitializeUIObjects();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                CurrentHealth -= Mathf.PI;
            }
            else if (Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                CurrentShield -= Mathf.PI;
            }
            if (IsDead)
            {
                return;
            }

            if (photonView.isMine)
            {
                ControlMovement();
#if !UNITY_ANDRIOD
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
                }
#endif
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

        #region IPunObservable 인터페이스
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // 체력 동기화
                stream.SendNext(CurrentHealth);
            }
            else
            {
                CurrentHealth = (float)stream.ReceiveNext();
            }
        }
        #endregion

        /// <summary>
        /// 특정 슬롯에 있는 아이템을 입력으로 받은 스택만큼 ItemObject를 드랍
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="dropStack"></param>
        public void DropItemsAtSlot(int slot, int dropStack)
        {
            if (ItemContainer.Capacity <= slot)
            {
                Debug.LogWarning($"{nameof(ItemContainer)}의 {nameof(ItemContainer.Capacity)} 범위를 벗어나는 {nameof(slot)}입니다");
                return;
            }

            var dropItem = ItemContainer.SubtrackItemsAtSlot(slot, dropStack);
            if (dropItem.IsStackEmpty)
            {
                return;
            }

            var itemObject = ItemSpawnManager.Instance.SpawnItemObjectAt(dropItem, transform.position + new Vector3(0, 1.5f, 0));
            if (itemObject == null)
            {
                return;
            }

            itemObject.AllowAutoLoot = false;

            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            var itemObjectRigidbody = itemObject.GetComponent<Rigidbody>();
            if (itemObjectRigidbody != null)
            {
                float force = 6f;
                itemObjectRigidbody.AddForce(new Vector3(randomDirection.x, 0.5f, randomDirection.y).normalized * force, ForceMode.Impulse);
            }
        }

        public void AssignItemToQuickBar(int slot, Item item)
        {
            if (item == null || item.IsStackEmpty)
            {
                Debug.LogError($"null이거나 비어있는 {nameof(Item)}은 등록할 수 없습니다");
                return;
            }
            
            if (slot < 0 || slot >= quickBarCapacity)
            {
                Debug.LogWarning($", {nameof(slot)}: {slot}");
                slot = Mathf.Clamp(slot, 0, quickBarCapacity);
            }

            ItemQuickBar[slot] = item;
        }

        public void UseItemAtQuickBar(int slot)
        {

        }

        public void UseItemAtItemContainer(int slot)
        {

        }

        private void InitializeUIObjects()
        {
            var uiObjects = GameObject.FindGameObjectsWithTag("UI");

            //이동 조이스틱 매핑
            movementJoystick = uiObjects.FirstOrDefault(e => e.name == "Movement Joystick").GetComponent<FloatingJoystick>();
            if (movementJoystick == null)
            {
                Debug.LogError($"{nameof(movementJoystick)}이 없습니다");
            }

            //공격 조이스틱 매핑
            attackJoystick = uiObjects.FirstOrDefault(e => e.name == "Attack Joystick").GetComponent<FloatingJoystick>();
            if (attackJoystick == null)
            {
                Debug.LogError($"{nameof(attackJoystick)}이 없습니다");
            }

            /*
#if !UNITY_ANDRIOD
            //공격 버튼 매핑
            Button attackButton = uiObjects.FirstOrDefault(e => e.name == "AttackButton").GetComponent<Button>();
            if (attackButton == null)
            {
                Debug.LogError($"{nameof(attackButton)}이 없습니다");
            }

            attackButton.onClick.AddListener(() => MeleeAttackTest(DamageType.Normal));
#endif
*/
        }

        /// <summary>
        /// InputSystem으로부터 입력받은 값을 기반으로 Player의 움직임을 컨트롤
        /// </summary>
        private void ControlMovement()
        {
            Vector2 direction;
            direction = inputManager.Player.Movement.ReadValue<Vector2>();
            if (direction == Vector2.zero)
            {
                direction = movementJoystick.Direction;
            }
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

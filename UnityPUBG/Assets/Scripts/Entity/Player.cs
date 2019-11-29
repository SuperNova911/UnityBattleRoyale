using System;
using System.Collections;
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
        [Header("Player Settings")]
        [SerializeField, Range(0, 100)] private int maximumShield = 100;
        [SerializeField, Range(0f, 100f)] private float currentShield = 0f;

        [Header("ItemContainer")]
        [SerializeField, Range(2, 8)] private int defaultContainerCapacity = 6;

        [Header("QuickSlot")]
        [SerializeField, Range(0.1f, 1f)] private float speedMultiplyWhenUseItem = 0.5f;
        [SerializeField, Range(1, 6)] private int quickBarCapacity = 4;

        [Header("Projectile")]
        public ProjectileBase projectileBasePrefab;
        public Transform projectileFirePosition;

        private PhotonView photonView;
        private InputManager inputManager;
        private PlayerItemLooter myItemLooter;

        // Weapon
        private float lastAttackTime = 0f;
        private bool isAiming = false;

        public event EventHandler<float> OnCurrentShieldUpdate;

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
        public Item[] ItemQuickBar { get; private set; }
        public Item EquipedWeapon { get; private set; }   // TODO: 장착은 메서드로 바꾸기
        public Item EquipedArmor { get; private set; }
        public Item EquipedBackpack { get; private set; }
        public Vehicle RidingVehicle { get; private set; }

        public int PhotonViewId => photonView.viewID;
        public bool IsMyPlayer => photonView.isMine;

        public bool IsAiming
        {
            get { return isAiming; }
            set
            {
                isAiming = value;
                // TODO: 조준선 그리기
            }
        }

        #region 유니티 메시지
        protected override void Awake()
        {
            base.Awake();

            ObjectPoolManager.Instance.InitializeObjectPool(projectileBasePrefab.gameObject, 20);

            photonView = GetComponent<PhotonView>();
            inputManager = new InputManager();
            if (photonView == null)
            {
                Debug.LogError($"{nameof(photonView)}가 없습니다");
            }

            CurrentShield = MaximumShield / 2f;     // Test value
            ItemContainer = new ItemContainer(defaultContainerCapacity);
            ItemQuickBar = new Item[quickBarCapacity];

            //장착하지 않았으므로 emptyItem으로 초기화
            EquipedWeapon = Item.EmptyItem;
            EquipedArmor = Item.EmptyItem;
            EquipedBackpack = Item.EmptyItem;

            for (int slot = 0; slot < ItemQuickBar.Length; slot++)
            {
                ItemQuickBar[slot] = Item.EmptyItem;
            }

            if (IsMyPlayer)
            {
                EntityManager.Instance.MyPlayer = this;
                myItemLooter = GetComponentInChildren<PlayerItemLooter>();
            }
        }

        protected override void Start()
        {
            base.Start();

            if (IsMyPlayer)
            {
                UIManager.Instance.UpdateInventoryItemSlotSize();
            }
        }

        protected override void Update()
        {
            base.Update();

            if (IsDead)
            {
                return;
            }

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                //if (myItemLooter.LootableItemObjects.Count > 0)
                //{
                //    LootItem(myItemLooter.LootableItemObjects[0]);
                //    myItemLooter.LootableItemObjects.RemoveAt(0);
                //}
            }

            if (Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                var overflows = ItemContainer.ResizeCapacity(ItemContainer.Capacity - 2);
                foreach (var item in overflows)
                {
                    DropItem(item);
                }
            }
            else if (Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                var overflows = ItemContainer.ResizeCapacity(ItemContainer.Capacity + 2);
                foreach (var item in overflows)
                {
                    DropItem(item);
                }
            }
            else if (Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                DropBackpack();
            }
            else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                DropArmor();
            }
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                DropWeapon();
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                CurrentShield -= 9.11f;
                CurrentHealth -= 9.11f;
            }

            if (photonView.isMine)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    RangeAttack(transform.forward);
                }

#if !UNITY_ANDRIOD
                //if (Input.GetKeyDown(KeyCode.Mouse0))
                //{
                //    MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
                //}
#endif
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsMyPlayer)
            {
                EntityManager.Instance.MyPlayer = null;
            }
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

        public void MoveTo(Vector2 direction)
        {
            MovementDirection = direction.normalized;
        }

        public void RotateTo(Vector2 direction)
        {
            if (IsAiming == false)
            {
                RotateDirection = direction.normalized;
            }
        }

        public void AimTo(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                RotateDirection = Vector2.zero;
                IsAiming = false;
            }
            else
            {
                RotateDirection = direction.normalized;
                IsAiming = true;
            }
        }

        public void AttackTo(Vector2 direction)
        {
            Vector3 attackDirection;
            if (direction == Vector2.zero)
            {
                attackDirection = transform.forward;
            }
            else
            {
                attackDirection = new Vector3(direction.x, 0, direction.y);
            }
            attackDirection = attackDirection.normalized;

            if (EquipedWeapon.IsStackEmpty)
            {
                // 맨손 공격
                MeleeAttackTest(attackDirection, UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
            }
            else
            {
                switch (EquipedWeapon.Data)
                {
                    case MeleeWeaponData meleeWeaponData:
                        MeleeAttackTest(attackDirection, UnityEngine.Random.Range(0f, 100f), meleeWeaponData.DamageType);
                        break;
                    case RangeWeaponData rangeWeaponData:
                        RangeAttack(attackDirection);
                        break;
                    default:
                        Debug.LogError($"관리되지 않고 있는 {nameof(WeaponData)}입니다, {EquipedWeapon.Data.GetType().Name}");
                        break;
                }
            }
        }

        // TODO: 쉴드
        public void EquipArmor(Item armorItem)
        {
            if ((armorItem.Data is ArmorData) == false)
            {
                Debug.LogWarning($"착용하려는 아이템이 {nameof(ArmorData)}가 아닙니다, {nameof(ItemData)}: {armorItem.Data.GetType().Name}");
                return;
            }

            if (EquipedArmor.IsStackEmpty == false)
            {
                DropItem(EquipedArmor);
            }
            EquipedArmor = armorItem;
        }

        public void EquipBackpack(Item backpackItem)
        {
            if ((backpackItem.Data is BackpackData) == false)
            {
                Debug.LogWarning($"착용하려는 아이템이 {nameof(BackpackData)}가 아닙니다, {nameof(ItemData)}: {backpackItem.Data.GetType().Name}");
                return;
            }

            var backpackData = backpackItem.Data as BackpackData;
            if (ItemContainer.Capacity != defaultContainerCapacity + backpackData.BonusCapacity)
            {
                var overflowItems = ItemContainer.ResizeCapacity(defaultContainerCapacity + backpackData.BonusCapacity);
                foreach (var item in overflowItems)
                {
                    DropItem(item);
                }
            }

            if (EquipedBackpack.IsStackEmpty == false)
            {
                DropItem(EquipedBackpack);
            }
            EquipedBackpack = backpackItem;
        }

        public void EquipWeapon(Item weaponItem)
        {
            if ((weaponItem.Data is WeaponData) == false)
            {
                Debug.LogWarning($"착용하려는 아이템이 {nameof(WeaponData)}가 아닙니다, {nameof(ItemData)}: {weaponItem.Data.GetType().Name}");
                return;
            }

            if (EquipedWeapon.IsStackEmpty == false)
            {
                DropItem(EquipedWeapon);
            }
            EquipedWeapon = weaponItem;
        }

        // TODO: 쉴드 제거
        public void DropArmor()
        {
            if (EquipedArmor.IsStackEmpty == false)
            {
                DropItem(EquipedArmor);
                EquipedArmor = Item.EmptyItem;
            }
        }

        public void DropBackpack()
        {
            if (EquipedBackpack.IsStackEmpty == false)
            {
                var overflowItems = ItemContainer.ResizeCapacity(defaultContainerCapacity);
                foreach (var item in overflowItems)
                {
                    DropItem(item);
                }

                DropItem(EquipedBackpack);
                EquipedBackpack = Item.EmptyItem;
            }
        }

        public void DropWeapon()
        {
            if (EquipedWeapon.IsStackEmpty == false)
            {
                DropItem(EquipedWeapon);
                EquipedWeapon = Item.EmptyItem;
            }
        }

        public void LootItem(ItemObject lootItemObject)
        {
            if (lootItemObject.Item.IsStackEmpty)
            {
                return;
            }

            if (lootItemObject.Item.Data is ArmorData)
            {
                EquipArmor(lootItemObject.Item);
                LootAnimator.Instance.CreateNewLootAnimation(this, lootItemObject);
                lootItemObject.RequestDestroy();
            }
            else if (lootItemObject.Item.Data is BackpackData)
            {
                EquipBackpack(lootItemObject.Item);
                LootAnimator.Instance.CreateNewLootAnimation(this, lootItemObject);
                lootItemObject.RequestDestroy();
            }
            else if (lootItemObject.Item.Data is WeaponData)
            {
                if (EquipedWeapon.IsStackEmpty || EquipedWeapon.Data.Rarity < lootItemObject.Item.Data.Rarity)
                {
                    EquipWeapon(lootItemObject.Item);
                }
                else
                {
                    ItemContainer.AddItem(lootItemObject.Item);
                }
                LootAnimator.Instance.CreateNewLootAnimation(this, lootItemObject);
                lootItemObject.RequestDestroy();
            }
            else
            {
                int previousStack = lootItemObject.Item.CurrentStack;
                var remainItem = ItemContainer.AddItem(lootItemObject.Item);

                if (previousStack != remainItem.CurrentStack)
                {
                    LootAnimator.Instance.CreateNewLootAnimation(this, lootItemObject);
                    lootItemObject.Item = remainItem;

                    if (lootItemObject.Item.IsStackEmpty)
                    {
                        lootItemObject.RequestDestroy();
                    }
                    else
                    {
                        lootItemObject.NotifyUpdateCurrentStack();
                    }
                }
            }
        }

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

            DropItem(dropItem);
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
                Debug.LogWarning($"퀵바의 범위를 벗어나는 슬롯 인덱스, {nameof(slot)}: {slot}");
                slot = Mathf.Clamp(slot, 0, quickBarCapacity - 1);
            }

            //퀵슬롯에 동일한 아이템이 들어가있지 않도록 함
            for (int i = 0; i < quickBarCapacity; i++)
            {
                if (ItemQuickBar[i] == item)
                {
                    ItemQuickBar[i].ClearStack();
                    break;
                }
            }

            ItemQuickBar[slot] = item;

            UIManager.Instance.UpdateInventorySlots();
            UIManager.Instance.UpdateQuickSlots();
        }

        public void UseItemAtQuickBar(int slot)
        {
            if (slot < 0 || slot >= quickBarCapacity)
            {
                Debug.LogWarning($"퀵바의 범위를 벗어나는 슬롯 인덱스, {nameof(slot)}: {slot}");
                slot = Mathf.Clamp(slot, 0, quickBarCapacity - 1);
            }

            var selectedItem = ItemQuickBar[slot];
            if (selectedItem.IsStackEmpty)
            {
                return;
            }

            switch (selectedItem.Data)
            {
                case ConsumableData consumable:
                    StartCoroutine(TryConsumeItem(selectedItem));
                    break;

                case WeaponData weapon:
                    // switch weapon
                    break;
            }
        }

        public void UseItemAtItemContainer(int slot)
        {

        }

        // 테스트 전용
        private void MeleeAttackTest(DamageType damageType)
        {
            MeleeAttackTest(UnityEngine.Random.Range(0f, 100f), damageType);
        }

        private void MeleeAttackTest(float damage, DamageType damageType)
        {
            Vector3 attackDirection = transform.forward;

            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var mouseRayHit, 100f, LayerMask.GetMask("Terrain")))
            {
                attackDirection = (new Vector3(mouseRayHit.point.x, 0, mouseRayHit.point.z) - transform.position).normalized;
            }
            MeleeAttackTest(attackDirection, damage, damageType);
        }

        private void MeleeAttackTest(Vector3 attackDirection, float damage, DamageType damageType)
        {
            float attackRange = 2f;
            float attackAngle = 90f;
            int rayNumber = 10;

            Vector3 attackOriginPosition = transform.position + new Vector3(0, 1, 0);
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

        // TODO: Cast Delay 구현
        private void RangeAttack(Vector3 attackDirection)
        {
            var rangeWeaponData = EquipedWeapon.Data as RangeWeaponData;
            var requireAmmoData = rangeWeaponData.RequireAmmo;
            if (requireAmmoData == null)
            {
                Debug.LogWarning($"탄약이 필요하지 않는 무기는 구현하지 않음, {rangeWeaponData.ItemName}");
                return;
            }

            // 쿨다운 검사
            if (lastAttackTime + rangeWeaponData.AttackCooldown > Time.time)
            {
                return;
            }

            // 필요한 탄약이 있는지 검사
            int ammoSlot = ItemContainer.FindMatchItemSlotFromLast(rangeWeaponData.RequireAmmo.ItemName);
            if (ammoSlot < 0)
            {
                return;
            }
            ItemContainer.SubtrackItemAtSlot(ammoSlot);

            // Projectile 설정
            var projectileBase = ObjectPoolManager.Instance.ReuseObject(projectileBasePrefab.gameObject).GetComponent<ProjectileBase>();
            var projectileInfo = new ProjectileBase.ProjectileInfo(attackDirection, requireAmmoData.ProjectileSpeed, rangeWeaponData.AttackRange / requireAmmoData.ProjectileSpeed,
                requireAmmoData.ColliderRadius, rangeWeaponData.Damage, rangeWeaponData.DamageType, rangeWeaponData.KnockbackPower);
            projectileBase.InitializeProjectile(projectileInfo, projectileFirePosition.position);

            projectileBase.Fire();
        }

        private void DropItem(Item dropItem)
        {
            var dropItemObject = ItemSpawnManager.Instance.SpawnItemObjectAt(dropItem, transform.position + new Vector3(0, 1.5f, 0));
            if (dropItemObject == null)
            {
                return;
            }

            // 무작위 방향으로 던짐
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            var itemObjectRigidbody = dropItemObject.GetComponent<Rigidbody>();
            if (itemObjectRigidbody != null)
            {
                float force = 6f;
                itemObjectRigidbody.AddForce(new Vector3(randomDirection.x, 0.5f, randomDirection.y).normalized * force, ForceMode.Impulse);
            }

            UIManager.Instance.UpdateQuickSlots();
        }

        private void ConsumeItem(Item consumableItem)
        {
            if (consumableItem.IsStackEmpty)
            {
                Debug.LogWarning($"사용하려고 하는 아이템이 비어 있습니다");
                return;
            }

            var consumableData = consumableItem.Data as ConsumableData;
            switch (consumableData)
            {
                case HealingKitData healingKit:
                    CurrentHealth += healingKit.HealthRestoreAmount;
                    CurrentShield += healingKit.ShieldRestoreAmount;
                    break;

                default:
                    Debug.LogWarning($"관리되지 않고 있는 {nameof(ItemData)}입니다, {consumableItem.Data.GetType().Name}");
                    return;
            }

            // TODO ItemContainer에서 스택 개수 줄이기
            if (ItemContainer.HasItem(consumableData.ItemName) == false)
            {
                Debug.LogWarning($"사용하려고 하는 아이템이 {nameof(ItemContainer)}에 없습니다, {nameof(consumableData.ItemName)}: {consumableData.ItemName}");
                return;
            }

            for (int slot = 0; slot < ItemContainer.Count; slot++)
            {
                var targetItem = ItemContainer.GetItemAt(slot);
                if (targetItem == consumableItem)
                {
                    ItemContainer.SubtrackItemAtSlot(slot);
                }
            }
        }

        // TODO: 아이템 사용 시전 중단 기능
        private IEnumerator TryConsumeItem(Item consumableItem)
        {
            if (consumableItem.IsStackEmpty)
            {
                Debug.LogWarning($"빈 아이템을 사용하려고 하고 있습니다");
                yield return null;
            }

            if ((consumableItem.Data is ConsumableData) == false)
            {
                Debug.LogError($"사용하려는 아이템이 {nameof(ConsumableData)}를 상속하지 않습니다, {nameof(consumableItem.Data.ItemName)}: {consumableItem.Data.ItemName}");
                yield return null;
            }
            var consumableData = consumableItem.Data as ConsumableData;

            // 아이템 사용 시전
            float startTime = Time.time;
            float endTime = startTime + consumableData.TimeToUse;
            float progress = 0f;
            while (Time.time <= endTime)
            {
                // TODO: 플레이어 이동속도 느려지게, UI와 동기화
                progress = Mathf.InverseLerp(startTime, endTime, Time.time);
                Debug.Log($"RemainTime: {endTime - Time.time}, Progress: {progress}");
                yield return null;
            }

            // 아이템 사용
            ConsumeItem(consumableItem);
        }
    }
}

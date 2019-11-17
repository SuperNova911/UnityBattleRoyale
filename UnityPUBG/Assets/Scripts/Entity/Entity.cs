using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int maximumHealth;
        [SerializeField] private float currentHealth;

        [Header("Movement")]
        [SerializeField] [Range(0f, 20f)] protected float movementSpeed = 5f;
        [SerializeField] [Range(0f, 1f)] protected float rotationSpeed = 0.2f;
        protected Vector3 movementDirection;
        protected Vector3 facingDirection;

        protected ItemContainer itemContainer;
        private Rigidbody entityRigidbody;

        public EventHandler<float> OnCurrentHealthUpdate;

        public int MaximumHealth
        {
            get { return maximumHealth; }
            protected set { maximumHealth = value; }
        }
        public float CurrentHealth
        {
            get { return currentHealth; }
            protected set
            {
                currentHealth = value;
                currentHealth = Mathf.Clamp(currentHealth, 0f, MaximumHealth);

                IsDead = currentHealth <= 0;
                OnCurrentHealthUpdate?.Invoke(this, currentHealth);
            }
        }
        public bool IsDead { get; private set; }
        public ItemContainer ItemContainer => itemContainer;

        #region 유니티 메시지
        protected virtual void Awake()
        {
            itemContainer = new ItemContainer(6);
            entityRigidbody = GetComponent<Rigidbody>();

            CurrentHealth = MaximumHealth;

            EntityManager.Instance.RegisterEntity(this);
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {
            if (IsDead == false)
            {
                MoveEntity();
                RotateEntity();
            }
        }

        protected virtual void OnDestory()
        {
            EntityManager.Instance.UnRegisterEntity(this);
        }
        #endregion

        /// <summary>
        /// 특정 슬롯에 있는 아이템을 입력으로 받은 스택만큼 ItemObject를 드랍
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="dropStack"></param>
        public void DropItemsAtSlot(int slot, int dropStack)
        {
            if (itemContainer.Capacity <= slot)
            {
                Debug.LogWarning($"{nameof(itemContainer)}의 {nameof(itemContainer.Capacity)} 범위를 벗어나는 {nameof(slot)}입니다");
                return;
            }

            var dropItem = itemContainer.SubtrackItemsAtSlot(slot, dropStack);
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

        /// <summary>
        /// movementDirection 방향으로 Entity를 이동
        /// </summary>
        private void MoveEntity()
        {
            if (movementDirection == Vector3.zero)
            {
                return;
            }

            Vector3 direction = movementDirection * movementSpeed * Time.fixedDeltaTime;
            entityRigidbody.MovePosition(transform.position + direction);
        }

        /// <summary>
        /// movementDirection 방향을 Entity가 바라보도록 함
        /// </summary>
        private void RotateEntity()
        {
            if (movementDirection == Vector3.zero)
            {
                return;
            }

            facingDirection = movementDirection;

            var targetRotation = Quaternion.LookRotation(facingDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
        }
    }
}
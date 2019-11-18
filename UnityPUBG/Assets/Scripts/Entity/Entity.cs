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

        #region 유니티 메시지
        protected virtual void Awake()
        {
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
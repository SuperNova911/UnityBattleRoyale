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
        [SerializeField, Range(10, 100)] private int maximumHealth = 100;
        [SerializeField, Range(0f, 100f)] private float currentHealth;

        [Header("Movement")]
        [SerializeField] [Range(0f, 20f)] private float movementSpeed = 5f;
        [SerializeField] [Range(0f, 1f)] private float rotationSpeed = 0.2f;

        private Rigidbody entityRigidbody;

        public event EventHandler<float> OnCurrentHealthUpdate;

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
                if (IsDead)
                {
                    Destroy(gameObject);
                }
            }
        }
        public bool IsDead { get; private set; }
        public Vector2 MovementDirection { get; protected set; }
        public Vector3 FacingDirection { get; private set; }

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

        protected virtual void OnDestroy()
        {
            EntityManager.Instance?.UnRegisterEntity(this);
        }
        #endregion

        /// <summary>
        /// movementDirection 방향으로 Entity를 이동
        /// </summary>
        private void MoveEntity()
        {
            if (MovementDirection == Vector2.zero)
            {
                return;
            }

            Vector3 direction = new Vector3(MovementDirection.x, 0, MovementDirection.y) * movementSpeed * Time.fixedDeltaTime;
            entityRigidbody.MovePosition(transform.position + direction);
        }

        /// <summary>
        /// movementDirection 방향을 Entity가 바라보도록 함
        /// </summary>
        private void RotateEntity()
        {
            if (MovementDirection == Vector2.zero)
            {
                return;
            }

            FacingDirection = new Vector3(MovementDirection.x, 0, MovementDirection.y);

            var targetRotation = Quaternion.LookRotation(FacingDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
        }
    }
}
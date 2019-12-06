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
        [SerializeField] [Range(1f, 50f)] private float dropSpeed = 8f;
        [SerializeField] [Range(1f, 100f)] private float flySpeed = 20f;

        private Rigidbody entityRigidbody;
        private bool isDroping = false;
        private Vector3 currentVelocity;

        public event EventHandler<float> OnCurrentHealthUpdate;
        public event EventHandler OnDie;

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
                float previousHealth = currentHealth;
                currentHealth = value;
                currentHealth = Mathf.Clamp(currentHealth, 0f, MaximumHealth);

                float changeAmount = previousHealth - currentHealth;
                if (changeAmount > 0)
                {
                    UI.FloatingTextDrawer.Instance.DrawDamageText(transform, changeAmount);
                }
                OnCurrentHealthUpdate?.Invoke(this, changeAmount);

                IsDead = currentHealth <= 0;
                if (IsDead)
                {
                    OnDie?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public bool IsDead { get; private set; }
        public bool IsDroping
        { 
            get { return isDroping; }
            protected set
            {
                isDroping = value;
                entityRigidbody.useGravity = isDroping ? false : true;
            }
        }
        public float SpeedMultiplier { get; set; }
        public Vector2 MovementDirection { get; protected set; }
        public Vector2 RotateDirection { get; protected set; }

        #region 유니티 메시지
        protected virtual void Awake()
        {
            entityRigidbody = GetComponent<Rigidbody>();

            CurrentHealth = MaximumHealth;
            SpeedMultiplier = 1f;

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
            if(MovementDirection == Vector2.zero && IsDroping == false)
            {
                return;
            }

            if (IsDroping)
            {
                var targetVelocity = new Vector3(MovementDirection.x * flySpeed, -1 * dropSpeed, MovementDirection.y * flySpeed);
                entityRigidbody.velocity = Vector3.SmoothDamp(entityRigidbody.velocity, targetVelocity, ref currentVelocity, 0.5f);
            }
            else
            {
                var direction = new Vector3(MovementDirection.x, 0, MovementDirection.y) * movementSpeed * SpeedMultiplier * Time.fixedDeltaTime;
                entityRigidbody.MovePosition(transform.position + direction);
            }
        }

        /// <summary>
        /// movementDirection 방향을 Entity가 바라보도록 함
        /// </summary>
        private void RotateEntity()
        {
            if (RotateDirection == Vector2.zero)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(new Vector3(RotateDirection.x, 0, RotateDirection.y));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
        }
    }
}
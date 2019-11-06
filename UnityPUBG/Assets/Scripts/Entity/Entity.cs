using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] protected int maximumHealth;
        [SerializeField] protected float currentHealth;

        [Header("Movement")]
        [SerializeField] [Range(0f, 20f)] protected float movementSpeed = 5f;
        [SerializeField] [Range(0f, 1f)] protected float rotationSpeed = 0.2f;
        protected Vector3 movementDirection;
        protected Vector3 facingDirection;

        protected ItemContainer itemContainer;
        private Rigidbody entityRigidbody;

        public ItemContainer ItemContainer => itemContainer;

        #region 유니티 메시지
        protected virtual void Awake()
        {
            currentHealth = maximumHealth;
            entityRigidbody = GetComponent<Rigidbody>();

            itemContainer = new ItemContainer(6);
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {
            MoveEntity();
            RotateEntity();
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
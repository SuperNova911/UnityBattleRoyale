using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        #region 필드
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
        #endregion

        #region 유니티 메시지
        protected virtual void Awake()
        {
            currentHealth = maximumHealth;
            entityRigidbody = GetComponent<Rigidbody>();
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

        #region 메서드
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
        #endregion
    }
}
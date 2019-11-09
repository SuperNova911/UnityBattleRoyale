using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;

namespace UnityPUBG.Scripts.Logic
{
    public class EntityManager : Singleton<EntityManager>
    {
        public List<Entity> Entities { get; private set; }
        public List<IDamageable> Damageables { get; private set; }

        private void Awake()
        {
            Entities = new List<Entity>();
            Damageables = new List<IDamageable>();
        }

        public void RegisterEntity(Entity entity)
        {
            if (entity == null)
            {
                Debug.LogError($"등록하려는 {nameof(entity)}가 null입니다");
                return;
            }

            Entities.Add(entity);
            if (entity is IDamageable)
            {
                Damageables.Add((IDamageable)entity);
            }
        }

        public void UnRegisterEntity(Entity entity)
        {
            if (entity == null)
            {
                Debug.LogError($"제거하려는 {nameof(entity)}가 null입니다");
                return;
            }

            Entities.Remove(entity);
            if (entity is IDamageable)
            {
                Damageables.Remove((IDamageable)entity);
            }
        }
    }
}

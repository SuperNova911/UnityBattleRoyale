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
        private List<Entity> entities = new List<Entity>();
        private List<IDamageable> damageables = new List<IDamageable>();

        public List<Entity> Entities => entities;
        public List<IDamageable> Damageables => damageables;

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

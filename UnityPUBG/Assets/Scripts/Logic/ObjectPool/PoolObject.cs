using UnityEngine;

namespace UnityPUBG.Scripts.Logic
{
    public abstract class PoolObject : MonoBehaviour
    {
        public int ObjectPoolKey { get; set; }

        public abstract void OnObjectReuse();
        public abstract void OnObjectSaveToPool();
    }
}

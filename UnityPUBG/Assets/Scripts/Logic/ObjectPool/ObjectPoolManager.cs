using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private readonly Dictionary<int, Queue<PoolObject>> objectPools = new Dictionary<int, Queue<PoolObject>>();
        private readonly Dictionary<int, Queue<PoolObject>> uiObjectPools = new Dictionary<int, Queue<PoolObject>>();
        private readonly Dictionary<int, Transform> parentDictionary = new Dictionary<int, Transform>();

        public void InitializeObjectPool(GameObject prefab, int initialPoolSize)
        {
            int key = prefab.GetInstanceID();
            if (objectPools.ContainsKey(key))
            {
                Debug.LogWarning($"{prefab.name}의 {nameof(objectPools)}이 이미 존재합니다");
                return;
            }

            var poolHolder = new GameObject($"{prefab.name} pool");
            poolHolder.transform.SetParent(transform);
            parentDictionary.Add(key, poolHolder.transform);

            objectPools.Add(key, new Queue<PoolObject>());
            for (int i = 0; i < initialPoolSize; i++)
            {
                var newPoolObject = Instantiate(prefab, poolHolder.transform).GetComponent<PoolObject>();
                newPoolObject.gameObject.SetActive(false);
                newPoolObject.ObjectPoolKey = key;
                objectPools[key].Enqueue(newPoolObject);
            }
        }

        public void InitializeUIObjectPool(GameObject prefab, RectTransform parentRect, int initialPoolSize)
        {
            int key = prefab.GetInstanceID();
            if (uiObjectPools.ContainsKey(key))
            {
                Debug.LogWarning($"{prefab.name}의 {nameof(uiObjectPools)}이 이미 존재합니다");
                return;
            }

            var poolHolder = new GameObject($"{prefab.name} pool").AddComponent<RectTransform>();
            poolHolder.transform.SetParent(parentRect);
            poolHolder.anchorMin = Vector2.zero;
            poolHolder.anchorMax = Vector2.one;
            poolHolder.offsetMin = Vector2.zero;
            poolHolder.offsetMax = Vector2.zero;

            parentDictionary.Add(key, poolHolder.transform);

            uiObjectPools.Add(key, new Queue<PoolObject>());
            for (int i = 0; i < initialPoolSize; i++)
            {
                var newPoolObject = Instantiate(prefab, poolHolder.transform).GetComponent<PoolObject>();
                newPoolObject.gameObject.SetActive(false);
                newPoolObject.ObjectPoolKey = key;
                uiObjectPools[key].Enqueue(newPoolObject);
            }
        }

        public PoolObject ReuseObject(GameObject prefab)
        {
            int key = prefab.GetInstanceID();
            if (objectPools.ContainsKey(key) == false)
            {
                Debug.LogError($"{prefab.name}의 {nameof(objectPools)}이 생성되기 전에 접근하려고 하고 있습니다");
                return null;
            }

            var objectQueue = objectPools[key];
            PoolObject poolObject;
            if (objectQueue.Count > 0)
            {
                poolObject = objectQueue.Dequeue();
                poolObject.gameObject.SetActive(true);
            }
            else
            {
                poolObject = Instantiate(prefab, parentDictionary[key]).GetComponent<PoolObject>();
                poolObject.ObjectPoolKey = key;
            }
            poolObject.OnObjectReuse();
            return poolObject;
        }

        public PoolObject ReuseUIObject(GameObject prefab)
        {
            int key = prefab.GetInstanceID();
            if (uiObjectPools.ContainsKey(key) == false)
            {
                Debug.LogError($"{prefab.name}의 {nameof(uiObjectPools)}이 생성되기 전에 접근하려고 하고 있습니다");
                return null;
            }

            var objectQueue = uiObjectPools[key];
            PoolObject poolObject;
            if (objectQueue.Count > 0)
            {
                poolObject = objectQueue.Dequeue();
                poolObject.gameObject.SetActive(true);
            }
            else
            {
                poolObject = Instantiate(prefab, parentDictionary[key]).GetComponent<PoolObject>();
                poolObject.ObjectPoolKey = key;
            }
            poolObject.OnObjectReuse();
            return poolObject;
        }

        public void SaveObjectToPool(PoolObject poolObject)
        {
            int key = poolObject.ObjectPoolKey;
            if (objectPools.ContainsKey(key) == false)
            {
                Debug.LogWarning($"{nameof(objectPools)}에서 관리되지 않고 있는 {nameof(poolObject)}를 풀링하려고 하고 있습니다");
                Destroy(poolObject.gameObject);
                return;
            }

            poolObject.OnObjectSaveToPool();
            poolObject.gameObject.SetActive(false);

            var objectQueue = objectPools[key];
            objectQueue.Enqueue(poolObject);
        }

        public void SaveUIObjectToPool(PoolObject poolObject)
        {
            int key = poolObject.ObjectPoolKey;
            if (uiObjectPools.ContainsKey(key) == false)
            {
                Debug.LogWarning($"{nameof(uiObjectPools)}에서 관리되지 않고 있는 {nameof(poolObject)}를 풀링하려고 하고 있습니다");
                Destroy(poolObject.gameObject);
                return;
            }

            poolObject.OnObjectSaveToPool();
            poolObject.gameObject.SetActive(false);

            var objectQueue = uiObjectPools[key];
            objectQueue.Enqueue(poolObject);
        }
    }
}

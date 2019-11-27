using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;
                }

                return instance;
            }
        }
    }
}
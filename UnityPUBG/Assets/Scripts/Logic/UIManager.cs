using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Entities;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 창 닫기
        /// </summary>
        /// <param name="window">닫을 창</param>
        public void CloseWindow(GameObject window)
        {
            window.SetActive(false);
        }

        /// <summary>
        /// 창 열기
        /// </summary>
        /// <param name="window">열 창</param>
        public void OpenWindow(GameObject window)
        {
            window.SetActive(true);
        }
    }
}
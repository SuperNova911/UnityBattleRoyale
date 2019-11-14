using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 이 클라이언트의 플레이어
        /// </summary>
        private Entities.Player myplayer;

        public Entities.Player myPlayer
        {
            get
            {
                return myplayer;
            }
            set
            {
                if (myplayer == null)
                    myplayer = value;
            }
        }

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
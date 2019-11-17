using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 링이 줄어드는 등의
        /// 공지사항을 표기하는 텍스트
        /// </summary>
        [SerializeField]
        private Text noticeText;

        private void Awake()
        {
            RingSystem.Instance.OnRoundStart += DisplayRoundStartMessage;
            RingSystem.Instance.OnRingCloseStart += DisplayRingCloseStartMessage;
        }

        /// <summary>
        /// 창 닫기
        /// </summary>
        /// <param name="window">닫을 창</param>
        public void CloseWindow(GameObject window)
        {
            DisableChild(window);
            window.SetActive(false);
        }

        /// <summary>
        /// 창 열기
        /// </summary>
        /// <param name="window">열 창</param>
        public void OpenWindow(GameObject window)
        {
            EnableChild(window);
            window.SetActive(true);
        }

        /// <summary>
        /// 공지사항을 표기함
        /// </summary>
        /// <param name="notice">표기할 공지사항</param>
        public void NoticeTextUpdate(string notice)
        {
            StartCoroutine(NoticeTextUpdater(notice));
        }

        /// <summary>
        /// 인자로 받은 게임 오브젝트의 자식 오브젝트를 disable함
        /// </summary>
        /// <param name="gameObject"></param>
        private void DisableChild(GameObject gameObject)
        {
            int childCount = gameObject.transform.childCount;

            for(int i = 0; i<childCount; i++)
            {
                DisableChild(gameObject.transform.GetChild(i).gameObject);
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 인자로 받은 게임 오브젝트의 자식 오브젝트를 Enable함
        /// </summary>
        /// <param name="gameObject"></param>
        private void EnableChild(GameObject gameObject)
        {
            int childCount = gameObject.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                EnableChild(gameObject.transform.GetChild(i).gameObject);
            }

            gameObject.SetActive(true);
        }

        private void DisplayRoundStartMessage(object sender, RingSystem.RoundData roundData)
        {
            TimeSpan waitPeriod = TimeSpan.FromSeconds(roundData.WaitPeriod);
            string timeString = waitPeriod.Minutes > 0 ? $"{waitPeriod.Minutes}분 " : string.Empty;
            timeString += waitPeriod.Seconds > 0 ? $"{waitPeriod.Seconds}초" : string.Empty;

            string noticeMessage = $"라운드 {roundData.RoundNumber}\n{timeString} 후에 링이 줄어듭니다";
            NoticeTextUpdate(noticeMessage);
        }

        private void DisplayRingCloseStartMessage(object sender, RingSystem.RoundData roundData)
        {
            TimeSpan timeToClose = TimeSpan.FromSeconds(roundData.TimeToClose);
            string timeString = timeToClose.Minutes > 0 ? $"{timeToClose.Minutes}분 {timeToClose.Seconds}초" : $"{timeToClose.Seconds}초";
            
            string noticeMessage = $"링 축소중\n{timeString} 안에 링이 줄어듭니다";
            NoticeTextUpdate(noticeMessage);
        }

        /// <summary>
        /// 3초동안 공지사항을 표기하고 끔
        /// </summary>
        /// <param name="notice">표기할 공지사항</param>
        /// <returns></returns>
        private IEnumerator NoticeTextUpdater(string notice)
        {
            noticeText.text = notice;

            yield return new WaitForSeconds(3f);

            noticeText.text = string.Empty;

            yield break;
        }
    }
}
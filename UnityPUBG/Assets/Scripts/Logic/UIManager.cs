using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPUBG.Scripts.Entities;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 링이 줄어드는 등의
        /// 공지사항을 표기하는 텍스트
        /// </summary>
        [SerializeField]
        private UnityEngine.UI.Text noticeText;

        /// <summary>
        /// 플레이어 상태 바
        /// </summary>
        [SerializeField]
        private GameObject playerStateBar;

        /// <summary>
        /// 미니맵과 조이스틱 들
        /// </summary>
        [SerializeField]
        private GameObject nomalUIElements;

        /// <summary>
        /// 창 열고 닫기
        /// </summary>
        /// <param name="window">열고 닫을 창</param>
        public void ControlWindow(GameObject window)
        {
            if(window.activeSelf)
            {
                DisableChild(window);
                playerStateBar.SetActive(true);
                nomalUIElements.SetActive(true);
                window.SetActive(false);
            }
            else
            {
                EnableChild(window);
                playerStateBar.SetActive(false);
                nomalUIElements.SetActive(false);
                window.SetActive(true);
            }
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

        /// <summary>
        /// 공지사항을 표기함
        /// </summary>
        /// <param name="notice">표기할 공지사항</param>
        public void NoticeTextUpdate(string notice)
        {
            StartCoroutine(NoticeTextUpdater(notice));
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
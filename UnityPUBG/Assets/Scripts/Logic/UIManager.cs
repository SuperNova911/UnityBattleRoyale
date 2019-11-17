using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Entities;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        public Slider playerHealthSlider;
        public Slider playerShieldSlider;
        public TMP_Text playerHealthText;
        public TMP_Text playerShieldText;

        public TMP_Text roundMessage;
        public TMP_Text roundSubMessage;
        public TMP_Text killMessage;

        /// <summary>
        /// 링이 줄어드는 등의
        /// 공지사항을 표기하는 텍스트
        /// </summary>
        [SerializeField]
        private Text noticeText;

        private void Awake()
        {
            EntityManager.Instance.OnMyPlayerSpawn += InitializePlayerUIElements;
            RingSystem.Instance.OnRoundStart += DisplayRoundStartMessage;
            RingSystem.Instance.OnRingCloseStart += DisplayRingCloseStartMessage;
        }

        /// <summary>
        /// 미니맵과 조이스틱 들
        /// </summary>
        [SerializeField]
        private GameObject normalUIElements;

        /// <summary>
        /// 창 열고 닫기
        /// </summary>
        /// <param name="window">열고 닫을 창</param>
        public void ControlWindow(GameObject window)
        {
            if(window.activeSelf)
            {
                DisableChild(window);
                normalUIElements.SetActive(true);
                window.SetActive(false);
            }
            else
            {
                EnableChild(window);
                normalUIElements.SetActive(false);
                window.SetActive(true);
            }
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

        private void InitializePlayerUIElements(object sender, Player myPlayer)
        {
            // UI 초기값 설정
            playerHealthSlider.maxValue = myPlayer.MaximumHealth;
            playerHealthSlider.value = myPlayer.CurrentHealth;
            playerHealthText.text = Mathf.RoundToInt(myPlayer.CurrentHealth).ToString();

            playerShieldSlider.maxValue = myPlayer.MaximumShield;
            playerShieldSlider.value = myPlayer.CurrentShield;
            playerShieldText.text = Mathf.RoundToInt(myPlayer.CurrentShield).ToString();

            // 이벤트 구독
            myPlayer.OnCurrentHealthUpdate += UpdatePlayerHealthSlider;
            myPlayer.OnCurrentShieldUpdate += UpdatePlayerShieldSlider;
        }

        private void UpdatePlayerHealthSlider(object sender, float value)
        {
            playerHealthSlider.value = value;
            playerHealthText.text = Mathf.RoundToInt(value).ToString();
        }

        private void UpdatePlayerShieldSlider(object sender, float value)
        {
            playerShieldSlider.value = value;
            playerShieldText.text = Mathf.RoundToInt(value).ToString();
        }

        private void DisplayRoundStartMessage(object sender, RingSystem.RoundData roundData)
        {
            TimeSpan waitPeriod = TimeSpan.FromSeconds(roundData.WaitPeriod);
            string timeString = waitPeriod.Minutes > 0 ? $"{waitPeriod.Minutes}분" : string.Empty;
            timeString += waitPeriod.Seconds > 0 ? $" {waitPeriod.Seconds}초" : string.Empty;

            string message = $"라운드 {roundData.RoundNumber}";
            string subMessage = $"<color=yellow>{timeString}</color> 후에 링이 줄어듭니다";
            StartCoroutine(DisplayRoundMessage(message, subMessage, 3f));
        }

        private void DisplayRingCloseStartMessage(object sender, RingSystem.RoundData roundData)
        {
            TimeSpan timeToClose = TimeSpan.FromSeconds(roundData.TimeToClose);
            string timeString = timeToClose.Minutes > 0 ? $"{timeToClose.Minutes}분 {timeToClose.Seconds}초" : $"{timeToClose.Seconds}초";
            
            string message = $"링 축소중!";
            string subMessage = $"<color=yellow>{timeString}</color> 안에 링이 줄어듭니다";
            StartCoroutine(DisplayRoundMessage(message, subMessage, 3f));
        }

        private IEnumerator DisplayRoundMessage(string message, string subMessage, float visibleDuration)
        {
            roundMessage.text = message;
            roundSubMessage.text = subMessage;
            yield return new WaitForSeconds(visibleDuration);

            Color previousColor = roundMessage.color;
            float startTime = Time.time;
            float endTime = startTime + 0.5f;
            float progress = 1f;
            while (progress > 0)
            {
                Color newColor = roundMessage.color;
                newColor.a = progress;
                roundMessage.color = newColor;
                roundSubMessage.color = newColor;

                progress = Mathf.InverseLerp(endTime, startTime, Time.time);
                yield return null;
            }

            roundMessage.color = previousColor;
            roundSubMessage.color = previousColor;
            roundMessage.text = string.Empty;
            roundSubMessage.text = string.Empty;
            yield return null;
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
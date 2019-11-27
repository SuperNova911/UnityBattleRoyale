using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.UI;
using UnityEngine.U2D;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        public FloatingJoystick playerMovementJoystick;
        public FloatingJoystick playerAttackJoystick;
        [Space]
        public Slider playerHealthSlider;
        public Slider playerShieldSlider;
        public TMP_Text playerHealthText;
        public TMP_Text playerShieldText;
        [Space]
        public Slider roundProgressSlider;
        public RectTransform roundTextArea;
        public TMP_Text roundStatusText;
        public TMP_Text roundTimerText;
        [Space]
        public TMP_Text roundMessage;
        public TMP_Text roundSubMessage;
        public TMP_Text killMessage;
        [Space]
        /// <summary>
        /// 인벤토리 창
        /// </summary>
        [SerializeField]
        private GameObject inventoryWindow;
        /// <summary>
        /// 퀵슬롯
        /// </summary>
        [SerializeField]
        private GameObject quickSlot;
        [SerializeField]
        private SpriteAtlas iconAtlas;
        [Space]
        /// <summary>
        /// 일반 게임플레이시 보이는 UI 요소
        /// </summary>
        [SerializeField]
        private GameObject normalUIElements;

        /// <summary>
        /// 아이콘 스프라이트 아틀라스
        /// </summary>
        public SpriteAtlas IconAtlas => iconAtlas;

        private void Awake()
        {
            EntityManager.Instance.OnMyPlayerSpawn += InitializePlayerUIElements;
            EntityManager.Instance.OnMyPlayerDestory += DisablePlayerUIElements;

            RingSystem.Instance.OnRoundStart += RingSystem_OnRoundStart;
            RingSystem.Instance.OnRingCloseStart += RingSystem_OnRingCloseStart;
        }

        /// <summary>
        /// 창 열고 닫기
        /// </summary>
        /// <param name="window">열고 닫을 창</param>
        public void ControlWindow(GameObject window)
        {
            if (window.activeSelf)
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

        //인벤토리 슬롯을 업데이트 함
        public void UpdateInventorySlots()
        {
            if (!inventoryWindow.activeSelf)
            {
                return;
            }

            ItemSlot[] itemSlots = inventoryWindow.transform.GetComponentsInChildren<ItemSlot>();

            int length = itemSlots.Length;
            for (int i = 0; i < length; i++)
            {
                itemSlots[i].UpdateSlotObject();
            }
        }

        /// <summary>
        /// 퀵슬롯을 업데이트 함
        /// </summary>
        public void UpdateQuickSlots()
        {
            QuickItemSlot[] quickItemSlots = quickSlot.transform.GetComponentsInChildren<QuickItemSlot>();

            int length = quickItemSlots.Length;
            for (int i = 0; i < length; i++)
            {
                quickItemSlots[i].UpdateQuickItemSlot();
            }
        }

        /// <summary>
        /// 인자로 받은 게임 오브젝트의 자식 오브젝트를 disable함
        /// </summary>
        /// <param name="gameObject"></param>
        private void DisableChild(GameObject gameObject)
        {
            int childCount = gameObject.transform.childCount;

            for (int i = 0; i < childCount; i++)
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

        private void InitializePlayerUIElements(object sender, EventArgs e)
        {
            var targetPlayer = EntityManager.Instance.MyPlayer;

            playerMovementJoystick.OnJoystickDrag += PlayerMovementJoystick_OnJoystickDrag;
            playerMovementJoystick.OnJoystickRelease += PlayerMovementJoystick_OnJoystickRelease;
            playerAttackJoystick.OnJoystickRelease += PlayerAttackJoystick_OnJoystickRelease;

            // UI 초기값 설정
            playerHealthSlider.maxValue = targetPlayer.MaximumHealth;
            playerHealthSlider.value = targetPlayer.CurrentHealth;
            playerHealthText.text = Mathf.RoundToInt(targetPlayer.CurrentHealth).ToString();

            playerShieldSlider.maxValue = targetPlayer.MaximumShield;
            playerShieldSlider.value = targetPlayer.CurrentShield;
            playerShieldText.text = Mathf.RoundToInt(targetPlayer.CurrentShield).ToString();

            // 이벤트 구독
            targetPlayer.OnCurrentHealthUpdate += UpdatePlayerHealthSlider;
            targetPlayer.OnCurrentShieldUpdate += UpdatePlayerShieldSlider;
        }

        private void DisablePlayerUIElements(object sender, EventArgs e)
        {
            playerMovementJoystick.OnJoystickDrag -= PlayerMovementJoystick_OnJoystickDrag;
            playerMovementJoystick.OnJoystickRelease -= PlayerMovementJoystick_OnJoystickRelease;
            playerAttackJoystick.OnJoystickRelease -= PlayerAttackJoystick_OnJoystickRelease;
        }

        private void PlayerMovementJoystick_OnJoystickDrag(object sender, Vector2 direction)
        {
            EntityManager.Instance.MyPlayer.MoveTo(direction);
        }

        private void PlayerMovementJoystick_OnJoystickRelease(object sender, Vector2 e)
        {
            EntityManager.Instance.MyPlayer.MoveTo(Vector2.zero);
        }

        private void PlayerAttackJoystick_OnJoystickRelease(object sender, Vector2 direction)
        {
            EntityManager.Instance.MyPlayer.AttackTo(direction);
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

        private void RingSystem_OnRoundStart(object sender, RingSystem.RoundData roundData)
        {
            TimeSpan waitPeriod = TimeSpan.FromSeconds(roundData.WaitPeriod);

            // Round Status Text
            roundStatusText.text = $"라운드 {roundData.RoundNumber} - 남은 시간";
            StartCoroutine(PlayRoundTimer(roundData.WaitPeriod, false));

            // Round Start Message
            string timeString = waitPeriod.Minutes > 0 ? $"{waitPeriod.Minutes}분" : string.Empty;
            timeString += waitPeriod.Seconds > 0 ? $" {waitPeriod.Seconds}초" : string.Empty;

            string message = $"라운드 {roundData.RoundNumber}";
            string subMessage = $"<color=yellow>{timeString}</color> 후에 링이 줄어듭니다";
            StartCoroutine(PlayRoundMessage(message, subMessage, 3f));
        }

        private void RingSystem_OnRingCloseStart(object sender, RingSystem.RoundData roundData)
        {
            TimeSpan timeToClose = TimeSpan.FromSeconds(roundData.TimeToClose);

            // Round Status Text
            roundStatusText.text = $"라운드 {roundData.RoundNumber} - 마감중";
            StartCoroutine(PlayRoundTimer(roundData.TimeToClose, true));

            // Ring Close Message;
            string timeString = timeToClose.Minutes > 0 ? $"{timeToClose.Minutes}분 {timeToClose.Seconds}초" : $"{timeToClose.Seconds}초";

            string message = $"링 축소중!";
            string subMessage = $"<color=yellow>{timeString}</color> 안에 링이 줄어듭니다";
            StartCoroutine(PlayRoundMessage(message, subMessage, 3f));
        }

        private IEnumerator PlayRoundTimer(float period, bool showProgressSlider)
        {
            roundProgressSlider.gameObject.SetActive(showProgressSlider);

            // 슬라이더의 유무에 따라 텍스트 위치를 옮김
            var sliderRectTransform = roundProgressSlider.GetComponent<RectTransform>();
            float textAreaAnchorHeight = roundTextArea.anchorMax.y - roundTextArea.anchorMin.y;
            if (showProgressSlider)
            {
                roundTextArea.anchorMin = new Vector2(roundTextArea.anchorMin.x, sliderRectTransform.anchorMin.y - textAreaAnchorHeight);
                roundTextArea.anchorMax = new Vector2(roundTextArea.anchorMax.x, sliderRectTransform.anchorMin.y);
            }
            else
            {
                roundTextArea.anchorMin = new Vector2(roundTextArea.anchorMin.x, sliderRectTransform.anchorMax.y - textAreaAnchorHeight);
                roundTextArea.anchorMax = new Vector2(roundTextArea.anchorMax.x, sliderRectTransform.anchorMax.y);
            }

            // UI 업데이트
            float updatePeriod = showProgressSlider ? 0.05f : 0.2f;
            float startTime = Time.time;
            float endTime = startTime + period - 0.5f;  // margin
            float progress = 0f;
            while (progress < 1f)
            {
                progress = Mathf.InverseLerp(startTime, endTime, Time.time);
                if (showProgressSlider)
                {
                    roundProgressSlider.value = progress;
                }

                var remainTime = TimeSpan.FromSeconds(endTime - Time.time);
                roundTimerText.text = string.Format("{0:m\\:ss}", remainTime);

                yield return new WaitForSeconds(updatePeriod);
            }

        }

        private IEnumerator PlayRoundMessage(string message, string subMessage, float visibleDuration)
        {
            // NOTE: 모바일 성능 문제로 천천히 드러내는 효과는 비활성화함

            // 투명하게 시작
            //Color previousColor = roundMessage.color;
            //Color transparentColor = roundMessage.color;
            //transparentColor.a = 0f;
            //roundMessage.color = transparentColor;
            //roundSubMessage.color = transparentColor;

            roundMessage.text = message;
            roundSubMessage.text = subMessage;
            roundMessage.gameObject.SetActive(true);
            roundSubMessage.gameObject.SetActive(true);

            // 천천히 보이기
            //float startTime = Time.time;
            //float endTime = startTime + 0.5f;
            //float progress = 0f;
            //while (progress < 1f)
            //{
            //    Color newColor = roundMessage.color;
            //    newColor.a = progress;
            //    roundMessage.color = newColor;
            //    roundSubMessage.color = newColor;

            //    progress = Mathf.InverseLerp(startTime, endTime, Time.time);
            //    yield return new WaitForSeconds(0.02f);
            //}

            yield return new WaitForSeconds(visibleDuration);

            // 천천히 사라지기
            //startTime = Time.time;
            //endTime = startTime + 0.5f;
            //progress = 0f;
            //while (progress < 1f)
            //{
            //    Color newColor = roundMessage.color;
            //    newColor.a = 1f - progress;
            //    roundMessage.color = newColor;
            //    roundSubMessage.color = newColor;

            //    progress = Mathf.InverseLerp(startTime, endTime, Time.time);
            //    yield return new WaitForSeconds(0.02f);
            //}

            //roundMessage.color = previousColor;
            //roundSubMessage.color = previousColor;
            roundMessage.gameObject.SetActive(false);
            roundSubMessage.gameObject.SetActive(false);
            yield return null;
        }
    }
}
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
        public FloatingJoystick playerMovementJoystick;
        public FloatingJoystick playerAttackJoystick;
        [Space]
        public Slider playerHealthSlider;
        public Slider playerShieldSlider;
        public TMP_Text playerHealthText;
        public TMP_Text playerShieldText;
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

        private void Awake()
        {
            playerAttackJoystick.OnJoystickUp += PlayerAttackJoystick_OnJoystickUp;

            EntityManager.Instance.OnMyPlayerSpawn += InitializePlayerUIElements;
            RingSystem.Instance.OnRoundStart += DisplayRoundStartMessage;
            RingSystem.Instance.OnRingCloseStart += DisplayRingCloseStartMessage;
        }

        private void PlayerAttackJoystick_OnJoystickUp(object sender, Vector2 direction)
        {
            EntityManager.Instance.MyPlayer.AttackTo(direction);
        }

        private void FixedUpdate()
        {
            EntityManager.Instance.MyPlayer.MoveTo(playerMovementJoystick.Direction);

            // 디버깅을 편하게 하기 위해 키보드 컨트롤
            var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (direction != Vector2.zero)
            {
                EntityManager.Instance.MyPlayer.MoveTo(direction.normalized);
            }
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

        //인벤토리 슬롯을 업데이트 함
        public void UpdateInventorySlots()
        {
            if (!inventoryWindow.activeSelf)
                return;

            UI.ItemSlot[] itemSlots = inventoryWindow.transform.GetComponentsInChildren<UI.ItemSlot>();

            int length = itemSlots.Length;

            for(int i = 0; i<length; i++)
            {
                itemSlots[i].UpdateSlotObject();
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.UI;
using UnityEngine.U2D;
using UnityPUBG.Scripts.Utilities;
using UnityPUBG.Scripts.Entities;
using UnityEngine.InputSystem;
using System.Linq;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts.Logic
{
    public class UIManager : Singleton<UIManager>
    {
        public Canvas masterCanvas;
        public RectTransform floatingUIHolder;
        public GameObject normalUIElements;
        public GameObject quickSlotUIElements;
        public GameObject inventoryUIElements;
        [Space]
        public GameObject movementJoystickPivot;
        public GameObject attackJoystickPivot;
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
        [Space]
        public TMP_Text killMessage;
        public TMP_Text remainAmmoText;
        public TMP_Text survivePlayersText;
        [Space]
        public Button playerWeaponSwapButton;
        public Button playerQuickLootButton;
        [Space]
        public ItemConsumeProgress itemConsumeProgress;
        [Space]
        public List<ItemSlot> inventoryItemSlots = new List<ItemSlot>();
        public List<EquipItemSlot> inventoryEquipItemSlots = new List<EquipItemSlot>();
        public List<QuickItemSlot> quickItemSlots = new List<QuickItemSlot>();
        [SerializeField] private SpriteAtlas iconAtlas;

        [Header("Debug Options")]
        public bool keyboardMovement = false;

        private bool dragMovementJoystick;
        private bool dragAttackJoystick;

        public SpriteAtlas IconAtlas => iconAtlas;

        public bool DragMovementJoystick
        {
            get { return dragMovementJoystick; }
            set
            {
                dragMovementJoystick = value;
                movementJoystickPivot.SetActive(!value);
            }
        }
        public bool DragAttackJoystick
        {
            get { return dragAttackJoystick; }
            set
            {
                dragAttackJoystick = value;
                attackJoystickPivot.SetActive(!value);
            }
        }

        private void Awake()
        {
            for (int index = 0; index < inventoryItemSlots.Count; index++)
            {
                inventoryItemSlots[index].slotIndex = index;
                inventoryItemSlots[index].Available = false;
            }
            for (int index = 0; index < quickItemSlots.Count; index++)
            {
                quickItemSlots[index].quickSlotIndex = index;
            }

            normalUIElements.SetActive(true);
            quickSlotUIElements.SetActive(true);
            inventoryUIElements.SetActive(true);
            inventoryUIElements.SetActive(false);

            EntityManager.Instance.OnMyPlayerSpawn += ConnectMyPlayerUIElements;
            EntityManager.Instance.OnPlayerSpawn += EntityManager_OnPlayerSpawn;

            RingSystem.Instance.OnRoundStart += RingSystem_OnRoundStart;
            RingSystem.Instance.OnRingCloseStart += RingSystem_OnRingCloseStart;
        }

        private void FixedUpdate()
        {
            // 디버깅용
            if (keyboardMovement)
            {
                var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (direction == Vector2.zero)
                {
                    if (Keyboard.current.wKey.wasReleasedThisFrame || Keyboard.current.sKey.wasReleasedThisFrame ||
                    Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.aKey.wasReleasedThisFrame)
                    {
                        PlayerMovementJoystick_OnJoystickRelease(null, direction);
                    }
                }
                else
                {
                    PlayerMovementJoystick_OnJoystickDrag(null, direction);
                }
            }
        }

        /// <summary>
        /// 인벤토리 창 열고 닫기
        /// </summary>
        public void ToggleInventoryWindow()
        {
            if (inventoryUIElements.activeSelf)
            {
                normalUIElements.SetActive(true);
                inventoryUIElements.SetActive(false);
            }
            else
            {
                normalUIElements.SetActive(false);
                inventoryUIElements.SetActive(true);
            }
        }

        public void UpdateInventorySlots()
        {
            for (int index = 0; index < inventoryItemSlots.Count; index++)
            {
                if (inventoryItemSlots[index].Available == false)
                {
                    break;
                }

                inventoryItemSlots[index].UpdateSlotObject();
            }
        }

        /// <summary>
        /// 동적으로 아이템 슬롯 갯수를 조정함
        /// </summary>
        public void UpdateInventoryItemSlotSize(int newCapacity)
        {
            for (int index = 0; index < inventoryItemSlots.Count; index++)
            {
                inventoryItemSlots[index].Available = index < newCapacity ? true : false;
            }
        }

        /// <summary>
        /// 퀵슬롯을 업데이트 함
        /// </summary>
        public void UpdateQuickSlots()
        {
            foreach (var quickItemSlot in quickItemSlots)
            {
                quickItemSlot.UpdateQuickItemSlot();
            }
        }

        public void UpdateEquipItemSlots()
        {
            foreach (var equipItemSlot in inventoryEquipItemSlots)
            {
                equipItemSlot.UpdateEquipItemSlot();
            }
        }

        private void ConnectMyPlayerUIElements(object sender, EventArgs e)
        {
            var targetPlayer = EntityManager.Instance.MyPlayer;

            playerMovementJoystick.OnJoystickDrag += PlayerMovementJoystick_OnJoystickDrag;
            playerMovementJoystick.OnJoystickRelease += PlayerMovementJoystick_OnJoystickRelease;
            playerAttackJoystick.OnJoystickDrag += PlayerAttackJoystick_OnJoystickDrag;
            playerAttackJoystick.OnJoystickRelease += PlayerAttackJoystick_OnJoystickRelease;
            playerWeaponSwapButton.onClick.AddListener(() => targetPlayer.SwapWeapon());
            playerQuickLootButton.onClick.AddListener(() => targetPlayer.LootClosestItem());

            // UI 초기값 설정
            playerHealthSlider.maxValue = targetPlayer.MaximumHealth;
            playerHealthSlider.value = targetPlayer.CurrentHealth;
            playerHealthText.text = Mathf.RoundToInt(targetPlayer.CurrentHealth).ToString();

            playerShieldSlider.maxValue = targetPlayer.MaximumShield;
            playerShieldSlider.value = targetPlayer.CurrentShield;
            playerShieldText.text = Mathf.RoundToInt(targetPlayer.CurrentShield).ToString();

            // 이벤트 구독
            targetPlayer.OnDie += MyPlayer_OnDie;
            targetPlayer.OnCurrentHealthUpdate += MyPlayer_OnHealthUpdate;
            targetPlayer.OnCurrentShieldUpdate += MyPlayer_OnShieldUpdate;
            targetPlayer.OnPrimaryWeaponChange += MyPlayer_OnPrimaryWeaponChange;
            targetPlayer.ItemContainer.OnContainerUpdate += MyPlayer_OnContainerUpdate;
            targetPlayer.ItemContainer.OnContainerResize += MyPlayer_OnContainerResize;

            UpdateInventoryItemSlotSize(targetPlayer.ItemContainer.Capacity);
            UpdateInventorySlots();
        }

        private void EntityManager_OnPlayerSpawn(object sender, Player spawnedPlayer)
        {
            UpdateSurvivePlayers();
            spawnedPlayer.OnDie += Player_OnDie;
        }

        private void Player_OnDie(object sender, EventArgs e)
        {
            UpdateSurvivePlayers();
        }

        private void PlayerMovementJoystick_OnJoystickDrag(object sender, Vector2 direction)
        {
            DragMovementJoystick = true;
            EntityManager.Instance.MyPlayer.MoveTo(direction);
            EntityManager.Instance.MyPlayer.RotateTo(direction);
        }

        private void PlayerMovementJoystick_OnJoystickRelease(object sender, Vector2 direction)
        {
            DragMovementJoystick = false;
            EntityManager.Instance.MyPlayer.MoveTo(Vector2.zero);
            EntityManager.Instance.MyPlayer.RotateTo(Vector2.zero);
        }

        private void PlayerAttackJoystick_OnJoystickDrag(object sender, Vector2 direction)
        {
            DragAttackJoystick = true;
            EntityManager.Instance.MyPlayer.AimTo(direction);
        }

        private void PlayerAttackJoystick_OnJoystickRelease(object sender, Vector2 direction)
        {
            DragAttackJoystick = false;
            EntityManager.Instance.MyPlayer.AimTo(Vector2.zero);
            EntityManager.Instance.MyPlayer.AttackTo(direction);
        }

        private void MyPlayer_OnDie(object sender, EventArgs e)
        {
            playerMovementJoystick.OnJoystickDrag -= PlayerMovementJoystick_OnJoystickDrag;
            playerMovementJoystick.OnJoystickRelease -= PlayerMovementJoystick_OnJoystickRelease;
            playerAttackJoystick.OnJoystickRelease -= PlayerAttackJoystick_OnJoystickDrag;
            playerAttackJoystick.OnJoystickRelease -= PlayerAttackJoystick_OnJoystickRelease;
            playerWeaponSwapButton.onClick.RemoveAllListeners();
            playerQuickLootButton.onClick.RemoveAllListeners();

            var myPlayer = sender as Player;
            myPlayer.OnCurrentHealthUpdate -= MyPlayer_OnHealthUpdate;
            myPlayer.OnCurrentShieldUpdate -= MyPlayer_OnShieldUpdate;
            myPlayer.ItemContainer.OnContainerUpdate -= MyPlayer_OnContainerUpdate;
            myPlayer.ItemContainer.OnContainerUpdate -= MyPlayer_OnContainerUpdate;
            myPlayer.ItemContainer.OnContainerResize -= MyPlayer_OnContainerResize;
        }

        private void MyPlayer_OnHealthUpdate(object sender, float value)
        {
            playerHealthSlider.value = value;
            playerHealthText.text = Mathf.CeilToInt(value).ToString();
        }

        private void MyPlayer_OnShieldUpdate(object sender, float value)
        {
            playerShieldSlider.value = value;
            playerShieldText.text = Mathf.CeilToInt(value).ToString();
        }

        private void MyPlayer_OnPrimaryWeaponChange(object sender, EventArgs e)
        {
            UpdateRemainAmmoText();
        }

        private void MyPlayer_OnContainerUpdate(object sender, EventArgs e)
        {
            if (inventoryUIElements.activeSelf)
            {
                UpdateInventorySlots();
            }

            UpdateQuickSlots();
            UpdateRemainAmmoText();
        }

        private void MyPlayer_OnContainerResize(object sender, EventArgs e)
        {
            UpdateInventoryItemSlotSize((sender as ItemContainer).Capacity);
            UpdateQuickSlots();
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

        private void UpdateRemainAmmoText()
        {
            Player targetPlayer = EntityManager.Instance.MyPlayer;
            if (targetPlayer == null)
            {
                remainAmmoText.enabled = false;
                return;
            }

            Item equipedWeapon = targetPlayer.EquipedPrimaryWeapon;
            if (equipedWeapon.IsStackEmpty || (equipedWeapon.Data is RangeWeaponData) == false)
            {
                remainAmmoText.enabled = false;
                return;
            }

            AmmoData requireAmmoData = (equipedWeapon.Data as RangeWeaponData).RequireAmmo;
            List<Item> remainAmmoItems = targetPlayer.ItemContainer.FindAllMatchItem(requireAmmoData.ItemName);
            int remainAmmoCount = remainAmmoItems.Sum(e => e.CurrentStack);

            if (remainAmmoCount > 5)
            {
                remainAmmoText.text = $"남은 화살: {remainAmmoCount}";
            }
            else if (remainAmmoCount > 0)
            {
                remainAmmoText.text = $"남은 화살: <color=yellow>{remainAmmoCount}</color>";
            }
            else
            {
                remainAmmoText.text = $"남은 화살: <color=red>{remainAmmoCount}</color>";
            }

            remainAmmoText.enabled = true;
        }

        private void UpdateSurvivePlayers()
        {
            int survivePlayerCount = EntityManager.Instance.Players
                .Where(e => e.IsDead == false)
                .Count();
            int myPlayerKillCount = 0;

            survivePlayersText.text = myPlayerKillCount > 0 ? $"{survivePlayerCount}명 생존 / {myPlayerKillCount}명 처치" : $"{survivePlayerCount}명 생존";
        }
    }
}
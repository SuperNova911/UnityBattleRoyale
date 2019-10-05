using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class HelpManager : MonoBehaviour
    {
        /// <summary>
        /// 도움말 이미지(스프라이트)들
        /// </summary>
        public Sprite[] HelpImageList;

        /// <summary>
        /// 도움말 스프라이트를 띄울 이미지
        /// </summary>
        public Image HelpImage;

        /// <summary>
        /// 도움말 패널
        /// </summary>
        public GameObject HelpPanel;

        /// <summary>
        /// 현재 HelpImage에 표기된 HelpImageList의 인덱스
        /// </summary>
        int imageIndex = 0;

        bool isHelpPanelOn = false;

        private void Start()
        {
            HelpPanel.SetActive(false);
            isHelpPanelOn = false;
        }

        private void Update()
        {
           if(isHelpPanelOn)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    HelpPanel.SetActive(false);
                    isHelpPanelOn = false;
                }
            }
        }

        /// <summary>
        /// 도움말 패널 활성화 버튼
        /// </summary>
        public void HelpPanelOnButton()
        {
            HelpPanel.SetActive(true);
            isHelpPanelOn = true;
        }

        /// <summary>
        /// 도움말 이미지 갱신 버튼
        /// </summary>
        /// <param name="isNext">다음이미지로 넘어가는가?</param>
        public void ImageUpdateButton(bool isNext)
        {

            if(isNext)
            {
                imageIndex = (imageIndex + 1) % 4;
            }
            else
            {
                imageIndex--;
                if (imageIndex < 0)
                    imageIndex = 3;
            }

            HelpImage.sprite = HelpImageList[imageIndex];
        }
    }
}
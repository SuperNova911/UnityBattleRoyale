using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// 로그인 화면
        /// </summary>
        public GameObject LoginObjects;

        /// <summary>
        /// Press Any Key라고 적힌 텍스트
        /// </summary>
        public Text text;

        /// <summary>
        /// 글자 점멸 속도
        /// </summary>
        float flashSpeed = 0.05f;

        bool isStarted = false;

        private void Start()
        {
            text.gameObject.SetActive(true);
            LoginObjects.SetActive(false);
            isStarted = false;

            StartCoroutine(FlashText());
        }

        private void Update()
        {
            if (Input.anyKey && !isStarted)
            {
                StopCoroutine(FlashText());
                text.gameObject.SetActive(false);
                LoginObjects.SetActive(true);
                //GameObject.Find("TitleImage").SetActive(false);
                isStarted = true;
            }
        }

        /// <summary>
        /// 텍스트 점멸
        /// </summary>
        /// <returns></returns>
        IEnumerator FlashText()
        {
            //텍스트의 알파값이 내려가는 중인가?
            bool isalphadown = true;

            //텍스트의 알파값
            float alpha = 1f;

            while (true)
            {
                if (isalphadown)
                    alpha -= flashSpeed;
                else
                    alpha += flashSpeed;

                if (alpha < 0f)
                    isalphadown = false;
                else if (alpha > 1f)
                    isalphadown = true;

                text.color = new Color(1, 1, 1, alpha);

                yield return new WaitForSeconds(flashSpeed);
            }
        }
    }
}
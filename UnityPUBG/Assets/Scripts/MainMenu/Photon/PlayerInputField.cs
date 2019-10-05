using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    [RequireComponent(typeof(InputField))]
    public class PlayerInputField : MonoBehaviour
    {
        #region Private Variables

        static string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehavior CallBacks

        private void Start()
        {
            string defaultName = string.Empty;

            InputField _inputField = gameObject.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        #endregion

        #region Public Methods

        public void SetPlayerName(string value)
        {
            value = gameObject.GetComponent<InputField>().text;
            PhotonNetwork.playerName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);

            //Debug.Log("PlayerName : " + PlayerPrefs.GetString(playerNamePrefKey));
        }

        #endregion
    }
}
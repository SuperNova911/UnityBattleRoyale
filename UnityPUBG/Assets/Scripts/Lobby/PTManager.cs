using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityPUBG.Scripts.Lobby
{
    public class PTManager : Photon.PunBehaviour
    {
        public static PTManager Instance;

        /// <summary>
        /// 시작 대기 시간
        /// </summary>
        [SerializeField]
        private int waitTime = 10;

        #region 유니티 메시지
        private void Start()
        {
            Instance = this;
        }
        #endregion

        #region Photon Messages
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            //base.OnPhotonPlayerConnected(newPlayer);

            Debug.Log("OnPhotonPlayerConnected() " + newPlayer.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected is MasterClient " + PhotonNetwork.isMasterClient);

                if (PhotonNetwork.playerList.Length == MainMenu.Launcher.Instance.MaxPlayersPerRoom)
                    StartGame();
                else
                    Debug.Log("PlayerCount : " + PhotonNetwork.playerList.Length);
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            //base.OnPhotonPlayerDisconnected(otherPlayer);

            Debug.Log("OnPhotonPlayerDisconnected() " + otherPlayer.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient" + PhotonNetwork.isMasterClient);

                //LoadGameRoom();
            }
        }
        #endregion

        public void LeaveRoom()
        {
            Lobby.LobbyManager.Instance.SendLRoom(PhotonTargets.Others, PhotonNetwork.playerName);
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 게임 룸을 로딩하는 함수
        /// </summary>
        public void StartGame()
        {
            StartCoroutine(GameStart());
        }

        private void LoadGameRoom()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("GameWorld");
        }

        #region 코루틴
        private IEnumerator GameStart()
        {
            UnityEngine.UI.Text countDownText = GameObject.Find("StartCountDown").GetComponent<UnityEngine.UI.Text>();

            for (int i = 0; i < waitTime; i++)
            {
                countDownText.text = "게임 시작까지 : " + (waitTime - i).ToString() + "초";

                yield return new WaitForSeconds(1f);
            }

            LoadGameRoom();

            yield break;
        }
        #endregion
    }
}
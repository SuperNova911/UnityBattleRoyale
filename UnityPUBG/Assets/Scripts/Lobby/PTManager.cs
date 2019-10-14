using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityPUBG.Scripts.Lobby
{
    public class PTManager : Photon.PunBehaviour
    {
        #region public 변수, 유니티 에디터에서만 사용
        public static PTManager Instance;
        #endregion

        #region 유니티 콜백, 유니티 에디터에서만 사용
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

        #region Private Methods

        private void LoadGameRoom()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("SandBox");
        }

        #endregion

        #region Public Methods

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

        #endregion

        #region 코루틴

        private IEnumerator GameStart()
        {
            UnityEngine.UI.Text countDownText = GameObject.Find("StartCountDown").GetComponent<UnityEngine.UI.Text>();

            for(int i = 0; i<10; i++)
            {
                countDownText.text = "게임 시작까지 : " + (10 - i).ToString() + "초";

                yield return new WaitForSeconds(1f);
            }

            LoadGameRoom();

            yield break;
        }

        #endregion
    }
}
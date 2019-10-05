using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityPUBG.Scripts.Lobby
{
    public class PTManager : Photon.PunBehaviour
    {
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

                LoadGameRoom();
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            //base.OnPhotonPlayerDisconnected(otherPlayer);

            Debug.Log("OnPhotonPlayerDisconnected() " + otherPlayer.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient" + PhotonNetwork.isMasterClient);

                LoadGameRoom();
            }
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            Lobby.LobbyManager.Instance.SendLRoom(PhotonTargets.Others, PhotonNetwork.playerName);
            PhotonNetwork.LeaveRoom();
        }

        public void StartGame()
        {
            PhotonNetwork.LoadLevel("Game");
        }

        #endregion

        #region Private Methods

        private void LoadGameRoom()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }

            Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
            //PhotonNetwork.LoadLevel("Game");
        }

        #endregion
    }
}
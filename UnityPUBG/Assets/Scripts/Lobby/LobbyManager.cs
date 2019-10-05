using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityPUBG.Scripts.Lobby
{
    public class ReadyPlayer
    {
        public bool Ready { private set; get; }
        public string Name;
        public GameObject obj { private set; get; }
        Text playerName;
        Text readyState;
        Image backGround;

        public ReadyPlayer()
        {
            Ready = false;
            Name = null;
            obj = null;
        }

        public ReadyPlayer(string name, GameObject lobbyPlayer)
        {
            Ready = false;
            Name = name;
            obj = lobbyPlayer;

            playerName = obj.transform.Find("PlayerName").GetComponent<Text>();
            readyState = obj.transform.Find("ReadyState").GetComponent<Text>();
            backGround = obj.transform.Find("BackGround").GetComponent<Image>();

            playerName.text = name;
            readyState.text = "Wait";
            backGround.color = Color.white;
        }

        public void SetReady(bool ready)
        {
            Ready = ready;

            if(Ready)
            {
                backGround.color = Color.green;
                readyState.text = "Ready";
            }
            else
            {
                backGround.color = Color.white;
                readyState.text = "Wait";
            }
        }
    }

    public class LobbyManager : Photon.MonoBehaviour
    {
        /// <summary>
        /// 준비 버튼의 텍스트
        /// </summary>
        public Text ReadyButtonText;

        /// <summary>
        /// 준비중인가?
        /// </summary>
        bool isReady = false;

        /// <summary>
        /// 플레이어들의 준비 여부
        /// </summary>
        List<ReadyPlayer> RPlayerList = new List<ReadyPlayer>();

        public GameObject StartButton;

        public GameObject LobbyPlayer;
        public Transform PlayerPanel;

        public static LobbyManager Instance;

        private void Start()
        {
            isReady = false;
            Instance = this;
            StartButton.SetActive(false);
            if(PhotonNetwork.playerList.Length == 1)
            {
                RPlayerList.Add(new ReadyPlayer(PhotonNetwork.playerName, Instantiate(LobbyPlayer,PlayerPanel)));
            }
            else
            {
                SendE(PhotonTargets.Others, PhotonNetwork.playerName);
            }
        }

        public void ReadyButton()
        {
            isReady = !isReady;

            if (isReady)
                ReadyButtonText.text = "Wait";
            else
                ReadyButtonText.text = "Ready";

            SendRMessage(PhotonTargets.All, PhotonNetwork.player.NickName, isReady);
        }

        #region RPC함수 껍데기
        /// <summary>
        /// SendReadyMessage를 호출하는 함수
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="readyState"></param>
        void SendRMessage(PhotonTargets targets, string playerName, bool readyState)
        {
            photonView.RPC("SendReadyMessage", targets, playerName, readyState);
        }

        /// <summary>
        /// SendEntry를 호출하는 함수
        /// </summary>
        /// <param name="targets"></param>
        void SendE(PhotonTargets targets, string playerName)
        {
            photonView.RPC("SendEntry", targets, playerName);
        }

        /// <summary>
        /// SendMakeList를 호출하는 함수
        /// 마스터 클라이언트가
        /// 새로 입장한 클라이언트에게
        /// 리스트 생성하라고 요청
        /// </summary>
        /// <param name="player">새로 입장한 클라이언트</param>
        /// <param name="playerNames">플레이어 이름들</param>
        /// <param name="readyList">플레이어 준비 상태 들</param>
        void SendMList(PhotonPlayer player, string[] playerNames, bool[] readyList)
        {
            photonView.RPC("SendMakeList", player, playerNames, readyList);
        }

        /// <summary>
        /// SendLeaveRoom을 호출하는 함수
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="playerName"></param>
        public void SendLRoom(PhotonTargets targets, string playerName)
        {
            photonView.RPC("SendLeaveRoom", targets, playerName);
        }
        #endregion

        #region PunRPC Functions
        /// <summary>
        /// 준비 메시지 발송
        /// </summary>
        /// <param name="playerName">준비버튼을 누른 플레이어</param>
        /// <param name="readyState">준비 상태</param>
        [PunRPC]
        void SendReadyMessage(string playerName, bool readyState)
        {
            string s;

            if (readyState)
                s = "준비됨";
            else
                s = "준비 안됨";
            Debug.Log(playerName + "의 준비 상태 : " + s);

            bool everyReady = true;
            for(int i = 0; i<RPlayerList.Count; i++)
            {
                if(playerName == RPlayerList[i].Name)
                {
                    RPlayerList[i].SetReady(readyState);
                    //break;
                }

                if (!RPlayerList[i].Ready)
                    everyReady = false;
            }

            if (everyReady && PhotonNetwork.isMasterClient)
                StartButton.SetActive(true);
            else
                StartButton.SetActive(false);
        }

        /// <summary>
        /// 플레이어가 방에 입장 했을 때
        /// 플레이어 리스트 갱신
        /// </summary>
        [PunRPC]
        void SendEntry(string playerName)
        {
            RPlayerList.Add(new ReadyPlayer(playerName, Instantiate(LobbyPlayer,PlayerPanel)));

            //마스터 클라이언트라면
            //입장한 클라이언트에게
            //리스트 생성 요청
            if(PhotonNetwork.isMasterClient)
            {
                PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
                int len = photonPlayers.Length;
                int i;
                for(i = 0; i<len; i++)
                {
                    if (photonPlayers[i].NickName == playerName)
                        break;
                }

                if(i!=len)
                {
                    string[] s = new string[RPlayerList.Count];
                    bool[] b = new bool[RPlayerList.Count];

                    for(int j = 0; j<s.Length; j++)
                    {
                        s[j] = RPlayerList[j].Name;
                        b[j] = RPlayerList[j].Ready;
                    }

                    SendMList(photonPlayers[i], s, b);
                }
            }
        }

        /// <summary>
        /// 새로 입장한 클라이언트의
        /// RPlayerList 생성
        /// </summary>
        /// <param name="playerNames"></param>
        /// <param name="playerReadys"></param>
        [PunRPC]
        void SendMakeList(string[] playerNames, bool[] playerReadys)
        {
            int len = playerNames.Length;

            for(int i = 0; i<len; i++)
            {
                RPlayerList.Add(new ReadyPlayer(playerNames[i], Instantiate(LobbyPlayer, PlayerPanel)));
                RPlayerList[i].SetReady(playerReadys[i]);
            }
        }

        /// <summary>
        /// 로비에서 나갈 시
        /// 다른 클라이언트들에게
        /// 나갔다고 알림
        /// </summary>
        /// <param name="playerName">나간 플레이어 이름</param>
        [PunRPC]
        void SendLeaveRoom(string playerName)
        {
            int len = RPlayerList.Count;

            for(int i = 0; i<len; i++)
            {
                if (RPlayerList[i].Name == playerName)
                {
                    Destroy(RPlayerList[i].obj);
                    RPlayerList.RemoveAt(i);
                    break;
                }
            }
        }
        #endregion
    }
}
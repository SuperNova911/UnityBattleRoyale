using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityPUBG.Scripts.Lobby
{
    public class ChatManager : Photon.MonoBehaviour
    {
        //public List<string> chatlist = new List<string>();
        public Text chatBox;
        public InputField chatInputField;
        public Scrollbar scroll;

        private string lastmsg = "";
        private bool isdowned = false;

        #region Unity CallBacks
        private void Start()
        {
            StartCoroutine(ScrollUpdate());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (chatInputField.text != string.Empty)
                {
                    Send(PhotonTargets.All, chatInputField.text);
                    chatInputField.text = string.Empty;
                    chatInputField.ActivateInputField();
                }

                else
                {
                    chatInputField.Select();
                }


            }
            //scroll.value = 0;
        }
        #endregion

        public IEnumerator ScrollUpdate()
        {
            while (true)
            {
                if (!isdowned && lastmsg != chatBox.text)
                {
                    isdowned = true;
                    lastmsg = chatBox.text;
                }

                yield return new WaitForEndOfFrame();

                if (isdowned)
                {
                    scroll.value = 0;
                    isdowned = false;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private void Send(PhotonTargets targets, string msg)//다른 클라이언트들에게 메시지를 보내기 위해 rpc 함수 호출
        {
            msg = PhotonNetwork.player.NickName + " : " + msg;//메시지 보낸 플레이어의 이름 표시
                                                              //Debug.Log(msg);
            photonView.RPC("SendMsg", targets, msg);
        }

        private void AddChatBox(string msg)//채팅 메시지 보여줌
        {
            string chat = chatBox.text;
            chat += (msg + "\n");
            chatBox.text = chat;
            scroll.value = 0;
            //chatlist.Add(msg);
        }

        #region PRC 함수
        [PunRPC]
        private void SendMsg(string msg)//메시지를 보내기 위한 rpc 함수
        {
            AddChatBox(msg);
        }
        #endregion
    }
}
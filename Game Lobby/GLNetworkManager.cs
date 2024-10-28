using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Chat;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Unity.VisualScripting;

//게임 로비의 채팅 구현, 씬 이동 구현
public class GameLobbyManager : MonoBehaviourPunCallbacks {
    public PhotonView PV;
    public InputField ChatInput;
    public Text[] ChatText;
    public EventSystem eventSystem;
    public bool isChatFocused = false;
 

    private bool missionReceived;


    private void Start() {
        missionReceived = false;
    }


    private void Update() {
      
        if (Input.GetKeyDown(KeyCode.Return)) {
            HandleEnterKeyPress();
        }
    }
    private void HandleEnterKeyPress() {
        if (isChatFocused) {
            if (!string.IsNullOrWhiteSpace(ChatInput.text)) {
                // 채팅 내용을 보냄
                Send();
            }
            // 포커스를 해제함
            eventSystem.SetSelectedGameObject(null);
            isChatFocused = false;
        }
        else {
            // 입력창에 포커스를 맞춤
            eventSystem.SetSelectedGameObject(ChatInput.gameObject);
            isChatFocused = true;
        }
    }
    //public void HandleChatInputEndEdit(string text) {
    //    // 입력창이 이미 포커스 상태인지 확인
    //    if (isChatFocused && !string.IsNullOrWhiteSpace(ChatInput.text)) {
    //        if (eventSystem.currentSelectedGameObject != ChatInput.gameObject) {
    //            eventSystem.SetSelectedGameObject(ChatInput.gameObject);
    //        }
    //    }
    //}



    #region 출동

    [PunRPC]
    public void SetMissionReceived() {
        missionReceived = true;
    }

    [PunRPC]
    public void StartLaunchCountdown() {
        if (missionReceived && PhotonNetwork.IsMasterClient) {
            StartCoroutine(LaunchCountdownCoroutine());
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("ChatRPC", RpcTarget.All, "미션을 먼저 받아야 합니다.");
            }
        }
    }

    private IEnumerator LaunchCountdownCoroutine() {
        int countdown = 3;
        while (countdown > 0) {
            photonView.RPC("ChatRPC", RpcTarget.All, $"출동 {countdown}초전...");
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        photonView.RPC("ChatRPC", RpcTarget.All, "출동!");
        LoadGameScene();
    }

    private void LoadGameScene() {
        if (PhotonNetwork.IsMasterClient) {
          
           
            PhotonNetwork.LoadLevel("Loading for Lobby to Planet");
        }
    }
    #endregion

    #region 채팅
    public void Send() {
        if (PhotonNetwork.InRoom) {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";

        }
        else {
            Debug.LogWarning("Cannot send message. Not currently in a room.");
        }
    }

    [PunRPC]
    private void ChatRPC(string msg) {
        StartCoroutine(UpdateChatText(msg));
    }

    private IEnumerator UpdateChatText(string msg) {
        yield return new WaitForEndOfFrame(); // RectTransform 변경이 완료된 후에 실행

        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++) {
            if (ChatText[i].text == "") {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        }
        if (!isInput) {
            for (int i = 1; i < ChatText.Length; i++) {
                ChatText[i - 1].text = ChatText[i].text;
            }
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("DisconnectPanel")]
    public InputField NickNameInput;
    public GameObject DisconnectPanel;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public Button Exit;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    // 게임을 시작할 최소 플레이어 수
    public int minPlayersToStart = 2;


    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion


    #region 서버연결


    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        if (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby && PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            // 네트워크 상태가 좋지 않을 경우 사용자에게 경고
            Debug.Log("네트워크 상태가 불안정합니다.");
        }
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        DisconnectPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        DisconnectPanel.SetActive(true);
    }
    #endregion


    #region 방
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 4 });

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room");
        if (RoomPanel == null) Debug.LogError("RoomPanel is null");
        if (LobbyPanel == null) Debug.LogError("LobbyPanel is null");
        if (ListText == null) Debug.LogError("ListText is null");
        if (RoomInfoText == null) Debug.LogError("RoomInfoText is null");
        RoomPanel.SetActive(true);
        LobbyPanel.SetActive(false );
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }

    void RoomRenewal()
    {
        if (ListText == null || RoomInfoText == null)
        {
            Debug.LogError("UI component is not assigned in the inspector");
            return;
        }
        // 각 UI 컴포넌트가 적절히 할당되었는지 로그 출력
        Debug.Log("Updating room information...");
        if (RoomInfoText == null)
        {
            Debug.LogError("RoomInfoText component is not assigned in the inspector");
            return;
        }

        ListText.text = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            ListText.text += player.NickName + ", ";
        }
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " +
                            PhotonNetwork.CurrentRoom.PlayerCount + "명 / " +
                            PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }
    #endregion


    #region 채팅
    public void Send()
    {
        if (PhotonNetwork.InRoom)
        {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";
        }
        else
        {
            Debug.LogWarning("Cannot send message. Not currently in a room.");
        }
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion




    #region 게임입장


    public Button StartGameButton; 

    public void StartGame()
    {
        Debug.Log("startGame");

        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트인지 확인
        {
            PhotonNetwork.LoadLevel("GamePlay");
        }
    }

    private void Start()
    {
        StartGameButton.onClick.AddListener(StartGame);
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // 게임을 시작하기 전에 플레이어 수를 확인하는 메소드
    public void CheckPlayersAndStartGame()
    {
        // 현재 방에 충분한 플레이어가 있는지 확인
        if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersToStart)
        {
            // 마스터 클라이언트만이 게임을 시작할 수 있음
            if (PhotonNetwork.IsMasterClient)
            {
                StartGameButton.interactable = false; // 버튼 비활성화
                StartGame();
            }
        }
        else
        {
            Debug.LogWarning("Not enough players to start the game.");
        }
    }
    #endregion

}
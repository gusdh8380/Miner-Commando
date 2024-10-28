using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyGameManager : MonoBehaviourPunCallbacks {
    [SerializeField]
    GameObject player1Prefab;
    [SerializeField]
    GameObject player2Prefab;
    [SerializeField]
    GameObject player3Prefab;
    [SerializeField]
    GameObject player4Prefab;

    [SerializeField]
    private Transform[] startPositions;  // 시작 위치를 배열로 관리

    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start() {
        if (PhotonNetwork.IsConnectedAndReady) {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer() {
        // 플레이어 인덱스에 따라 다른 위치에 스폰
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;  // ActorNumber starts at 1
        index = index % startPositions.Length;  // 위치가 부족할 경우를 대비한 나머지 연산

        GameObject playerPrefab = GetPlayerPrefab(PhotonNetwork.LocalPlayer.ActorNumber);
        if (playerPrefab != null) {
            PhotonNetwork.Instantiate(playerPrefab.name, startPositions[index].position, Quaternion.identity);
        }
        else {
            Debug.LogError("Player prefab is not assigned or not found.");
        }
    }

    private GameObject GetPlayerPrefab(int actorNumber) {
        switch (actorNumber) {
            case 1:
                return player1Prefab;
            case 2:
                return player2Prefab;
            case 3:
                return player3Prefab;
            case 4:
                return player4Prefab;
            default:
                return player1Prefab;  // 기본값으로 player1Prefab을 사용
        }
    }
}

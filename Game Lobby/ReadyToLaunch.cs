using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToLaunch : InteractableEntity
{

    private int playerCount = 0;
    public bool isLaunching = false;
    public GameLobbyManager gameLobbyManager;
    public Transform outsideShip;
    private void Start() {
        gameLobbyManager = FindObjectOfType<GameLobbyManager>();
    }
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.position = outsideShip.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (PhotonNetwork.IsMasterClient) { 
            playerCount++;
            if (playerCount >= PhotonNetwork.CurrentRoom.PlayerCount && !isLaunching && PhotonNetwork.IsMasterClient) {
                isLaunching = true;
                gameLobbyManager.photonView.RPC("StartLaunchCountdown", RpcTarget.All);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other) {
        if(PhotonNetwork.IsMasterClient) {
            playerCount--;
            if (playerCount < PhotonNetwork.CurrentRoom.PlayerCount) {
                isLaunching = false;
            }
        }
    }

}

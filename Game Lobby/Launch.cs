using UnityEngine;
using Photon.Pun;

public class LaunchPadTrigger : InteractableEntity {
    //private int playerCount = 0;
    //public bool isLaunching = false;
    //public GameLobbyManager gameLobbyManager;


    public Transform insideShip;
    //private void Start() {
    //    gameLobbyManager = FindObjectOfType<GameLobbyManager>();
    //}

    //private void OnTriggerEnter2D(Collider2D other) {
    //    if (other.CompareTag("Player")) {
    //        playerCount++;
    //        if (playerCount >= PhotonNetwork.CurrentRoom.PlayerCount && !isLaunching && PhotonNetwork.IsMasterClient) {
    //            isLaunching = true;
    //            gameLobbyManager.photonView.RPC("StartLaunchCountdown", RpcTarget.All);
    //        }
    //    }
    //}

    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.position = insideShip.position;
    }


    //private void OnTriggerExit2D(Collider2D other) {
    //    if (other.CompareTag("Player")) {
    //        playerCount--;
    //        if (playerCount < PhotonNetwork.CurrentRoom.PlayerCount) {
    //            isLaunching = false;
    //        }
    //    }
    //}
}

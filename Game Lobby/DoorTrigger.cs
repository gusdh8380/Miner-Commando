using UnityEngine;
using Photon.Pun;
//함장실 문 앞에 플레이어가 모이면
//함장실로 이동하여 미션을 받음
public class DoorTrigger : InteractableEntity{
    //private int playerCount = 0;
    public MissionInteraction missionInteraction;
    private bool isMissionActive = false;


    private void Start() {
        if (missionInteraction == null) {
            missionInteraction = FindObjectOfType<MissionInteraction>();
            if (missionInteraction == null) {
                Debug.LogError("MissionInteraction not found in the scene.");
            }
            else if (!missionInteraction.photonView) {
                Debug.LogError("MissionInteraction does not have a PhotonView component.");
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D other) {
    //    if (other.CompareTag("Player")) {
    //        playerCount++;
    //        if (playerCount >= 1&& !isMissionActive && PhotonNetwork.IsMasterClient) //2명 이상일 때 함장실 UI 활성화
    //        {
    //            isMissionActive = true;
    //            missionInteraction.photonView.RPC("ShowMissionUI", RpcTarget.All);
    //        }
    //    }
    //}

    public override void Interact(GameObject subject) {
        if (PhotonNetwork.IsMasterClient && !isMissionActive) {
            base.Interact(subject);
            isMissionActive = true;
            missionInteraction.photonView.RPC("ShowMissionUI", RpcTarget.All);
        }
    }

    //나중에 4인 플레이인 경우 
    //private void OnTriggerEnter(Collider other) {
    //    if (other.CompareTag("Player")) {
    //        playerCount++;
    //        if (playerCount == 4) // 플레이어가 4명 모였을 때
    //        {
    //            missionInteraction.photonView.RPC("ShowMissionUI", Photon.Pun.RpcTarget.All);
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other) {
    //    if (other.CompareTag("Player")) {
    //        playerCount--;
    //        if (playerCount < 1) {
    //            isMissionActive = false;
    //        }
    //    }
    //}
    

}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//채굴선 입구
public class MinerShipEntrance: InteractableEntity {
    [SerializeField]
    private Transform minerShipInteriorPos;

    //상호작용한 플레이어를 채굴선 내부로 이동시킨다.
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.position = minerShipInteriorPos.position;
        //if (StageManager.Instance.bShouldLight) {
        //    subject.GetPhotonView().RPC("LightOff", RpcTarget.All);
        //}
        CameraManager.instance.GetInShip();
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.enter);

    }
}

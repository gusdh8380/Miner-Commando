using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//채굴선 내부의 출구
public class Exit : InteractableEntity {
    [SerializeField]
    private Transform minerShipEntrancePos;

    //상호작용한 플레이어를 채굴선 외부로 이동시킨다
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.position = minerShipEntrancePos.position;
        subject.transform.Find("SpecialAmmo").gameObject.SetActive(false);
        if (StageManager.Instance.bShouldLight) {
            subject.GetPhotonView().RPC("LightOn", RpcTarget.All);
        }
        CameraManager.instance.GetOutShip(subject);
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.enter);

    }
}

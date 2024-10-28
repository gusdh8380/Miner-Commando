using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

//���� ��� ���� Ư�� ���� ź�� ĭ
public class AmmoCompartment : InteractableEntity, IPunObservable {
    //���� ������ ź���� ��
    public int magCapacity;
    //������ ź�� ��
    private int _magAmmo;
    public TextMeshProUGUI specialAttackText;
    private new PhotonView photonView;


    public int magAmmo {
        get { return _magAmmo; }
        set {
            _magAmmo = value;
            UIManager.instance.UpdateMagAmmo(_magAmmo, magCapacity);
        }
    }

    private void Start() {
        photonView = GetComponent<PhotonView>();
        magAmmo = magCapacity;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(magAmmo);
        }
        else {
            magAmmo = (int)stream.ReceiveNext();
        }
    }

    public void UpdateUI() {
        if (specialAttackText != null) {
            specialAttackText.text = $"{magAmmo}/{magCapacity}";
        }
    }

    public override void Interact(GameObject subject) {
        base.Interact(subject);
        if (subject.transform.Find("SpecialAmmo").gameObject.activeSelf && magAmmo < magCapacity) {
            photonView.RPC("AddAmmo", RpcTarget.All);
            subject.transform.Find("SpecialAmmo").gameObject.SetActive(false);
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.loadAmmo);

        }
    }

    [PunRPC]
    public void AddAmmo() {
        if (magAmmo < magCapacity) {
            magAmmo++;
        }
    }


}

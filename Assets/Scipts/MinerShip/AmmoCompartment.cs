using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoCompartment : InteractableEntity {
    public int magCapacity;
    public int magAmmo;
    private void Start() {
        magAmmo = magCapacity;
    }

    [PunRPC]
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        if (subject.transform.Find("SpecialAmmo").gameObject.activeSelf && magAmmo < magCapacity) {
            magAmmo++;
            subject.transform.Find("SpecialAmmo").gameObject.SetActive(false);
        }
    }
}

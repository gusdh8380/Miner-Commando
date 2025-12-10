using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Armory : InteractableEntity {

    [PunRPC]
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.Find("SpecialAmmo").gameObject.SetActive(true);
    }
}

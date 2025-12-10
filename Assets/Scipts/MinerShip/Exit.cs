using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : InteractableEntity {
    [SerializeField]
    private Transform minerShipEntrancePos;
    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.position = minerShipEntrancePos.position;
        subject.transform.Find("SpecialAmmo").gameObject.SetActive(false);
    }
}

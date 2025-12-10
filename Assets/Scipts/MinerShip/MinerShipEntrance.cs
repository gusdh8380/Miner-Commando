using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MinerShipEntrance: InteractableEntity {
    [SerializeField]
    private Transform minerShipInteriorPos;

    public override void Interact(GameObject subject) {
        base.Interact(subject);
        subject.transform.position = minerShipInteriorPos.position;
    }
}

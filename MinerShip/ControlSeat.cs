using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSeat : InteractableEntity {
    [SerializeField]
    private ShipController controller;

    public override void Interact(GameObject subject) {
        base.Interact(subject);
        
        controller.Activate(subject);//컨트롤러 활성화
    }
}

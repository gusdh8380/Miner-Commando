using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShipController : MonoBehaviourPun, IController
{
    public GameObject handler;
    private PlayerInput handlerInput;
    [SerializeField]
    GameObject basicUI;
    protected bool firstFrameChecked;
    private string stopInteractButtonName = "StopInteract";

    public bool StopInteract { get; private set; }
    public virtual void StopControl() {
        if (photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);  // 조종 권한 반납
        }
        Debug.Log("stop");
        handlerInput.canReceiveInput = true;
        handler = null;
        handlerInput = null;
        basicUI.SetActive(true);
    }

    protected void CheckInteractionStopped() {
        StopInteract = Input.GetButtonDown(stopInteractButtonName);

        if (firstFrameChecked) {
            StopInteract = false;
            firstFrameChecked = false;
        }
        
        if (StopInteract && photonView.IsMine) {
            Debug.Log("STOP");
            StopControl();
        }
    }

    public virtual void Activate(GameObject subject) {
        if (handler != null) {
            StopControl();
        }
        handler = subject;
        handlerInput = handler.GetComponent<PlayerInput>();
        handlerInput.canReceiveInput = false;
        photonView.RequestOwnership();  // 소유권 요청
        firstFrameChecked = true;
        Debug.Log("activate");
    }

}

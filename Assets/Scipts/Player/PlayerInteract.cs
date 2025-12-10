using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Photon.Pun;

public class PlayerInteract : MonoBehaviourPun
{
    private PlayerInput playerInput;
    private Animator anim;
    private IInteractable interactionTarget;
    private Ore mineTarget;
    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if(!photonView.IsMine)
        {
            return;
        }


        Debug.Log("상호작용하려는 대상: " + interactionTarget);
        InteractWithObject();
        if (playerInput.MoveX != 0 || playerInput.MoveY != 0) {
            mineTarget = null;
        }
    }


    //트리거 시스템으로 연결된 개체와 상호작용
    private void InteractWithObject()
    {
        if (interactionTarget != null && playerInput.Interact)
        {
            interactionTarget.Interact(gameObject);
        }
    }
    public void SwingPickaxe(Ore ore)
    {
        photonView.RPC("RPC_SwingPickaxe", RpcTarget.All, ore.gameObject.GetPhotonView().ViewID);
    }
    //Mine
    [PunRPC]
    private void RPC_SwingPickaxe(int oreID)
    {
        Ore ore = PhotonView.Find(oreID).GetComponent<Ore>();
        if (ore != null)
        {
            anim.SetTrigger("SwingPickaxe");
            mineTarget = ore;
        }
    }

    public void OnPickaxeStrike() {
        if (mineTarget != null) {
            mineTarget.OnHit();
        }
        else { Debug.Log("Strike Nothing"); }
    }



    //Trigger

    private void OnTriggerEnter2D(Collider2D collision) {
        IInteractable target = collision.GetComponent<IInteractable>();

        if (target != null) {
            if (interactionTarget != null) {
                interactionTarget.HideUI();
            }
            interactionTarget = target;

            interactionTarget.ShowUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        IInteractable target = collision.GetComponent<IInteractable>();

        if (target != null) {
            if (target == interactionTarget) {
                target.HideUI();
                interactionTarget = null;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (interactionTarget == null) {
            IInteractable target = collision.GetComponent<IInteractable>();
            if(target != null) {
                interactionTarget = target;
                interactionTarget.ShowUI();
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Photon.Pun;

//�÷��̾� ��ȣ�ۿ�
public class PlayerInteract_Lobby : MonoBehaviourPun {
    private PlayerInput_Lobby playerInput;
    private Animator anim;
    private IInteractable interactionTarget;
    private Ore mineTarget;
    private void Start() {
        playerInput = GetComponent<PlayerInput_Lobby>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        if (!photonView.IsMine) {
            return;
        }


        Debug.Log("상호작용대상: " + interactionTarget);
        InteractWithObject();
        if (playerInput.MoveX != 0 || playerInput.MoveY != 0) {
            mineTarget = null;
        }
    }


    //Ʈ���� �ý������� ����� ��ü�� ��ȣ�ۿ�
    private void InteractWithObject() {
        if (interactionTarget != null && playerInput.Interact) {
            interactionTarget.Interact(gameObject);
        }
    }





    //Trigger
    //��ȣ�ۿ��� ��ü����� ���� �ý���
    private void OnTriggerEnter2D(Collider2D collision) {
        if (photonView.IsMine) {
            IInteractable target = collision.GetComponent<IInteractable>();

            if (target != null) {
                if (interactionTarget != null) {
                    interactionTarget.HideUI();
                }
                interactionTarget = target;

                interactionTarget.ShowUI();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (photonView.IsMine) {
            IInteractable target = collision.GetComponent<IInteractable>();

            if (target != null) {
                if (target == interactionTarget) {
                    target.HideUI();
                    interactionTarget = null;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (photonView.IsMine) {
            if (interactionTarget == null) {
                IInteractable target = collision.GetComponent<IInteractable>();
                if (target != null) {
                    interactionTarget = target;
                    interactionTarget.ShowUI();
                }
            }
        }
    }

}

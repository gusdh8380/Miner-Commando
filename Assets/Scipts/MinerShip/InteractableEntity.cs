using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableEntity : MonoBehaviourPun, IInteractable
{
    [SerializeField]
    private GameObject interactionUI;

    public void ShowUI() {
        interactionUI.SetActive(true);
        Debug.Log("show ui");
    }

    public void HideUI() {
        interactionUI.SetActive(false);
        Debug.Log("hide ui");
    }

    public virtual void Interact(GameObject subject) {
        Debug.Log("상호작용");
    }
}

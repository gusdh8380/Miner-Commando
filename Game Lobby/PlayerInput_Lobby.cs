using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class PlayerInput_Lobby : MonoBehaviourPun {
    private string MoveXAxisName = "Horizontal";
    private string MoveYAxisName = "Vertical";
    private string InteractButtonName = "Interact";

    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    public bool Interact { get; private set; }

    public bool canReceiveInput = true;
    public GameLobbyManager gameLobbyManager;

    void Start() {
        gameLobbyManager = FindObjectOfType<GameLobbyManager>();
    }

    void Update() {
        if (!photonView.IsMine || gameLobbyManager.isChatFocused) return;

        if (!canReceiveInput) {
            MoveX = 0;
            MoveY = 0;
            Interact = false;
            return;
        }

        MoveX = Input.GetAxisRaw(MoveXAxisName);
        MoveY = Input.GetAxisRaw(MoveYAxisName);

        Interact = Input.GetButtonDown(InteractButtonName);
    }
}

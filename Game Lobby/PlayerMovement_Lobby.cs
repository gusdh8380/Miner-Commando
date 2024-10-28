using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement_Lobby : MonoBehaviourPun {
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private PlayerInput_Lobby playerInput;
    Animator anim;
    public GameLobbyManager gameLobbyManager;

    private bool isMoving;

    private bool facingRight = true;

    void Start() {
        Time.timeScale = 1;
        
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput_Lobby>();
        gameLobbyManager = FindObjectOfType<GameLobbyManager>();
        anim = GetComponent<Animator>();

    }

    void FixedUpdate() {
        if (!photonView.IsMine || gameLobbyManager.isChatFocused) {
            rb.velocity = Vector3.zero;
            return;
        }


        if (playerInput.MoveX != 0 && (facingRight ^ playerInput.MoveX > 0)) {
            Flip();
        }
        Move();

       anim.SetBool("isMoving", isMoving);

    }
    private void Move() {
        rb.velocity = new Vector2(playerInput.MoveX, playerInput.MoveY) * moveSpeed;

        if (playerInput.MoveX != 0 || playerInput.MoveY != 0) {
            isMoving = true;
        }
        else {
            isMoving = false;
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }
}

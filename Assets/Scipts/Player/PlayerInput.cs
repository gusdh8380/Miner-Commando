using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPun {
    private string MoveXAxisName = "Horizontal";
    private string MoveYAxisName = "Vertical";
    private string InteractButtonName = "Interact";


    public float MoveX { get; private set;}
    public float MoveY { get; private set;}
    public bool Interact { get; private set;}

    public bool canReceiveInput;

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //게임오버에 대한 코드 필요 StageManager
        if (!canReceiveInput) {
            MoveX = 0;
            MoveY = 0;
            Interact = false;
            return;
        }

        MoveX= Input.GetAxisRaw(MoveXAxisName);
        MoveY = Input.GetAxisRaw(MoveYAxisName);

        Interact = Input.GetButtonDown(InteractButtonName);
    }
}


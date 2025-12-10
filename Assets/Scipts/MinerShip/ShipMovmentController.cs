using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ShipMovmentController : ShipController
{
    private Rigidbody2D shipRb;
    [SerializeField]
    private float moveSpeed;

    private string moveXAxisName = "Horizontal";
    private string moveYAxisName = "Vertical";

    public float MoveX { get; private set; }
    public float MoveY { get; private set; }


    private void Start()
    {
        shipRb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        //게임오버에 대한 코드 필요 StageManager
        if (photonView.IsMine)
        {

            if (handler == null)
            {
                shipRb.velocity = Vector2.zero;
                return;
            }
            MoveX = Input.GetAxisRaw(moveXAxisName);
            MoveY = Input.GetAxisRaw(moveYAxisName);


            Move();

        }
        //else if(!handler.photonView.IsMine) return;    null 이면 검사하면 오류나니까


        CheckInteractionStopped();


    }



    private void Move()
    {
        // Move 메소드 내에서도 소유권 확인을 추가할 수 있습니다.
        if (handler != null && photonView.IsMine)
        {
            shipRb.velocity = new Vector2(MoveX, MoveY) * moveSpeed;
        }
        else
        {
            shipRb.velocity = Vector2.zero;
        }
    }
}

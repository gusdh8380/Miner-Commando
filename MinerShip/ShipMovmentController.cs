using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//채굴선 이동 컨트롤러
public class ShipMovmentController : ShipController {
    [SerializeField]
    GameObject ship;
    PhotonView shipPhotonView;
    private Rigidbody2D shipRb;
    [SerializeField]
    public float moveSpeed;

    //이동 입력 축
    private string moveXAxisName = "Horizontal";
    private string moveYAxisName = "Vertical";


    public float MoveX { get; private set; }
    public float MoveY { get; private set; }


    private void Start()
    {
        shipRb = ship.GetComponent<Rigidbody2D>();
        shipPhotonView = ship.GetPhotonView();
    }

        

    void Update()
    {
        
        //���ӿ����� ���� �ڵ� �ʿ� StageManager

        //핸들러가 없거나 로컬 소유가 아니면 중지

            Move(); //이동처리

        if (handler == null)
            {
                //shipRb.velocity = Vector2.zero;
                //shipRb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                return;
            }
            MoveX = Input.GetAxisRaw(moveXAxisName);
            MoveY = Input.GetAxisRaw(moveYAxisName);


            CheckInteractionStopped(); //상호작용 중지 여부 확인

    }

    public override void Activate(GameObject subject)
    {
        photonView.RPC("StopControl", RpcTarget.Others);
        base.Activate(subject);
        UIManager.instance.ControlMovement();
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.engine);

        //shipPhotonView.RequestOwnership();  // 소유권 전환
    }

    //ä���� �̵�
    public override void CheckInteractionStopped()
    {
        base.CheckInteractionStopped();
        if (StopInteract && handler!=null)
        {
            Debug.Log("STOP");
            StopControl();
        }
    }
    private void Move() {
        // Move 메소드 호출 전에 소유권 확인
        if (handler != null) //&& shipPhotonView.IsMine)
        {
            photonView.RPC(nameof(MoveShip), RpcTarget.MasterClient, MoveX, MoveY);
        }

        if (PhotonNetwork.IsMasterClient) {
            if (MoveX != 0 || MoveY != 0) {
                shipRb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            }
            else {
                shipRb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            }
            shipRb.velocity = new Vector2(MoveX, MoveY) * moveSpeed;
        }
    }

    [PunRPC]
    public void MoveShip(float moveX, float moveY) {
        MoveX = moveX;
        MoveY = moveY;
    }

    [PunRPC]
    public override void StopControl() {
        if (handler != null) {

            //if (shipPhotonView.IsMine) {
            //    shipPhotonView.TransferOwnership(PhotonNetwork.MasterClient);  // 소유권을 마스터 클라이언트로 변환 : 즉 호스트(서버)에게 소유권을 반납하는 것
            //}
            Debug.Log("stop");
            photonView.RPC(nameof(MoveShip), RpcTarget.MasterClient, 0f, 0f);

            handlerInput.canReceiveInput = true;
            handler = null;
            handlerInput = null;
            UIManager.instance.StopControlMovement();

            base.StopControl();
        }
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime; 



public class ShipController : MonoBehaviourPun, IController
{
  
    public GameObject handler;
    public PlayerInput handlerInput;

   
    protected bool firstFrameChecked;
    private string stopInteractButtonName = "StopInteract";

  

    public bool StopInteract { get; private set; }
    public virtual void StopControl() {
        Debug.Log(gameObject.name);

        //기존 핸들러 입력을 활성화 하고, 초기화하는 코드를 자식 클래스 메소드에 구현
    }

    //상호작용 중지 여부 확인 메소드
    public virtual void CheckInteractionStopped() {
        StopInteract = Input.GetButtonDown(stopInteractButtonName);
        if (firstFrameChecked) {
            StopInteract = false;
            firstFrameChecked = false;
        }
        //기존에 Stopinteract과 로컬 검사를 자식 클래스의 재정의 메소드로 이동 -> 창고과 이동, 공격의 구현이 조금 다르기 때문
        //원래는 protected 접근지정자로 구현되었지만, 오버라이딩을 위해 public으로 재구현
    }

    //모듈 활성화
    public virtual void Activate(GameObject subject) {
        if (handler != null) {
            StopControl();
        }
        handler = subject;
        handlerInput = handler.GetComponent<PlayerInput>();
        handlerInput.canReceiveInput = false;
        //photonView.RequestOwnership();  // 소유권 전환
        firstFrameChecked = true;
        Debug.Log("activate");
        /* 위 로직 흐름 정리
         누군가 핸들을 가지고 있다면, stopControl을 호출하여 기존 조작을 중지
        핸들과 핸들 인풋에 플레이어의 입력 컴포넌트를 장착
        handlerInput.canReceiveInput = false;를 통해 입력 제어
        이후 소유권을 요청하여 객체를 제어할 수 있는 권한을 확보
        클라이언트가 객체의 소유권을 보유하게 되면
        그 객체는 클라이언트의 로컬 객체로 인식되어 조종이 되는 것
        조종하게 되면 그 조종의 결과가 호스트에 의해 동기화 되어야

         */
    }

    //자리 전환 로직 구현 - 미구현
    public void TakeControl(GameObject newController) {
        photonView.RequestOwnership();
    }

    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer) {
        if (targetView != photonView) {
            return;
        }
        photonView.TransferOwnership(requestingPlayer);
        Activate(requestingPlayer.TagObject as GameObject);  // 태그 오브젝트를 활용한 활성화
    }

  //자리 뻇기 및 소유권 이동 로직
}

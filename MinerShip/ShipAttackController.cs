using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//채굴선 공격 조종장치
public class ShipAttackController : ShipController
{


    [SerializeField]
    GameObject normalBulletPrefab, specialBulletPrefab;

    //특수공격 탄약 칸
    [SerializeField]
    private AmmoCompartment ammoCompartment;

    //공격 방향 변경, 공격 모드 변경 인풋
    
    //화살표로 공격방향 전환 키 변경
    //private string clockwiseRotateButtonName = "ClockwiseRotation";
    //private string counterClockwiseRotateButtonName = "CounterClockwiseRotation";

    private string attackModeChangeButtonName = "AttackModeChange";
    private string fireButtonName = "Fire";

    
    private string moveXAxisName = "Horizontal";


    public float MoveX { get; private set; }

    //q
    public bool ClockwiseRotate { get; private set; }
    //e
    public bool CounterClockwiseRotate { get; private set; }
    //tab
    public bool SpecialAttackModeOn { get; private set; }
    //space
    public bool Fire { get; private set; }


    bool isSpecialAttackMode;



    //변경
    public GameObject AttackDirection;
    public float rotateSpeed;
    public Transform firePos;

    

    Transform[] firePoints;

    private int currentFirePointIndex;

    //공격 재사용 대기시간
    private float lastFireTime;
    [SerializeField]
    private float timeBetFire;


    void Update()
    {
        if (handler == null)
        {
            return;
        }

        //ClockwiseRotate = Input.GetButton(clockwiseRotateButtonName);
        //Debug.Log(ClockwiseRotate);
        //CounterClockwiseRotate = Input.GetButton(counterClockwiseRotateButtonName);

        //x축 움직임을 공격 방향 회전으로 사용
        MoveX = Input.GetAxisRaw(moveXAxisName);
        SpecialAttackModeOn = Input.GetButtonDown(attackModeChangeButtonName);
        Fire = Input.GetButton(fireButtonName);

        SwitchAttackDirection();
        SwitchAttackMode();
        Attack();

        CheckInteractionStopped();

    }

    public override void CheckInteractionStopped()
    {
        base.CheckInteractionStopped();
        if (StopInteract)
        {
            Debug.Log("STOP");
            StopControl();
        }

    }

    //공격 방향 변경 - 시계방향 반시계방향
    //순환 이동
    private void SwitchAttackDirection()
    {
        if (MoveX >= 1)
        {
            AttackDirection.transform.eulerAngles = new Vector3(0, 0, AttackDirection.transform.eulerAngles.z - rotateSpeed);
            UIManager.instance.SwitchAttackDirection(AttackDirection.transform.eulerAngles);
        }
        else if (MoveX <= -1)
        {
            AttackDirection.transform.eulerAngles = new Vector3(0, 0, AttackDirection.transform.eulerAngles.z + rotateSpeed);
            UIManager.instance.SwitchAttackDirection(AttackDirection.transform.eulerAngles);
        }
    }

    //공격 모드 변경 
    //공격 모드가 여러가지가 될 것을 대비해 순환 이동으로 구현
    private void SwitchAttackMode()
    {
        if (SpecialAttackModeOn && ammoCompartment.magAmmo>0)
        {
            isSpecialAttackMode = true;
        }
        else if (ammoCompartment.magAmmo == 0) {
            isSpecialAttackMode = false;
        }
        UIManager.instance.AttackModeChange(isSpecialAttackMode, ammoCompartment.magAmmo > 0);
    }


    private void Attack() {
        if (Fire && Time.time > lastFireTime + timeBetFire) {
            lastFireTime = Time.time;

            if ((isSpecialAttackMode && ammoCompartment.magAmmo > 0)||!isSpecialAttackMode) {
                string bulletPrefab1 = specialBulletPrefab.name;
                photonView.RPC("Shoot", RpcTarget.All, isSpecialAttackMode, firePos.position, Quaternion.Euler(AttackDirection.transform.rotation.eulerAngles));
            }
        }
    }

    [PunRPC]
    private void Shoot(bool isSpecialAttackMode, Vector3 pos, Quaternion quaternion) {

        if (isSpecialAttackMode) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.Instantiate(specialBulletPrefab.name, pos, quaternion);

                ammoCompartment.magAmmo -= 1;
            }
            SoundManager.Instance.PlaySfx3D(SoundManager.Sfx3D.specialAttack);
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.Instantiate(normalBulletPrefab.name, pos, quaternion);
            }
            SoundManager.Instance.PlaySfx3D(SoundManager.Sfx3D.normalAttack);
        }

    }

    //[PunRPC]
    //void FireBullet(string bulletName, Vector3 rotation) {
    //    GameObject bulletPrefab = Resources.Load<GameObject>(bulletName);
    //    Instantiate(bulletPrefab, firePos.position, Quaternion.Euler(rotation));
    //}


    //조종장치 가동
    public override void Activate(GameObject subject)
    {
        photonView.RPC("StopControl", RpcTarget.Others);
        base.Activate(subject);
        //photonView.RequestOwnership();  // 소유권 본인에게로 전환
        AttackDirection.transform.eulerAngles = new Vector3(0, 0, 0);
        isSpecialAttackMode = false;
        UIManager.instance.ControlAttack(currentFirePointIndex);
        UIManager.instance.AttackModeChange(isSpecialAttackMode, ammoCompartment.magAmmo>0);
    }

    //조종 중지
    //Rpc로 자신을 제외한 다른 사람들에게 호출하여 자리 뺏기
    [PunRPC]
    public override void StopControl()
    {
        //if (photonView.IsMine)
        //{
        //   photonView.TransferOwnership(PhotonNetwork.MasterClient);  // 소유권을 마스터 클라이언트로 변환
        //}
        if (handler != null) {
            Debug.Log("stop");
            handlerInput.canReceiveInput = true;
            handler = null;
            handlerInput = null;
            base.StopControl();
            UIManager.instance.StopControlAttack();
        }
    }
}
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//채굴선 공격 조종장치
public class ShipAttackController : ShipController
{
    [SerializeField]
    GameObject AttackUI;

    [SerializeField]
    GameObject normalBulletPrefab, specialBulletPrefab;

    //특수공격 탄약 칸
    [SerializeField]
    private AmmoCompartment ammoCompartment;

    //공격 방향 변경, 공격 모드 변경 인풋
    //추후에 인풋을 분리시켜야 할 듯
    private string clockwiseRotateButtonName = "ClockwiseRotation";
    private string counterClockwiseRotateButtonName = "CounterClockwiseRotation";

    private string attackModeChangeButtonName = "AttackModeChange";
    private string fireButtonName = "Fire";

    //q
    public bool ClockwiseRotate { get; private set; }
    //e
    public bool CounterClockwiseRotate { get; private set; }
    //tab
    public bool AttackModeChange { get; private set; }
    //space
    public bool Fire { get; private set; }


    private enum AttackMode
    {
        normalAttack = 0,
        specialAttack = 1
    }
    private AttackMode currentMode;

    //현재 공격모드를 알 수 있는 UI
    [SerializeField]
    GameObject normalAttackModeUI, specialAttackModeUI;
    GameObject[] attackModeUIs;

    //공격모드를 변경할 때마다 컴포넌트를 다시 참조하지 않기 위해서 미리 Start에서 둘다 참조
    //현재 공격모드에 따라 위의 UI의 투명도를 조절하기 위해 가져오는 Image 컴포넌트
    Image normalAttackModeUIImage, specialAttackModeUIImage;
    //구분하지 않고 투명도를 조절하기 위해서 배열로 만들었다
    Image[] attackModeUIImages;

    //공격 방향 별 발사 지점
    [SerializeField]
    private Transform firePointN, firePointNE, firePointE, firePointSE, firePointS, firePointSW, firePointW, firePointNW;

    //공격 방향 표시 UI
    [SerializeField]
    private GameObject fireDirectionUIN, fireDirectionUINE, fireDirectionUIE, fireDirectionUISE, fireDirectionUIS, fireDirectionUISw, fireDirectionUIW, fireDirectionUINW;

    Transform[] firePoints;
    GameObject[] fireDirectionUIs;

    private int currentFirePointIndex;

    //공격 재사용 대기시간
    private float lastFireTime;
    [SerializeField]
    private float timeBetFire;


    void Start()
    {
        currentMode = AttackMode.normalAttack;
        currentFirePointIndex = 0;
        firePoints = new Transform[] { firePointN, firePointNE, firePointE, firePointSE, firePointS, firePointSW, firePointW, firePointNW };
        fireDirectionUIs = new GameObject[] { fireDirectionUIN, fireDirectionUINE, fireDirectionUIE, fireDirectionUISE, fireDirectionUIS, fireDirectionUISw, fireDirectionUIW, fireDirectionUINW };
        attackModeUIs = new GameObject[] { normalAttackModeUI, specialAttackModeUI };
        normalAttackModeUIImage = normalAttackModeUI.GetComponent<Image>();
        specialAttackModeUIImage = specialAttackModeUI.GetComponent<Image>();

        attackModeUIImages = new Image[] { normalAttackModeUIImage, specialAttackModeUIImage };
    }


    void Update()
    {
        if (handler == null || !photonView.IsMine)
        {
            return;
        }

        ClockwiseRotate = Input.GetButtonDown(clockwiseRotateButtonName);
        CounterClockwiseRotate = Input.GetButtonDown(counterClockwiseRotateButtonName);
        AttackModeChange = Input.GetButtonDown(attackModeChangeButtonName);
        Fire = Input.GetButton(fireButtonName);

        SwitchAttackDirection();
        SwitchAttackMode();
        Attack();

        CheckInteractionStopped();

    }

    //공격 방향 변경 - 시계방향 반시계방향
    //순환 이동
    private void SwitchAttackDirection()
    {
        if (ClockwiseRotate)
        {
            fireDirectionUIs[currentFirePointIndex].SetActive(false);
            currentFirePointIndex = ++currentFirePointIndex % firePoints.Length;
        }
        else if (CounterClockwiseRotate)
        {
            fireDirectionUIs[currentFirePointIndex].SetActive(false);
            currentFirePointIndex = (firePoints.Length + --currentFirePointIndex) % firePoints.Length;
        }
        if (!fireDirectionUIs[currentFirePointIndex].activeSelf)
        {
            fireDirectionUIs[currentFirePointIndex].SetActive(true);
        }
    }

    //공격 모드 변경 
    //공격 모드가 여러가지가 될 것을 대비해 순환 이동으로 구현
    private void SwitchAttackMode()
    {
        if (AttackModeChange)
        {
            //스프라이트 적용시키면 new Color(1,1,1,0.2f)로 코드가 간단해짐 지금은 solid color라 복잡
            attackModeUIImages[(int)currentMode].color = new Color(attackModeUIImages[(int)currentMode].color.r, attackModeUIImages[(int)currentMode].color.g, attackModeUIImages[(int)currentMode].color.b, 0.2f);

            currentMode = (AttackMode)((int)++currentMode % System.Enum.GetValues(typeof(AttackMode)).Length);
        }
        if (attackModeUIImages[(int)currentMode].color.a != 1f)
        {
            attackModeUIImages[(int)currentMode].color = new Color(attackModeUIImages[(int)currentMode].color.r, attackModeUIImages[(int)currentMode].color.g, attackModeUIImages[(int)currentMode].color.b, 1f);
        }
    }

    //공격
    //일반공격은 탄약 제한x
    //특수공격은 탄약 제한
    private void Attack()
    {
        if (Fire && Time.time > lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            string prefabName = currentMode == AttackMode.normalAttack ? normalBulletPrefab.name : specialBulletPrefab.name;
            Transform selectedFirePoint = firePoints[currentFirePointIndex];
            // PhotonNetwork.Instantiate를 사용하여 총알 동기화
            PhotonNetwork.Instantiate(prefabName, selectedFirePoint.position, selectedFirePoint.rotation);

            // 특수 공격인 경우 탄약 감소
            if (currentMode == AttackMode.specialAttack && ammoCompartment.magAmmo > 0)
            {
                ammoCompartment.magAmmo--;
            }
        }
    }

    //조종장치 가동
    public override void Activate(GameObject subject)
    {
        base.Activate(subject);
        AttackUI.SetActive(true);
        fireDirectionUIs[currentFirePointIndex].SetActive(true);
    }

    //조종 중지
    //Rpc로 자신을 제외한 다른 사람들에게 호출하여 자리 뺏기
    public override void StopControl()
    {
        base.StopControl();
        AttackUI.SetActive(false);
    }
}

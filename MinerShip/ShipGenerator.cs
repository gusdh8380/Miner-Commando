using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering.Universal;




public class ShipGenerator : ShipController, IPunObservable {
    const float BASE_TEMPERATURE = 20f;
    //발전기 종류
    public enum Generator {
        sheild = 0,
        //speed = 1,
        light = 1,
        cooling = 2,
        heating = 3
    }

    //현재 다루고 있는 발전기(방어를 위한 발전기, 속도를 위한 발전기, 빛을 위한 발전기 기능마다 발전기가 따로 있음)
    Generator currentGenerator;


    private string moveYAxisName = "Vertical";
    private string fireButtonName = "Fire";

    public float MoveY { get; private set; }
    public bool Use { get; private set; }

    //어떤 발전기가 스테이지에서 사용될지 인스펙터에서 선택 가능
    public Generator[] generators;



    private int index = 0;

    //스테이지에서 사용될 발전기 종류가 몇개인지 정해지지 않고 스테이지마다 배열들 개수와 값을 인스펙터에서 설정하도록

    //발전기 별 전력 수치
    //혹시 클라이언트별로 차이가 나면 안되니까
    //데미지처리와 마찬가지로 마스터에서 증가시키거나 감소시키고
    //그 후에 포톤뷰로 연동
    public int[] powerGauge;
    //발전기 별 최대 전력 수치
    [SerializeField]
    private int[] maxPowerGauge;
    //시간에 따라 소모되는 전력량, 스페이바 클릭 당 얻는 전력량
    [SerializeField]
    int powerConsumed, powerGained;

    private float lastGeneratorSelectTime;
    [SerializeField]
    private float timeBetSelectGenerator;

    //쉴드와 속도를 위해
    MinerShipHealth shipHealth;
    ShipMovmentController movementController;
    [SerializeField]
    Light2D shipLight;

    [SerializeField]
    Transform shipPos;
    //[SerializeField]
    //float basicSpeed, enhancedSpeed;


    //히터
    [SerializeField]
    LayerMask playerLayer;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            for (int i = 0; i < powerGauge.Length; i++) {
                stream.SendNext(powerGauge[i]);
            }
        }
        else {
            for (int i = 0; i < powerGauge.Length; i++) {
                powerGauge[i] = (int)stream.ReceiveNext();
                UIManager.instance.UpdatePowerGauge(i, powerGauge[i]);
            }
        }
    }

    private void Start() {
        for (int i = 0; i < maxPowerGauge.Length; i++) {
            UIManager.instance.SetPowerGauge(i, maxPowerGauge[i]);
        }
        shipHealth = FindObjectOfType<MinerShipHealth>();
        movementController = FindObjectOfType<ShipMovmentController>();
    }

    void Update() {
        //발전기를 activate하는 상태가 아니어도 돌아가야되니까 handler null검사 전에 위치

        if (handler == null) {
            return;
        }


        MoveY = Input.GetAxisRaw(moveYAxisName);

        //space key
        Use = Input.GetButtonDown(fireButtonName);
        SelectGenerator();


        RunGenerator();

        CheckInteractionStopped();

    }

    //컴퓨터가 느리든 빠르든 일정속도로 전력 소모하도록 fixedupdate에서
    private void FixedUpdate() {
        if (PhotonNetwork.IsMasterClient) {
            PowerDecrease();
        }
        RunPowerSystem();

    }

    public override void CheckInteractionStopped() {
        base.CheckInteractionStopped();
        if (StopInteract) {
            Debug.Log("STOP");
            StopControl();
        }

    }

    //발전기 선택
    private void SelectGenerator() {
        if (Time.time > lastGeneratorSelectTime + timeBetSelectGenerator && MoveY != 0) {
            UIManager.instance.LeaveGenerator(index);
            if (MoveY < 0) {
                index = ++index % generators.Length;
                currentGenerator = generators[index];
            }
            else {
                index = (generators.Length + --index) % generators.Length;
                currentGenerator = generators[index];
            }
            lastGeneratorSelectTime = Time.time;
            UIManager.instance.SelectGenerator(index);
        }
    }

    //발전기를 돌린다
    //마스터 클라이언트의 powerGauge만 rpc로 증가시킨다.(본인을 포함한 마스터가 아닌 클라이언트는 증가x)
    //이유:
    //포톤뷰로 powerGauage를 연동받는다

    private void RunGenerator() {
        if (Use) {
            photonView.RPC("RpcRunGenerator", RpcTarget.MasterClient, index);
            SoundManager.Instance.PlaySfx(SoundManager.Sfx.electricity);
        }
    }

    [PunRPC]
    private void RpcRunGenerator(int index) {

        powerGauge[index] += powerGained;
        if (powerGauge[index] > maxPowerGauge[index]) {
            powerGauge[index] = maxPowerGauge[index];
        }

    }

    //조종장치 가동
    public override void Activate(GameObject subject) {
        photonView.RPC("StopControl", RpcTarget.Others);
        base.Activate(subject);
        index = 0;
        currentGenerator = generators[index];
        UIManager.instance.ContollGenerator();
    }

    //조종 중지
    //Rpc로 자신을 제외한 다른 사람들에게 호출하여 자리 뺏기
    [PunRPC]
    public override void StopControl() {
        Debug.Log("stop");
        if (handler != null) {
            handlerInput.canReceiveInput = true;
            handler = null;
            handlerInput = null;

            base.StopControl();
            UIManager.instance.StopContollGenerator();
        }
    }

    //시간이 지남에 따라 전력이 소모됨
    //마스터클라이언트에서만 이 함수 실행(update에서)
    //powerGauge는 photonView로 연동
    private void PowerDecrease() {
        for (int i = 0; i < powerGauge.Length; i++) {
            powerGauge[i] -= powerConsumed;
            if (powerGauge[i] < 0) {
                powerGauge[i] = 0;
            }
            UIManager.instance.UpdatePowerGauge(i, powerGauge[i]);
        }
    }

    //충전되면 각 항목에 맞는 기능이 작동
    //마스터클라이언트에서만 이 함수 실행(update에서)
    //이유:
    //쉴드는 채굴선의 OnDamage가 애초에 마스터에서만 실행되기 때문에 다른 클라이언트의 채굴선은 트루로 바꿀 필요 없음
    //스피드는 포톤뷰로 연동하게
    private void RunPowerSystem() {
        for (int i = 0; i < generators.Length; i++) {
            switch (generators[i]) {
                case Generator.sheild: {
                        if (powerGauge[i] > 0) {
                            shipHealth.sheildOn = true;
                        }
                        else {
                            shipHealth.sheildOn = false;
                        }
                        break;
                    }
                //case Generator.speed: {
                //        if (powerGauge[i] > 0) {
                //            movementController.moveSpeed = enhancedSpeed;
                //        }
                //        else {
                //            movementController.moveSpeed = basicSpeed;
                //        }
                //        break;
                //    }
                case Generator.light: {

                        shipLight.pointLightOuterRadius = 6 + (int)powerGauge[i] / 200;

                        break;
                    }
                case Generator.cooling: {
                        if (powerGauge[i] > 0) {
                            RaycastHit2D[] players = Physics2D.BoxCastAll(shipPos.position, new Vector2(10, 10), 0, Vector2.zero, 0, playerLayer);
                            foreach (RaycastHit2D player in players) {
                                PlayerTemperature playerTemperature = player.transform.GetComponent<PlayerTemperature>();
                                playerTemperature.Temperature = Mathf.MoveTowards(playerTemperature.Temperature, BASE_TEMPERATURE ,0.2f);
                                Debug.Log(players.ToString());
                            }
                        }
                        break;
                    }
                case Generator.heating: {
                        if (powerGauge[i] > 0) {
                            RaycastHit2D[] players = Physics2D.BoxCastAll(shipPos.position, new Vector2(10, 10), 0, Vector2.zero, 0, playerLayer);
                            foreach (RaycastHit2D player in players) {
                                PlayerTemperature playerTemperature = player.transform.GetComponent<PlayerTemperature>();
                                playerTemperature.Temperature = Mathf.MoveTowards(playerTemperature.Temperature, BASE_TEMPERATURE, 0.2f);
                                Debug.Log(players.ToString());
                            }
                        }

                        break;
                    }
            }
        }
    }
}

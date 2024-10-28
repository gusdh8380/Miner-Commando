using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionInteraction : MonoBehaviourPunCallbacks {
    public GameObject missionUIPanel;
    public GameObject BasicUI;
    public Text missionText;
    public MissionData missionData;
    public float paragraphDelay = 2f;
    public MissionSummaryUI missionSummaryUI;
    public MissionSummary missionSummary;
    public PhotonView pv;
    public GameLobbyManager gameLobbyManager;
    public MissionData[] missionDatas;
    public MissionSummary[] missionSummaries;

    // Planets UI에 할당된 Animator
    public Animator planetsAnimator;

    private Coroutine missionCoroutine;
    private bool isMissionActive = false;

    private void Start() {
        missionData = missionDatas[(int)GameManager.Instance.currentStage];
        missionSummary = missionSummaries[(int)GameManager.Instance.currentStage];

        if (missionUIPanel == null || BasicUI == null || missionText == null || missionSummaryUI == null || missionSummary == null || planetsAnimator == null) {
            Debug.LogError("MissionInteraction: One or more required components are not set.");
        }
    }

    [PunRPC]
    public void ShowMissionUI() {
        Debug.Log("ShowMissionUI called");
        missionUIPanel.SetActive(true);
        BasicUI.SetActive(false);
        isMissionActive = true;
        missionCoroutine = StartCoroutine(DisplayMissionText());

        // 현재 스테이지에 맞는 애니메이션 설정
        RuntimeAnimatorController animatorController = GameManager.Instance.GetCurrentStageAnimatorController();
        if (animatorController != null) {
            planetsAnimator.runtimeAnimatorController = animatorController;
            planetsAnimator.SetTrigger("ShowPlanet");
        }

        gameLobbyManager.photonView.RPC("SetMissionReceived", RpcTarget.All); // 미션 수락 후 출동 활성화
    }

    public void SkipMission() {
        if (missionCoroutine != null) {
            StopCoroutine(missionCoroutine);
        }
        missionText.text = "";
        missionUIPanel.SetActive(false);
        BasicUI.SetActive(true);
        isMissionActive = false;
        UpdateMissionInfo();
    }

    private IEnumerator DisplayMissionText() {
        missionText.text = "";
        foreach (string paragraph in missionData.missionParagraphs) {
            foreach (char letter in paragraph.ToCharArray()) {
                missionText.text += letter;
                yield return new WaitForSeconds(0.05f); // 타이핑 효과
            }
            yield return new WaitForSeconds(paragraphDelay);
            missionText.text = ""; // 다음 단락을 위해 텍스트 초기화
        }
        missionUIPanel.SetActive(false);
        BasicUI.SetActive(true);
        isMissionActive = false;
        UpdateMissionInfo();
    }

    private void Update() {
        if (isMissionActive && Input.GetKeyDown(KeyCode.P)) {
            SkipMission();
        }
    }

    private void UpdateMissionInfo() {
        missionSummaryUI.missionSummary = missionSummary;
        missionSummaryUI.UpdateMissionSummary();
    }
}

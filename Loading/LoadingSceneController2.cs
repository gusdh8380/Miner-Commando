using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Collections;
using TMPro;

public class LoadingSceneController2 : MonoBehaviourPun {
    public Slider loadingSlider;
    public Text loadingText;
    public Text TIpInfoText; // 행성 정보 텍스트
    public PlanetInfo[] planetInfos; // 행성에 대한 정보
    public PlanetInfo planetInfo;

    public Animator planetsAnimator;

    private void Start() {
        //얼음행성 정보 안넣었기 때문에 시작이 안되서 주석 처리

        planetInfo = planetInfos[(int)GameManager.Instance.currentStage];
        TIpInfoText.text = planetInfo.description;
        StartCoroutine(LoadGameScene());

        // 현재 스테이지에 맞는 애니메이션 설정
        //얼음행성 정보 안넣었기 때문에 시작이 안되서 주석 처리

        RuntimeAnimatorController animatorController = GameManager.Instance.GetCurrentStageAnimatorController();
        if (animatorController != null) {
            planetsAnimator.runtimeAnimatorController = animatorController;
            planetsAnimator.SetTrigger("ShowPlanet");
        }
    }


    IEnumerator LoadGameScene() {
        // 초기 0%에서 1초 대기
        loadingSlider.value = 0;
        loadingText.text = "Loading... 0%";
        yield return new WaitForSeconds(1f);

        // 진행 상황을 단계별로 세분화하여 천천히 증가시킴, 페이크 로딩게이지
        for (float progress = 0.1f; progress < 0.9f; progress += 0.1f) {
            yield return StartCoroutine(UpdateLoadingProgress(progress, 0.5f));
        }

        // 로딩 완료
        yield return StartCoroutine(UpdateLoadingProgress(1f, 1f));


        if (PhotonNetwork.IsMasterClient) {

            yield return new WaitForSeconds(2f); 
            LoadGamePlayScene();
        }

    }

    IEnumerator UpdateLoadingProgress(float progress, float waitTime) {
        loadingSlider.value = progress;
        loadingText.text = $"Loading... {progress * 100}%";
        yield return new WaitForSeconds(waitTime);
    }

    private void LoadGamePlayScene() {
        if (PhotonNetwork.IsMasterClient) {
            // 마스터 클라이언트가 씬 전환 트리거
            PhotonNetwork.LoadLevel(GameManager.Instance.currentStage.ToString());
        }
    }
    


}

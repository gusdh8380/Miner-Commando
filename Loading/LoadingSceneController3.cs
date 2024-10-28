using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Collections;
using TMPro;

public class LoadingSceneController3 : MonoBehaviourPun {
    public Slider loadingSlider;
    public Text loadingText;
    public Text dotLoadingText;

    private void Start() {
        Time.timeScale = 1;

        StartCoroutine(LoadGameScene());
        StartCoroutine(UpdateDotLoadingText());
    }


    IEnumerator LoadGameScene() {
        // 초기 0%에서 1초 대기
        loadingSlider.value = 0;
        loadingText.text = "Loading... 0%";
        yield return new WaitForSeconds(1f);

        // 진행 상황을 단계별로 세분화하여 천천히 증가시킴, 페이크 로딩게이지
        for (float progress = 0.1f; progress < 0.9f; progress += 0.1f) {
            Debug.Log($"Loading progress: {progress * 100}%");
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
            PhotonNetwork.LoadLevel("GameLobby");
        }
    }

    IEnumerator UpdateDotLoadingText() {
        while (true) {
            for (int i = 2; i <= 7; i++) {
                dotLoadingText.text = "함선으로 돌아가는 중" + new string('.', i);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

}

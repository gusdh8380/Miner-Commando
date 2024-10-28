using UnityEngine;

public class BackgroundMover : MonoBehaviour {
    public float speed ; // 배경 이미지 이동 속도
    public float resetPositionY = -5400f; // 배경 이미지가 리셋되는 Y 위치
    public float startPositionY = 0; // 배경 이미지의 시작 Y 위치

    private RectTransform rectTransform;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);
    }

    void Update() {
        // 배경 이미지 이동
        rectTransform.anchoredPosition += Vector2.down * speed * Time.deltaTime;

        // 배경 이미지가 resetPosition에 도달하면 startPosition으로 되돌아감
        if (rectTransform.anchoredPosition.y <= resetPositionY) {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);
        }
    }
}

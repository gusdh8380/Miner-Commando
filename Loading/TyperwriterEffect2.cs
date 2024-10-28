using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterEffect2 : MonoBehaviour {
    public Text missionTextUI;
    public string missionDescription;
    public float typingSpeed = 0.05f;
    public delegate void TypingComplete();
    public event TypingComplete OnTypingComplete;

    public void StartTyping() {
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText() {
        missionTextUI.text = "";
        foreach (char letter in missionDescription.ToCharArray()) {
            missionTextUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        OnTypingComplete?.Invoke(); // 텍스트 출력이 완료되었을 때 이벤트 발생
    }
}

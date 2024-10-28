using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//타이핑 효과 구현
public class TypewriterEffect : MonoBehaviour {
    public Text missionTextUI;
    public string missionDescription;
    public float typingSpeed = 0.05f;

    private void Start() {
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText() {
        missionTextUI.text = "";
        foreach (char letter in missionDescription.ToCharArray()) {
            missionTextUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}

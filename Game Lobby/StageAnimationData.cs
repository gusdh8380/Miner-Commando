using UnityEngine;

[CreateAssetMenu(fileName = "StageAnimationData", menuName = "ScriptableObjects/StageAnimationData", order = 1)]
public class StageAnimationData : ScriptableObject {
    public GameManager.Stage stage;
    public RuntimeAnimatorController animatorController;
}
using UnityEngine;
//스테이지 매니저를 구현..?
[CreateAssetMenu(fileName = "New Mission Data", menuName = "Mission Data", order = 51)]
public class MissionData : ScriptableObject {
    [TextArea]
    public string[] missionParagraphs;
}

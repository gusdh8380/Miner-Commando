using UnityEngine;

[CreateAssetMenu(fileName = "New Mission Summary", menuName = "Mission Summary", order = 52)]
public class MissionSummary : ScriptableObject {
    [TextArea]
    public string missionTitle;
    [TextArea]
    public string missionObjective1;
   
}

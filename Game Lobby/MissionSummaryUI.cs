using UnityEngine;
using UnityEngine.UI;

//플레이어 기본 UI에 미션 요약본 추가

public class MissionSummaryUI : MonoBehaviour {

    public Text missionTitleText;
    public Text missionObjective1Text;
   
    public MissionSummary missionSummary;

    public string DefaultSummaryTitle = "임무";
    public string DefaultSummaryContend = "함장실에서 임무를 받으세요";
    private void Start() {
        // missionSummary가 null이 아닌 경우에만 UpdateMissionSummary 호출
        if (missionSummary != null) {
            UpdateMissionSummary();
        }
        else {
            ClearMissionSummary();
        }
    }

    public void UpdateMissionSummary() {
        if (missionSummary != null) {
            missionTitleText.text = missionSummary.missionTitle;
            missionObjective1Text.text = missionSummary.missionObjective1;
            
        }
        
    }
    public void ClearMissionSummary() {
       
            missionTitleText.text = DefaultSummaryTitle;
            missionObjective1Text.text = DefaultSummaryContend;
        

    }
}
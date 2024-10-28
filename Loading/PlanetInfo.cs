using UnityEngine;

// 게임 플레이씬으로 가는 로딩 씬에서 사용할
// 행성에 대한 정보 데이터

[CreateAssetMenu(fileName = "NewPlanetInfo", menuName = "PlanetInfo")]
public class PlanetInfo : ScriptableObject {
    
    [TextArea]
    public string description;
}

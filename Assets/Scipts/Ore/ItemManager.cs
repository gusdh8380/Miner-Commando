using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
    public static OreData[] ores;

    private void Start() {
        ores = Resources.LoadAll<OreData>("ScriptableDatas");
    }
}

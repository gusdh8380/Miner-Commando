using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Scriptable/OreData", fileName = "Ore Data")]
public class OreData : ScriptableObject
{
    public string oreName;
    public int oreId;
    public int hitsRequired;
    public Sprite minedOreSprite;
    public int minAmount, maxAmount;
}

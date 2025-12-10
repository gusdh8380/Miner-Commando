using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private TMP_Text textCount;
    private OreData _oreData;
    public OreData oreData {
        get { return _oreData; }
        set {
            _oreData = value;
            _oreData = oreData;
            image.color = new Color(1, 1, 1, 1);
        }
    }
    //널값에 따른 동작 구분을 위해 프로퍼티 사용
    private int _count;
    public int count {
        get { return _count; }
        set {
            _count = value;
            image.sprite = oreData.minedOreSprite;
            textCount.text = _count.ToString();
            if (_count <= 0) {
                oreData = null;
                image.color = new Color(1, 1, 1, 0);
                textCount.text = "";
            }
        }
    }
    public int maxCapacity;


    private void Awake() {
        textCount.text = "";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;

public class Ore : InteractableEntity
{
    [SerializeField]
    private OreData oreData;

    private int hitCount;
    private int amount;

    [SerializeField]
    private GameObject minedOrePrefab;

    private void Start() {
        amount = new System.Random().Next(oreData.minAmount, oreData.maxAmount + 1);
    }
    private void Update() {
        if (hitCount >= oreData.hitsRequired) {
            OnMiningComplete();
        }
    }

    public override void Interact(GameObject subject) {
        base.Interact(subject);
        //subject 애니메이션 작동
        //채광 진행도 ui? 
        subject.GetComponent<PlayerInteract>().SwingPickaxe(this);
    }

    public void OnHit() {
        hitCount++;
    }
    private void OnMiningComplete() {

        for (int i = 0; i < amount; i++) {
            Vector2 minedOrePos = (Vector2)transform.position + new Vector2(new System.Random().Next(-2, 3), new System.Random().Next(-2, 3));
            GameObject minedOre = Instantiate(minedOrePrefab, minedOrePos, new Quaternion());
            minedOre.GetComponent<MinedOre>().SetUp(oreData);
            gameObject.SetActive(false);
        }
    }
}

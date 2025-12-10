using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinedOre : InteractableEntity
{
    private OreData oreData;
    private SpriteRenderer spRd;

    private Inventory inventory;

    private void Start() {
        spRd = GetComponent<SpriteRenderer>();
        spRd.sprite = oreData.minedOreSprite;
        inventory = FindObjectOfType<Inventory>();
    }

    public void SetUp(OreData oreData) {
        this.oreData = oreData;
    }

    public override void Interact(GameObject subject) {
        base.Interact(subject);
        if (inventory.AddOre(oreData.oreId)) {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    private List<OreData> ores = new();

    [SerializeField]
    protected Slot[] slots;

#if UNITY_EDITOR
    private void OnValidate() {
        slots = transform.GetComponentsInChildren<Slot>();
    }
#endif



    public bool AddOre(int oreId) {
        foreach (Slot slot in slots) {
            if (slot.oreData != null) {
                if (slot.oreData.oreId == oreId && slot.count < slot.maxCapacity) {
                    slot.count++;
                    return true;
                }
            }

        }
        foreach (Slot slot in slots) {
            if (slot.oreData == null) {
                slot.oreData = ItemManager.ores[oreId];
                slot.count++;
                return true;
            }
        }
        Debug.Log("slot is full");
        return false;

    }

    public void RemoveOre(int slotIndex) {
        slots[slotIndex].count--;

        if (slots[slotIndex].count < 0) {
            slots[slotIndex].count = 0;
        }
    }
    public int GetOreId(int slotIndex) {
        if (slots[slotIndex].oreData != null) {
            return slots[slotIndex].oreData.oreId;
        }
        else {
            //means there is no item in slot
            return -1;
        }
    }

    public bool CheckSlot(int slotIndex, int oreId) {
        if (GetOreId(slotIndex) == oreId && slots[slotIndex].count > 0) {
            RemoveOre(slotIndex);
            return true;
        }
        return false;
    }
}

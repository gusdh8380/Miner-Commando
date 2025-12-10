using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStorageSystem : MonoBehaviour
{
    private ChestStorage chestStorage;
    private Inventory inventory;
    private void Start() {
        chestStorage = FindObjectOfType<ChestStorage>(true);
        inventory = FindObjectOfType<Inventory>();  
    }


    //클라이언트B의 플레이어b가 아이템을 꺼내오는 시나리오
    //CHECKSLOT하는 부분과 AddOre하는 부분을 2개의 rpc 함수로 나눈다
    //CheckSlot을 MASTER CLIENT가 실행하도록 rpc 호출하고 MASTER CLIENT는 CHECKSLOT으로 플레이어가 요청한 아이템이 슬롯에 있는지와 갯수가 0이 넘는지를 확인하고
    //다시 RPC로 모든 클라이언트의 플렝이어 b에게 AddOre를 시킨다. isMined로 해당 플레이어만 AddOre를 실행한다
    public void TakeItem(int slotIndex, int oreId) {
        if(chestStorage.CheckSlot(slotIndex, oreId)) {
            inventory.AddOre(oreId);
        }
    }
}

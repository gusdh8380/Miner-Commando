using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : ShipController {
    private string moveXAxisName = "Horizontal";
    private string moveYAxisName = "Vertical";
    private string fireButtonName = "Fire";

    public enum ChestMode {
        store = 0,
        take = 1
    }
    public ChestMode currentMode;

    private PlayerStorageSystem handlerInventorySystem;

    //input
    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    public bool Use { get; private set; }


    private int slotIndex;
    private int oreId;

    [SerializeField]
    GameObject chestStorageUI;


    ChestStorage chestStorage;
    Inventory inventory;

    GameObject[] selectedInventorySlotUI;
    GameObject[] selectedChestStorageSlotUI;

    [SerializeField]
    private GameObject inventorySlot0, inventorySlot1, inventorySlot2, inventorySlot3;
    [SerializeField]
    private GameObject chestStorageSlot0, chestStorageSlot1, chestStorageSlot2, chestStorageSlot3, chestStorageSlot4, chestStorageSlot5, chestStorageSlot6, chestStorageSlot7;

    private GameObject selectedSlotUI;

    //time between change slot
    private float lastSlotChangeTime;
    [SerializeField]
    private float timeBetChangeSlot;

    private void Start() {
        chestStorage = FindObjectOfType<ChestStorage>(true);
        inventory = FindObjectOfType<Inventory>();
        selectedInventorySlotUI = new[]{ inventorySlot0, inventorySlot1, inventorySlot2, inventorySlot3 };
        selectedChestStorageSlotUI = new[] { chestStorageSlot0, chestStorageSlot1, chestStorageSlot2, chestStorageSlot3 , chestStorageSlot4, chestStorageSlot5, chestStorageSlot6, chestStorageSlot7 };
    }
    void Update() {
        //게임오버에 대한 코드 필요 StageManager


        if (handler == null) {
            return;
        }
        //else if(!handler.photonView.IsMine) return;    null 이면 검사하면 오류나니까


        MoveX = Input.GetAxisRaw(moveXAxisName);
        MoveY = Input.GetAxisRaw(moveYAxisName);

        //space key
        Use = Input.GetButtonDown(fireButtonName);

        ChangeSlot();
        ChangeInOutMode();

        UseChest();


        CheckInteractionStopped();
    }
    public override void Activate(GameObject subject) {
        base.Activate(subject);
        handlerInventorySystem = handler.GetComponent<PlayerStorageSystem>();
        slotIndex = 0;

        chestStorageUI.SetActive(true);
        selectedSlotUI = selectedInventorySlotUI[slotIndex];
        selectedSlotUI.SetActive(true);
        currentMode = ChestMode.store;
    }

    public override void StopControl() {
        base.StopControl();
        handlerInventorySystem = null;
        selectedSlotUI.SetActive(false);
        chestStorageUI.SetActive(false);

    }


    private void ChangeSlot() {
        if (Time.time > lastSlotChangeTime + timeBetChangeSlot && MoveX != 0) {
            switch (currentMode) {
                case ChestMode.store:
                    if (MoveX > 0) {
                        selectedInventorySlotUI[slotIndex].SetActive(false);
                        slotIndex = ++slotIndex % selectedInventorySlotUI.Length;
                        lastSlotChangeTime = Time.time;
                    }
                    else if (MoveX < 0) {
                        selectedInventorySlotUI[slotIndex].SetActive(false);
                        slotIndex = (selectedInventorySlotUI.Length + --slotIndex) % selectedInventorySlotUI.Length;
                        lastSlotChangeTime = Time.time;
                    }
                    selectedSlotUI = selectedInventorySlotUI[slotIndex];
                    selectedSlotUI.SetActive(true);
                    break;
                case ChestMode.take:
                    if (MoveX > 0) {
                        selectedChestStorageSlotUI[slotIndex].SetActive(false);
                        slotIndex = ++slotIndex % selectedChestStorageSlotUI.Length;
                        lastSlotChangeTime = Time.time;
                    }
                    else if (MoveX < 0) {
                        selectedChestStorageSlotUI[slotIndex].SetActive(false);
                        slotIndex = (selectedChestStorageSlotUI.Length + --slotIndex) % selectedChestStorageSlotUI.Length;
                        lastSlotChangeTime = Time.time;
                    }
                    selectedSlotUI = selectedChestStorageSlotUI[slotIndex];
                    selectedSlotUI.SetActive(true);
                    break;
            }
        }
    }

    private void ChangeInOutMode() {
        if (MoveY != 0) {
            switch (currentMode) {
                case ChestMode.store:
                    if (MoveY > 0) {
                        currentMode = ChestMode.take;
                        selectedSlotUI.SetActive(false);

                        slotIndex = 0;
                        selectedSlotUI = selectedChestStorageSlotUI[slotIndex];
                        selectedSlotUI.SetActive(true);
                    }
                    break;
                case ChestMode.take:
                    if (MoveY < 0) {
                        currentMode = ChestMode.store;
                        selectedSlotUI.SetActive(false);

                        slotIndex = 0;
                        selectedSlotUI = selectedInventorySlotUI[slotIndex];
                        selectedSlotUI.SetActive(true);
                    }
                    break;
            }
        }
    }

    private void UseChest() {
        if (Use) {
            switch(currentMode) {               
                case ChestMode.store:
                    oreId = inventory.GetOreId(slotIndex);
                    if (oreId != -1) {
                        if (chestStorage.AddOre(oreId)) {
                            inventory.RemoveOre(slotIndex);
                        };
                    }
                    
                    break;
                case ChestMode.take:
                    oreId = chestStorage.GetOreId(slotIndex);
                    if (oreId != -1) {
                        handlerInventorySystem.TakeItem(slotIndex, oreId);
                    }
                    break;
            }
        }
    }


}

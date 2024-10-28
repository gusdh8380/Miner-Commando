using Photon.Pun;
using UnityEngine;



public class Chest : ShipController
{
    
    private string moveXAxisName = "Horizontal";
    private string moveYAxisName = "Vertical";
    private string fireButtonName = "Fire";


    public enum ChestMode {
        store = 0,
        take = 1
    }

    public ChestMode currentMode;

   
    private PlayerStorageSystem handlerStorageSystem;


    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    public bool Use { get; private set; }

   
    private int slotIndex;
   
    private int oreId;



 
    ChestStorage chestStorage;
    Inventory inventory;






    //time between change slot
    private float lastSlotChangeTime;
    [SerializeField]
    private float timeBetChangeSlot;

    private void Start()
    {
        chestStorage = FindObjectOfType<ChestStorage>(true);
        inventory = FindObjectOfType<Inventory>();

    }
    void Update()
    {
  


        if (handler == null )
        {
            return;
        }


        MoveX = Input.GetAxisRaw(moveXAxisName);
        MoveY = Input.GetAxisRaw(moveYAxisName);

        //space key
        Use = Input.GetButtonDown(fireButtonName);

        ChangeSlot();
        ChangeMode();

        UseChest();


        CheckInteractionStopped();



        //테스트를 위한 스테이지 클리어
        if (Input.GetKeyDown(KeyCode.Alpha9)){
            StageManager.Instance.MissionClear();
            Debug.Log("cheat");
        }
    }
    //나가는 함수 오버라이드
    public override void CheckInteractionStopped()
    {
        base.CheckInteractionStopped(); 

        if (StopInteract)
        {
            Debug.Log("STOP");
            StopControl();
        }
    }
  
    public override void Activate(GameObject subject)
    {
        base.Activate(subject);
       
        handlerStorageSystem = handler.GetComponent<PlayerStorageSystem>();


        slotIndex = 0;


        currentMode = ChestMode.store;

        UIManager.instance.OpenChest();
        SoundManager.Instance.PlaySfx(SoundManager.Sfx.chest);

        // 클라이언트가 창고를 열 때 초기 상태를 동기화
        chestStorage.RequestInitialState();
    }


 
    public override void StopControl()
    {
        //창고는 소유권 분쟁이 없음 : 로컬 검사 안함

        //널검사 안해서 레퍼런스 오류 뜨길래 추가
        if (handler != null) {

            Debug.Log("stop");
            handlerInput.canReceiveInput = true;
            handler = null;
            handlerInput = null;

            base.StopControl();
            handlerStorageSystem = null;
            UIManager.instance.CloseChest();
        }

    }

    //슬롯 변경 로직
    private void ChangeSlot()
    {
        if (Time.time > lastSlotChangeTime + timeBetChangeSlot && MoveX != 0)
        {
            switch (currentMode)
            {
                case ChestMode.store:
                    if (MoveX > 0)
                    {
                        UIManager.instance.LeaveSlot(slotIndex, currentMode);
                        //slotIndex = ++slotIndex % selectedInventorySlotUI.Length;
                        slotIndex = ++slotIndex % 4;
                        lastSlotChangeTime = Time.time;
                    }
                    else if (MoveX < 0)
                    {
                        UIManager.instance.LeaveSlot(slotIndex, currentMode);
                        //slotIndex = (selectedInventorySlotUI.Length + --slotIndex) % selectedInventorySlotUI.Length;
                        slotIndex = (4 + --slotIndex) % 4;

                        lastSlotChangeTime = Time.time;
                    }
                    UIManager.instance.ChangeSlot(slotIndex, currentMode);
                    break;
                case ChestMode.take:
                    if (MoveX > 0)
                    {
                        UIManager.instance.LeaveSlot(slotIndex, currentMode);

                        //slotIndex = ++slotIndex % selectedChestStorageSlotUI.Length;
                        slotIndex = ++slotIndex % 8;
                        lastSlotChangeTime = Time.time;
                    }
                    else if (MoveX < 0)
                    {
                        UIManager.instance.LeaveSlot(slotIndex, currentMode);

                        //slotIndex = (selectedChestStorageSlotUI.Length + --slotIndex) % selectedChestStorageSlotUI.Length;
                        slotIndex = (8 + --slotIndex) % 8;
                        lastSlotChangeTime = Time.time;
                    }
                    UIManager.instance.ChangeSlot(slotIndex, currentMode);
                    break;
            }
        }
    }

    //모드 변경 로직
    private void ChangeMode()
    {
        //if (Time.time > lastSlotChangeTime + timeBetChangeSlot && MoveY != 0 && MoveX == 0)
        //{
        //    switch (currentMode)
        //    {
        //        case ChestMode.store:
        //            if (MoveY > 0)
        //            {
        //                currentMode = ChestMode.take;
        //                slotIndex = 0;
        //                lastSlotChangeTime = Time.time;
        //                UIManager.instance.ChangeChestMode(currentMode);
        //            }
        //            break;
        //        case ChestMode.take:
        //            if (MoveY < 0)
        //            {
        //                currentMode = ChestMode.store;
        //                slotIndex = 0;
        //                lastSlotChangeTime = Time.time;
        //                UIManager.instance.ChangeChestMode(currentMode);
        //            }
        //            break;
        //    }
        //}
    }

    //창고 사용 로직
    private void UseChest()
    {
        if (Use)
        {
            switch (currentMode)
            {
                case ChestMode.store:
                    oreId = inventory.GetOreId(slotIndex);
                    if (oreId != -1)
                    {
                        if (chestStorage.AddOre(oreId))
                        {
                            inventory.RemoveOre(slotIndex);
                            SoundManager.Instance.PlaySfx(SoundManager.Sfx.item);
                        }
                    }

                    break;
                case ChestMode.take:
                    oreId = chestStorage.GetOreId(slotIndex);
                    if (oreId != -1)
                    {
                        handlerStorageSystem.TakeItem(slotIndex, oreId);
                        SoundManager.Instance.PlaySfx(SoundManager.Sfx.item);
                    }
                    break;
            }
        }
    }


}

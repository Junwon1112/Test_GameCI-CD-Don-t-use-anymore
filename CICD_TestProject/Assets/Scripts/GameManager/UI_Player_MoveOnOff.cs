using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 어떤 UI라도 1개 이상 켜지면 플레이어 움직임을 멈추게 하기 위해 전체 UI를 관리하도록 만든 클래스
/// </summary>
public class UI_Player_MoveOnOff : MonoBehaviour
{
    /// <summary>
    /// playerInputSystem에 있는 Inventroy에 UI전용 우클릭 입력이 있어서 전체 UI에 사용하려면 끌어와야 했다. 
    /// 다음에는 전체 UI를 관리하는 InputSystem항목을 만들고 거기서 우클릭 입력을 구현해야 함 (확장성에 대한 고려 필요)
    /// </summary>
    InventoryUI inventoryUI;    

    bool isOnInventoryItemUseConnect = false;

    public CanvasGroup[] canvasGroups; 

    private void Awake()
    {
        inventoryUI = GetComponentInChildren<InventoryUI>();
    }


    /// <summary>
    /// UI가 켜지거나 꺼지면 플레이어가 움직일지 확인
    /// </summary>
    public void IsUIOnOff()    
    {
        uint count = 0;
        for (int i = 0; i < canvasGroups.Length; i++)
        {
            
            if (canvasGroups[i].interactable)
            {
                GameManager.Instance.MainPlayer.input.Disable();
                if(!isOnInventoryItemUseConnect)
                {
                    isOnInventoryItemUseConnect = true;
                    inventoryUI.inventoryControl.Inventory.InventoryItemUse.performed += inventoryUI.OnInventoryItemUse;
                    Debug.Log("OnInventoryItemUseConnect");
                }
                break;
            }
            else
            {
                count++;
                if(count >= canvasGroups.Length)
                {
                    GameManager.Instance.MainPlayer.input.Enable();
                    if(isOnInventoryItemUseConnect)
                    {
                        isOnInventoryItemUseConnect = false;
                        inventoryUI.inventoryControl.Inventory.InventoryItemUse.performed -= inventoryUI.OnInventoryItemUse;
                        Debug.Log("NO OnInventoryItemUseConnect");
                    }
                    
                    
                } 
            }
        }
    }

}

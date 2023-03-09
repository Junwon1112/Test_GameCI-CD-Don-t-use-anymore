using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 속 아이템 슬롯 데이터
/// </summary>
public class ItemSlot  //: MonoBehaviour
{
    /// <summary>
    /// 외부에서 몇번째 슬롯인지 구분하는 슬롯 아이디. 할당전엔 -1값을 할당해 놓음
    /// </summary>
    public int slotID = -1; 

    /// <summary>
    /// 아이템 슬롯의 데이터
    /// </summary>
    ItemData slotItemData;
    uint itemCount;

    /// <summary>
    /// 아이템 슬롯 데이터 프로퍼티
    /// </summary>
    public ItemData SlotItemData
    {
        get
        {
            return slotItemData;
        }
        set
        {
            if(slotItemData != value)
            {
                slotItemData = value;
            }
        }
    }

    /// <summary>
    /// 아이템 갯수에 대한 프로퍼티
    /// </summary>
    public uint ItemCount
    { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    public ItemSlot() { }
    public ItemSlot(ItemData data, uint count)
    {
        slotItemData = data;
        itemCount = count;
    }

    public ItemSlot(ItemSlot newItemSlot)
    {
        slotItemData = newItemSlot.slotItemData;
        itemCount = newItemSlot.itemCount;
    }

    //아이템 슬롯에 아이템 갯수 추가
    //아이템 슬롯에 아이템 갯수 감소
 


    /// <summary>
    /// 아이템 슬롯에 아이템 할당(새로 생성)
    /// </summary>
    /// <param name="newItemData"></param>
    /// <param name="newItemCount"></param>
    public void AssignSlotItem(ItemData newItemData, uint newItemCount = 1 )
    {
        if(IsEmpty())
        {
            SlotItemData = newItemData;
            ItemCount = newItemCount;
            Debug.Log("빈 슬롯에 할당한다");
        }
        
    }

    /// <summary>
    /// 슬롯에 아이템이 존재할 때 갯수 추가
    /// </summary>
    /// <param name="count"></param>
    public void IncreaseSlotItem(uint count = 1)
    {
        if(!IsEmpty())
        {
            if(ItemCount + count <= slotItemData.itemMaxCount)
            {
                ItemCount += count;
                Debug.Log("기존 슬롯에 추가한다");
            }
            else
            {
                ItemCount = (uint)slotItemData.itemMaxCount;
                Debug.Log("기존 슬롯이 꽉차있다");
            }
        }
    }

    /// <summary>
    /// 아이템 슬롯에 있는 아이템 갯수 감소
    /// </summary>
    /// <param name="count"></param>
    public void DecreaseSlotItem(uint count = 1)
    {
        if(ItemCount - count > 0)
        {
            ItemCount -= count;
        }
        else if(ItemCount - count <= 0)
        {
            ItemCount = 0;
            SlotItemData = null;
        }
    }

    /// <summary>
    /// 아이템 슬롯 비우기
    /// </summary>
    public void ClearSlotItem()
    {
        SlotItemData = null;
        ItemCount = 0;
    }



    /// <summary>
    /// 슬롯이 비어있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return (slotItemData == null);
    }
}

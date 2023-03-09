using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리의 데이터 클래스
/// </summary>
public class Inventory : MonoBehaviour
{
    //구현할 친구들
    /*
     * 아이템 인벤
     * -아이템 슬롯 => 슬롯 갯수 정하기, 아이템 데이터와 갯수
     * -다양한 아이템들 => 아이템 팩토리 제작, 아이템 데이터, 데이터를 가진 scriptable Object, 아이템 자체(프리팹이랑 데이터를 가짐) 
     * -아이템 슬롯에 아이템 추가 => 인벤에서 구현
     * -슬롯에서 아이템 제거 => 인벤에서 구현
     * -슬롯에서 아이템 옮기기 => 인벤 내부에서 옮기기 & 창고와 인벤사이에서 옮기기?
     * -아이템 장착 또는 소비 => 
     * -아이템 정렬?
     */

    //인벤에서 해야할것 : 아이템 슬롯들 사이의 상호작용, 전체 슬롯에 대한 작용 => 슬롯사이의 아이템 이동, 전체 슬롯 클리어

    //아이템 슬롯 돌면서 빈자리 찾는 함수랑 아이템을 추가하는 함수 구현해야 됨


    public ItemSlot[] itemSlots;
    private uint slotCount = 6; //슬롯 갯수 6개

    //아이템 드랍시 사용할 아이템 생성용 팩토리
    //ItemFactory itemFactory = new ItemFactory();
    //-----------------
    ItemData potion;

    public ItemSlot this[uint count]    //인덱서를 이용한 프로퍼티
    {
        get
        {
            return itemSlots[count];
        }
        set
        {
            itemSlots[count] = value;
        }
    }

    private void Awake()
    {
        itemSlots = new ItemSlot[slotCount]; //아이템 슬롯 갯수만큼 할당, start에서 실행했다가 inventoryUI에서 실행하는 start함수에서 slot을 불러와야해서 awake로 옮김

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i] = new ItemSlot();
            itemSlots[i].slotID = i;
        }
    }

    /// <summary>
    /// 아이템의 위치를 이동시키는 함수
    /// </summary>
    /// <param name="fromItemSlot">아이템이 이동하기 전 슬롯의 위치</param>
    /// <param name="toItemSlot">아이템이 이동한 후 슬롯의 위치</param>
    /// <param name="count">이동할 아이템 갯수, 파라미터 미작성시 기본값 1을 적용</param>
    public void MoveSlotItem(ItemSlot fromItemSlot, ItemSlot toItemSlot, uint count = 1)
    {
        //from => to에서 to가 null일때
        //from => to가 서로 종류가 같을 때
        //from => to가 서로 종류가 다를 때

        if(fromItemSlot.SlotItemData != null && toItemSlot.SlotItemData == null)    //잘됨
        {
            toItemSlot.SlotItemData = fromItemSlot.SlotItemData;
            toItemSlot.ItemCount = fromItemSlot.ItemCount;


            fromItemSlot.SlotItemData = null;
            fromItemSlot.ItemCount = 0;
            
        }
        else if (fromItemSlot.SlotItemData == toItemSlot.SlotItemData)              //잘됨  
        {
            if(fromItemSlot.ItemCount + toItemSlot.ItemCount <= toItemSlot.SlotItemData.itemMaxCount)
            {
                toItemSlot.ItemCount += fromItemSlot.ItemCount;

                fromItemSlot.SlotItemData = null;
                fromItemSlot.ItemCount = 0;
                
            }
            else if(fromItemSlot.ItemCount + toItemSlot.ItemCount > fromItemSlot.SlotItemData.itemMaxCount)
            {
                uint tempCount = toItemSlot.ItemCount; 
                toItemSlot.ItemCount = (uint)toItemSlot.SlotItemData.itemMaxCount;
                fromItemSlot.ItemCount = (uint)fromItemSlot.SlotItemData.itemMaxCount - tempCount;
                Debug.Log($"{fromItemSlot.SlotItemData.itemName}에서 {toItemSlot.SlotItemData.itemName}으로 이동");
                Debug.Log("전부 옮길 수 없음");
            }
        }
        else if(fromItemSlot.SlotItemData != null && fromItemSlot.SlotItemData != toItemSlot.SlotItemData)   //아이템을 한 종류만 만들어서 아직 확인 못해봄
        {
            ItemSlot tempItemSlot;
            tempItemSlot = new ItemSlot();

            tempItemSlot.SlotItemData = toItemSlot.SlotItemData;
            tempItemSlot.ItemCount = toItemSlot.ItemCount;

            toItemSlot.SlotItemData = fromItemSlot.SlotItemData;
            toItemSlot.ItemCount = fromItemSlot.ItemCount;

            fromItemSlot.SlotItemData = tempItemSlot.SlotItemData;
            fromItemSlot.ItemCount = tempItemSlot.ItemCount;
            Debug.Log("자리바꿈");
        }
        else
        {
            Debug.Log($"프롬이 비어있음");
        }
    }

    /// <summary>
    /// 모든 슬롯을 비우는 메서드
    /// </summary>
    public void ClearInven()    //확인 완료, 모든 슬롯 비움
    {
        foreach(ItemSlot itemSlot in itemSlots)
        {
            itemSlot.SlotItemData = null;
            itemSlot.ItemCount = 0;
        }
    }

    /// <summary>
    /// 특정 슬롯 아이템 필드로 버리기
    /// </summary>
    /// <param name="dropItemSlot">아이템을 버릴 슬롯</param>
    /// <param name="dropCount">버릴 갯수</param>
    public void DropItem(ItemSlot dropItemSlot,uint dropCount)  
    {
        if(dropItemSlot.ItemCount <= dropCount)  //아이템 갯수가 버릴 갯수보다 적거나 같다면
        {
            uint tempCount = dropItemSlot.ItemCount;
            dropItemSlot.DecreaseSlotItem(dropCount);   //decrease함수는 할당된 갯수보다 많이 버리면 저절로 0개가 되게 설정되어있음
            for(int i = 0; i < tempCount; i++)
            {
                ItemFactory.MakeItem(dropItemSlot.SlotItemData.ID, transform.position, Quaternion.identity, true);
            }

        }
        else
        {
            dropItemSlot.DecreaseSlotItem(dropCount);   //decrease함수는 할당된 갯수보다 많이 버리면 저절로 0개가 되게 설정되어있음
            for (int i = 0; i < dropCount; i++)
            {
                ItemFactory.MakeItem(dropItemSlot.SlotItemData.ID, transform.position, Quaternion.identity, true);
            }
        }
        
    }

    /// <summary>
    /// 같은 아이템 슬롯 중 가장 앞쪽 슬롯 리턴 또는 비어있는 슬롯 리턴 (아이템을 추가할 시 사용)
    /// </summary>
    /// <param name="compareItemData">비교할 아이템 데이터</param>
    /// <returns>같은 아이템 슬롯을 리턴</returns>
    public ItemSlot FindSameItemSlotForAddItem(ItemData compareItemData)     
    {
        bool isFindSlot = false;
        ItemSlot returnItemSlot = null;
        for (int i = 0; i < slotCount; i++)
        {
            //찾는 아이템과 같은 아이템 종류고 슬롯에 자리가 있다면(최대 개수보다 적다면)
            if (itemSlots[i].SlotItemData == compareItemData && itemSlots[i].ItemCount < itemSlots[i].SlotItemData.itemMaxCount)    
            {
                Debug.Log($"{itemSlots[i]}가 같은 데이터이다");
                returnItemSlot = itemSlots[i];
                isFindSlot = true;
                return returnItemSlot;
                //break;
            }
            
        }
        if(isFindSlot == false)
        {
            for (int i = 0; i < slotCount; i++)
            {
                //아이템 슬롯이 비어있다면
                if (itemSlots[i].SlotItemData == null && isFindSlot == false)
                {
                    Debug.Log($"{i}번 슬롯이 비어있다");
                    returnItemSlot = itemSlots[i];
                    isFindSlot=true;
                    return returnItemSlot;
                    //break;
                }

            }
        }

        //비어있는 슬롯을 찾지 못했다면 null값을 리턴한다.
        Debug.Log("슬롯이 부족하다");
        return returnItemSlot;


    }

    /// <summary>
    /// 같은 아이템 슬롯 중 가장 앞쪽 슬롯 리턴 또는 비어있는 슬롯 리턴 (아이템을 사용할 시 사용)
    /// </summary>
    /// <param name="compareItemData"></param>
    /// <returns></returns>
    public ItemSlot FindSameItemSlotForUseItem(ItemData compareItemData)
    {
        ItemSlot returnItemSlot = null;
        for (int i = 0; i < slotCount; i++)
        {
            //찾는 아이템과 같은 아이템 종류고 슬롯에 자리가 있다면(최대 개수보다 적다면)
            if (itemSlots[i].SlotItemData.ID == compareItemData.ID)
            {
                Debug.Log($"{itemSlots[i]}가 같은 데이터이다");
                returnItemSlot = itemSlots[i];
                return returnItemSlot;
                //break;
            }

        }

        Debug.Log($"아이템이 없다");
        return returnItemSlot;
    }

    /// <summary>
    /// 아이템이 인벤토리에 들어오면 알맞은 슬롯을 찾아 할당해 주는 함수
    /// </summary>
    /// <param name="takeItemData">들어올 아이템데이터</param>
    /// <param name="count">들어온 갯수</param>
    public void TakeItem(ItemData takeItemData, uint count)
    {
        //아이템이 꽉차면 NULL reference가 뜨는데 findsameitemslot함수에서 null값을 리턴해서 거기서는 slotItemData를 받을수 없어 에러가 나는듯, 하지만 일단 작동은 잘돼서 나중에 수정할 것
        if(FindSameItemSlotForAddItem(takeItemData).SlotItemData != null) //데이터가 null이 아니라면 => 데이터가 비어있지 않음.
        {
            FindSameItemSlotForAddItem(takeItemData).IncreaseSlotItem(count);
        }
        else if(FindSameItemSlotForAddItem(takeItemData).SlotItemData == null)  //데이터가 null일때
        {
            FindSameItemSlotForAddItem(takeItemData).AssignSlotItem(takeItemData, count);
        }
        else //비어있는 슬롯을 찾지못했다면 찾지못함을 표시
        {
            Debug.Log("인벤토리에 할당할수없다");
        }
        //else if(FindSameItemSlot(takeItemData) == null)  //비어있는 슬롯을 찾지못했다면 찾지못함을 표시
        //{
        //    Debug.Log("인벤토리가 꽉 차있다");
        //}
    }

   

    // try catch문을 이용한 에러잡는 함수 연습
    //public void aaa()
    //{ 
    //    try { } //중괄호 안에서 함수를 돌리고
    //    catch (System.Exception e)  //에러를 잡음
    //    {
    //        e.ToString();
    //    }
    //}

}

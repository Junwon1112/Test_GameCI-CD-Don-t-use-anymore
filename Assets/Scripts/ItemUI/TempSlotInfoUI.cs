using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Info창 아이템 이미지로 이용
/// </summary>
public class TempSlotInfoUI : MonoBehaviour
{
    public Image itemImage;                //Image에 프로퍼티로 스프라이트가 존재한다. 

    // 아이템 움직일 떄 사용
    public ItemData takeSlotItemData;   //tempSlot을 발생시킨곳에서 받아온다.
    public uint takeSlotItemCount;      //tempSlot을 발생시킨곳에서 받아온다.

    

    void Awake()
    {
        itemImage = GetComponentInChildren<Image>();
    }

}

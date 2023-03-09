using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// 장비 슬롯에 들어갈 클래스, TempSlotInfoUI를 상속받음, 이것도 Slot클래스를 따로 만들고 상속을 했어야 했다
/// </summary>
public class EquipSlotUI : TempSlotInfoUI
{
    private TextMeshProUGUI takeSlotItemCountText;
    int takeID = -1;
    /// <summary>
    /// 장비창 무기 슬롯 아이디 = 1001 번
    /// </summary>
    public int equipSlotID = 1001;     

    void Awake()
    {
        this.itemImage = GetComponentInChildren<Image>();
        takeSlotItemCountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        ClearTempSlot();
    }

    /// <summary>
    /// 해당 슬롯을 완전히 비우는 함수
    /// </summary>
    public void ClearTempSlot()
    {
        itemImage.color = Color.clear;
        takeSlotItemCountText.alpha = 0;
        takeSlotItemData = null;
        takeSlotItemCount = 0;
    }

    /// <summary>
    /// 해당 슬롯을 입력받은 아이템으로 세팅하는 함수
    /// </summary>
    /// <param name="itemData">어떤 아이템인지 데이터로 받음</param>
    /// <param name="count">몇개나 받을지 받음</param>
    public void SetTempSlotWithData(ItemData itemData, uint count)
    {
        itemImage.sprite = itemData.itemIcon;   
        itemImage.color = Color.white;

        takeSlotItemData = itemData;
        takeSlotItemCount = count;

        takeSlotItemCountText.text = takeSlotItemCount.ToString();
        takeSlotItemCountText.alpha = 1;
    }
}

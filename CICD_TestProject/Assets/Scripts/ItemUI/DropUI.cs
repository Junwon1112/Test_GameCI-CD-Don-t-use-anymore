using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// SplitUI를 상속받은 DropUI, 아이템을 밖에 버릴때 사용되는 클래스
/// </summary>
public class DropUI : SplitUI
{
    private Transform playerTransform;

    protected override void Awake()
    {
        okButton = transform.Find("OKButton").GetComponent<Button>();
        cancelButton = transform.Find("CancelButton").GetComponent<Button>();
        inputField = GetComponentInChildren<TMP_InputField>();
        splitUICanvasGroup = GetComponent<CanvasGroup>();
        inventory = FindObjectOfType<Inventory>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        splitTempSlotSplitUI = GameObject.Find("ItemMoveSlotUI").transform.GetChild(0).GetComponent<TempSlotSplitUI>();   //활성화후 컴포넌트 찾은거 변수에 저장하고
        playerTransform = FindObjectOfType<Player>().transform;
    }

    protected override void Start()
    {
        //inputField.
        inputField.onEndEdit.AddListener(this.CheckRightCount); //스트링타입 리턴받는 함수 실행  => 입력된 숫자가 슬롯의 itemCount보다 크면 itemCount를, 작으면 0을 리턴

        okButton.onClick.AddListener(this.ClickOKButton);
        cancelButton.onClick.AddListener(ClickCancelButton);

    }

    protected override void ClickOKButton()
    {
        splitPossibleCount -= (uint)splitCount;

        for(int i = 0; i < splitCount; i++)
        {
            ItemFactory.MakeItem(splitItemData.ID, playerTransform.position, playerTransform.rotation);
        }

        if(splitPossibleCount > 0)  //현재 버리고 남은 총 갯수가 1개 이상이면 원래 슬롯에 아이템을 다시 만들어 준다.
        {
            inventory.itemSlots[takeID].AssignSlotItem(splitItemData, splitPossibleCount);             //UI와 슬롯 데이터에서는 뺌
            inventoryUI.slotUIs[takeID].SetSlotWithData(splitItemData, splitPossibleCount);
        }

        SplitUIClose();
    }

    /// <summary>
    /// 텍스트에 버릴 갯수 입력시 맞는 숫자를 버리는지 확인하는 함수 
    /// </summary>
    /// <param name="inputText"></param>
    protected override void CheckRightCount(string inputText) //텍스트에 버릴 갯수 입력 시 실행
    {

        //uint tempNum;
        //bool isParsing = uint.TryParse(splitUI.inputCount.text, out tempNum);
        bool isParsing = int.TryParse(inputText, out splitCount);
        if (splitCount > (int)splitPossibleCount)
        {
            splitCount = (int)splitPossibleCount;
        }
        else if (splitCount < 1)
        {
            splitCount = 1;
        }

        inputField.text = splitCount.ToString();
        //inputText = splitCount.ToString();
        //textCount = splitCount.ToString();
        //return textCount;
    }

    public override void ClickCancelButton()
    {
        inventory.itemSlots[takeID].AssignSlotItem(splitItemData, splitPossibleCount);
        inventoryUI.slotUIs[takeID].SetSlotWithData(splitItemData, splitPossibleCount);
        SplitUIClose();
    }
}

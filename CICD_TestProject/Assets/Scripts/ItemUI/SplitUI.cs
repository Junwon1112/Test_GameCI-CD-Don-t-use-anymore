using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 아이템 갯수를 나눌 때 사용하는 UI에 들어가는 클래스
/// </summary>
public class SplitUI : MonoBehaviour
{
    protected Button okButton;
    protected Button cancelButton;
    protected TMP_InputField inputField;
    protected CanvasGroup splitUICanvasGroup;

    /// <summary>
    /// ItemSlotUI에서 받아옴
    /// </summary>
    public ItemData splitItemData;

    /// <summary>
    /// 스플릿 하기 직전에 아이템슬롯UI에서 데이터값을 그대로 할당해 줌
    /// </summary>
    public uint splitPossibleCount = 1;

    /// <summary>
    /// checkRightcount에서 최종적으로 할당해줌
    /// </summary>
    protected int splitCount = 0; 

    public TempSlotSplitUI splitTempSlotSplitUI;

    /// <summary>
    /// 아이템 슬롯과 UI의 ID를 받아올 값
    /// </summary>
    public int takeID = -1; 

    public bool isSplitting = false;

    protected Inventory inventory;
    protected InventoryUI inventoryUI;


    protected virtual void Awake()
    {
        okButton = transform.Find("OKButton").GetComponent<Button>();
        cancelButton = transform.Find("CancelButton").GetComponent<Button>();
        inputField = GetComponentInChildren<TMP_InputField>();
        splitUICanvasGroup = GetComponent<CanvasGroup>();
        inventory = FindObjectOfType<Inventory>();
        inventoryUI = FindObjectOfType<InventoryUI>();
        splitTempSlotSplitUI = GameObject.Find("ItemMoveSlotUI").transform.GetChild(0).GetComponent<TempSlotSplitUI>();   //활성화후 컴포넌트 찾은거 변수에 저장하고
    }

    protected virtual void Start()
    {
        //스트링타입 리턴받는 함수 실행  => 입력된 숫자가 슬롯의 itemCount보다 크면 itemCount를, 작으면 0을 리턴
        inputField.onEndEdit.AddListener(CheckRightCount); 
        
        okButton.onClick.AddListener(ClickOKButton);
        cancelButton.onClick.AddListener(ClickCancelButton);

    }

    /// <summary>
    /// UI를 열었을 때 보이게 만드는 메서드
    /// </summary>
    public void SplitUIOpen()
    {
        splitUICanvasGroup.alpha = 1.0f;
        splitUICanvasGroup.interactable = true;
        splitUICanvasGroup.blocksRaycasts = true;

        //시작하면 나오는 초기값을 제대로 설정해주는 과정 
        CheckRightCount(inputField.text);
    }

    /// <summary>
    /// UI를 열었을 때 보이게 만드는 메서드
    /// </summary>
    public void SplitUIClose()
    {
        splitUICanvasGroup.alpha = 0.0f;
        splitUICanvasGroup.interactable = false;
        splitUICanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 텍스트에 나눌 갯수 입력 시 실행,입력된 숫자가 슬롯의 itemCount보다 크면 itemCount를, 작으면 0을 리턴
    /// </summary>
    /// <param name="inputText"></param>
    protected virtual void CheckRightCount(string inputText) 
    {
        
        //uint tempNum;
        //bool isParsing = uint.TryParse(splitUI.inputCount.text, out tempNum);
        bool isParsing = int.TryParse(inputText, out splitCount);
        if (splitCount > (int)splitPossibleCount-1)
        {
            splitCount = (int)splitPossibleCount-1;
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

    /// <summary>
    /// ok버튼 누를 시 사용될 메서드, 아이템을 나누는 작업 실행
    /// </summary>
    protected virtual void ClickOKButton()
    {
        GameObject.Find("ItemMoveSlotUI").transform.GetChild(0).gameObject.SetActive(true);  //tempSlot을 비활성화 시켰다 부모오브젝트를 통해 찾아서 활성화 시킬것이다.
        
        splitTempSlotSplitUI.SetTempSlotWithData(splitItemData, (uint)splitCount);       //나눌 데이터 tempslot에 전달하고


        isSplitting = true;

        inventory.itemSlots[takeID].DecreaseSlotItem((uint)splitCount);             //UI와 슬롯 데이터에서는 뺌
        inventoryUI.slotUIs[takeID].slotUICount -= (uint)splitCount; ;

        inventoryUI.SetAllSlotWithData();

        SplitUIClose();
    }

    public virtual void ClickCancelButton()
    {
        SplitUIClose();
    }
}

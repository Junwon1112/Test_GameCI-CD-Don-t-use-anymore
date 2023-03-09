using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 아이템 슬롯 UI와 관련된 메서드
/// </summary>
public class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler , IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    /// <summary>
    /// 전체 슬롯에서 몇번째 슬롯인지 말해주는 말해주는 값, UI와 데이터를 받는 슬롯의 ID는 같다. 할당전에 값은 -1값 할당
    /// </summary>
    public int slotUIID = -1;

    /// <summary>
    /// Info창을 생성할 때 slotUI에서 이름이나 설명 등의 정보를 전달해 줘야해서 만듬
    /// </summary>
    public ItemData slotUIData;

    /// <summary>
    /// tempslot에 갯수전달을 위해 만듬
    /// </summary>
    public uint slotUICount = 0;       
    uint splitCount;        //splitUI에서 값을받고 프로퍼티에서 나눌수있는 값으로 변환

    //빈 이미지를 만들고 아이템 데이터를 받았을 때 해당 데이터의 Icon을 이미지로 변환
    Image itemImage;                //Image에 프로퍼티로 스프라이트가 존재한다. 
    TextMeshProUGUI itemCountText;  //UI로 텍스트를 표기하면 UGUI를 사용
    Inventory playerInven;
    InventoryUI playerInvenUI;

    /// <summary>
    /// 인포창이나 tempslotUI를 마우스 위치에서 열기위한 마우스 포지션 변수
    /// </summary>
    Vector2 mousePos = Vector2.zero;

    /// <summary>
    /// 마우스를 슬롯위에 올려놨을 때 몇초후 열지 설정할 변수
    /// </summary>
    float infoOpenTime = 1.0f;          

    bool isDrag = false;
    bool isOnPointer = false;   //마우스가 슬롯위에 올라가 있는지
    bool isInfoOpen = false;

    ItemInfo itemInfo;  //슬롯위에 마우스 올려놨을 때 활성시킬 아이템 인포창 가져오기
    SplitUI splitUI;
    DropUI dropUI;
    
    TempSlotSplitUI tempSlotSplitUI;
   
    private void Awake()
    {
        itemImage = GetComponentInChildren<Image>();
        itemCountText = GetComponentInChildren<TextMeshProUGUI>();
        playerInven = FindObjectOfType<Inventory>();
        playerInvenUI = FindObjectOfType<InventoryUI>();
        itemInfo = FindObjectOfType<ItemInfo>();
        splitUI = GameObject.Find("SplitUI").GetComponent<SplitUI>();  //dropUI가 splitUI를 상속받았는데 findobjectoftype으로 가져오면 dropUI를 받아올수도있다.
        dropUI = GameObject.Find("DropUI").GetComponent<DropUI>();
        //tempSlotSplitUI = FindObjectOfType<TempSlotSplitUI>();    //비활성화 체크해놓으면 Awake에서 찾아도 못찾는다. 비활성화 타이밍이 Awake보다 빠른것 같다. 그래서 아래처럼 찾는다. 
        tempSlotSplitUI = GameObject.Find("ItemMoveSlotUI").transform.GetChild(0).GetComponent<TempSlotSplitUI>();   //활성화후 컴포넌트 찾은거 변수에 저장하고
    }


    private void Update()
    {
        InfoInUpdate();
    }

    /// <summary>
    /// 슬롯의 데이터로 슬롯UI 설정 
    /// </summary>
    /// <param name="itemData">세팅할 아이템 데이터</param>
    /// <param name="count">세팅할 갯수</param>
    public void SetSlotWithData(ItemData itemData, uint count)  
    {
        if(itemData != null && count > 0)    //아이템 데이터가 존재한다면
        {
            slotUIData = itemData;
            slotUICount = count;

            itemImage.color = Color.white;
            itemImage.sprite = itemData.itemIcon;

            itemImage.raycastTarget = true;
            itemCountText.alpha = 1.0f;
            itemCountText.text = count.ToString();
        }
        else
        {
            slotUIData = null;
            slotUICount = count;
            itemImage.color = Color.clear;
            itemCountText.alpha = 0;
        }
        
    }
    /// <summary>
    /// 마우스가 슬롯에 들어간 상태 파악
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData) 
    {
        isOnPointer = true;
    }

    /// <summary>
    /// 마우스가 슬롯에서 나간 상태 파악
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)    
    {
        isOnPointer = false;
    }

    /// <summary>
    /// 아이템 위에 1초이상 멈췄을 때 아이템 인포 창 표시, 인포창이 표시된 상태에서 움직이면 인포창 닫기
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerMove(PointerEventData eventData)  
    {
        if (!isDrag && isOnPointer && slotUIData != null) //드래그 안한 상태(일반적인 상태에서) 슬롯으로 들어가고 안 움직이면 적정시간 후 인포창 열기 
        {
            infoOpenTime = 1.0f;
            
            if(isInfoOpen)
            {
                itemInfo.CloseInfo();
                isInfoOpen = false;
            }
            mousePos = eventData.position;

            //GameObject objOfFindSlot = eventData.pointerCurrentRaycast.gameObject;
            //objOfFindSlot.GetComponent<ItemData>() 



            /*
             * 코루틴은 마우스가 움직일때마다 실행해야 될때마다 실행해야되서 부담스러우므로 update에서 타임체크
             * 마우스가 움직일 때마다 infoOpenTime을 1초로 다시 초기화
             * 0초가 되면 인포 오픈
             */
        }
    }

    /// <summary>
    /// 인포창 표시할 때 실행할 애들
    /// </summary>
    private void SetInfo()  
    {
        itemInfo.infoTempSlotUI.itemImage.sprite = slotUIData.itemIcon;
        itemInfo.infoTransform.position = mousePos;
        itemInfo.OpenInfo();
        itemInfo.infoName.text = slotUIData.itemName;
        itemInfo.itemInformation.text = "No Information";
        isInfoOpen = true;
    }

    /// <summary>
    /// 업데이트에서 실행할 인포창 오픈관련 함수
    /// </summary>
    private void InfoInUpdate() 
    {
        if(!isDrag)
        {
            if (isOnPointer)
            {
                infoOpenTime -= Time.deltaTime;
            }
            if (isOnPointer && !isInfoOpen && infoOpenTime < 0.0f)
            {
                if (slotUIData != null)  //데이터가 있어야 표시한다.
                {
                    SetInfo();
                }
            }
        }
        
    }

    /// <summary>
    /// 클릭은 누르고 떼어져야 클릭이다, 아이템 클릭시 실행시킬 메서드
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)  
    {
        //shift랑 같이 눌러서 아이템 분할
        //shift와 함께 눌렀을 때 => splitUI등장, Keyboard함수는 inputsystem을 넣어야만 활용가능, 나누는 도중에 shift클릭을 한건 아닌지 확인
        if (Keyboard.current.leftShiftKey.ReadValue() > 0 && !splitUI.isSplitting) 
        {
            if (slotUICount < slotUIData.itemMaxCount +1 && slotUICount > 1)    //데이터를 나눌만한지 확인
            {
                splitUI.splitItemData = slotUIData;
                splitUI.splitPossibleCount = slotUICount;   //현재 슬롯이 가지고있는 갯수를 전해줌, splitPossibleCount는 splitUI내에서 실제 적용할 splitCount로 변환
                splitUI.takeID = slotUIID;                  //현재 해당 슬롯의 ID를 전달해 어떤 슬롯인지 구분
                splitUI.SplitUIOpen();
                //여기부터

            }

        }
        //------------------Split후 OK를 눌러 마우스에 TempSlot이 활성화된 상태----------------------------------------------------
        else if(splitUI.isSplitting)    //그냥 눌렀을 떄 => tempSlot이 활성화 된 상태인지 확인 && 올바른 위치인지 확인
        {
            if (slotUIData == null)
            {
                slotUIData = splitUI.splitTempSlotSplitUI.takeSlotItemData;     //tempslot의 데이터를 넘겨받기
                slotUICount = splitUI.splitTempSlotSplitUI.takeSlotItemCount;   //tempslot의 데이터의 갯수를 넘겨받기
                playerInven.itemSlots[this.slotUIID].AssignSlotItem(slotUIData, slotUICount);    //원본 슬롯에도 데이터와 갯수 전달
                splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                splitUI.isSplitting = false;
                splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                playerInvenUI.SetAllSlotWithData(); //새로바뀐값 새로고침
            }
            else if(slotUIData == splitUI.splitTempSlotSplitUI.takeSlotItemData)  //tempslot의 데이터와 동일한지 확인
            {
                //내꺼와 나눠져서옮겨오는 걸 합친게 최대 갯수보다 적은 경우
                if (splitUI.splitTempSlotSplitUI.takeSlotItemCount + slotUICount < splitUI.splitTempSlotSplitUI.takeSlotItemData.itemMaxCount)   
                {
                    slotUICount += splitUI.splitTempSlotSplitUI.takeSlotItemCount;   //tempslot의 데이터의 갯수를 합치기
                    playerInven.itemSlots[this.slotUIID].IncreaseSlotItem(splitUI.splitTempSlotSplitUI.takeSlotItemCount);    //원본 슬롯에도 갯수 합치기
                    splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                    splitUI.isSplitting = false;
                    splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                    playerInvenUI.SetAllSlotWithData(); //새로바뀐값 새로고침
                }
                else //내꺼와 나눠져서옮겨오는 걸 합친게 최대 갯수보다 많은 경우
                {
                    uint remainCount;   //remainCount = 최대값까지 필요한 갯수
                    uint newTempSlotCount;
                    remainCount = (uint)splitUI.splitTempSlotSplitUI.takeSlotItemData.itemMaxCount - slotUICount;   //최대값까지 필요한 갯수 저장
                    newTempSlotCount = splitUI.splitTempSlotSplitUI.takeSlotItemCount - remainCount;  //전해주는 양만큼 빼주고 변수에 해당 값 저장
                    slotUICount = (uint)slotUIData.itemMaxCount;       //=> 최대값만큼 전달받았기 때문에 최대값만큼 저장
                    playerInven.itemSlots[this.slotUIID].IncreaseSlotItem(remainCount); //아이템 슬롯도 최대값만큼 저장

                    //최대갯수를 주고 원래 있던 슬롯에 남은 갯수 돌려주는 작업
                    playerInvenUI.slotUIs[splitUI.takeID].slotUICount += newTempSlotCount;      //원래있던 슬롯UI에 남은 갯수 돌려줌
                    playerInven.itemSlots[splitUI.takeID].IncreaseSlotItem(newTempSlotCount);   //원래있던 슬롯에 남은 갯수 돌려줌
                    splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                    splitUI.isSplitting = false;
                    splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                    playerInvenUI.SetAllSlotWithData(); //새로바뀐값 새로고침

                }


            }


            //1. 빈슬롯 일경우 그냥 넣기
            //2. 같은 데이터 타입이고 합쳤을 때 maxcount를 안넘는 경우
            //3. 같은 데이터 타입이고 합쳤을 때 maxcount를 넘는 경우 -> 남는 건 원래 슬롯으로 보낸다.
            //4. 다른 경우 아무일도 일어나지 않는다.

            //현재 에러 : 아이템을 한번 나눠서 할당하는건 가능한데 나눠진거로 다시 나누기 위해 스플릿 창열고(여기까지 잘됨) ok누르면 에러남
            //해결, 초기화 할때 Image를 null로 만들어서 생긴에러였음
        }


        /*
         * -분할 작업에 관하여-
         * splitUI를 킬때는 Shift와 함께 눌러야한다.
         * 그냥 클릭시 현재 SplitUI에서 OK를 누른 후 다른 슬롯을 선택해야하는 상황인지 확인하고 
         * 아니라면 아무일도 일어나지 않고 
         * 맞다면 클릭한 위치가 아이템 슬롯인지 확인하고 빈슬롯이거나 같은 슬롯인지 확인한다. 
         * 만약 잘못된 슬롯이거나 잘못된 위치를 찍으면 원래 슬롯에 합쳐진다.
         * 아이템 데이터 종류와 나눠지는 갯수에 따라 아이템을 새로 할당하거나 추가한다.   
         * 
         * -SplitUI에 대해-
         * 클릭하면 해당 슬롯의 아이템 갯수가 2이상 인지 확인하고 맞다면 SplitUI를 커서위치에 등장시킨다.
         * 인풋 필드는 1이상 아이템 갯수 이하의 값으로 설정이 된다.
         * ok버튼을 누르면 SplitUI를 닫고 기존 데이터에서 해당 하는 수만큼의 갯수를 빼고 해당하는 갯수의 데이터를 가진 tempSlotUI를 만든다.
         * cancelButton을 누르면 SplitUI만 닫는다.
         */
    }


    //------------------OnDrag를 사용하지 않더라도 인터페이스 상속을 안받으면 BiginDrag가 작동하지 않는다.-----------------------------

    /// <summary>
    /// 아이템 이동이나 드랍 등을 하기위해 드래그 시작 지점 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)     
    {
        if(slotUIData != null)
        {
            isDrag = true;

            //FindObjectOfType<TempSlotSplitUI>().gameObject.SetActive(true);   //비활성화돼서 바로 찾는건 안됨, 아래처럼 부모를 찾은뒤 그 자식을 찾는형식으로 찾아야함

            GameObject.Find("ItemMoveSlotUI").transform.GetChild(0).gameObject.SetActive(true); //tempSlot을 비활성화 시켰다 부모오브젝트를 통해 찾아서 활성화 시킬것이다.

            tempSlotSplitUI.SetTempSlotWithData(slotUIData, slotUICount);       //이동할 데이터 tempslot에 전달하고

            playerInven.itemSlots[slotUIID].ClearSlotItem();             //UI와 슬롯 데이터에서는 데이터와 갯수를 뺌
            slotUICount = 0;

            playerInvenUI.SetAllSlotWithData();

        }

        /*
         * 드래그를 시작하면 tempslotUI가 드래그를 시작한곳의 이미지를 가져와서 마우스 위치로 update된다.
         */
    }

    /// <summary>
    /// 이때 호출되는 OnEndDrag함수는 처음에 OnBiginDrag가 있던 슬롯의 함수다.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)       //아이템 이동이나 드랍 등을 하기 위한 드래그 도착 지점
    {
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;
        if (obj != null)    //일단 마우스를 땐 곳이 레이캐스트가 있는곳 = 인벤토리 내부인 경우
        {
            //Debug.Log($"{obj.name}");
            ItemSlotUI targetItemSlotUI = obj.GetComponent<ItemSlotUI>();

            if (targetItemSlotUI != null) //가져온 오브젝트가 아이템 슬롯UI가 있는곳이라면 = ItemSlotUI라면
            {
                if(targetItemSlotUI.slotUIData == null)    //놓은 슬롯이 빈슬롯이라면
                {
                    targetItemSlotUI.SetSlotWithData(tempSlotSplitUI.takeSlotItemData, tempSlotSplitUI.takeSlotItemCount);
                    playerInven.itemSlots[targetItemSlotUI.slotUIID].AssignSlotItem(tempSlotSplitUI.takeSlotItemData, tempSlotSplitUI.takeSlotItemCount);//원본 슬롯에도 데이터와 갯수 전달
                    splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                    splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                    playerInvenUI.SetAllSlotWithData(); //합치고 난뒤 전부 데이터가 제대로 들어갔는지 확인하기 위해 데이터로 슬롯을 바꿔준다
                }
                else    //빈슬롯이 아니고 아이템이 존재하는 슬롯이라면
                {
                    if(targetItemSlotUI.slotUIData == tempSlotSplitUI.takeSlotItemData &&   //옮기는 데이터 종류가 같고 //2개의 합이 maxCount보다 작거나 같은경우엔 합친다
                        targetItemSlotUI.slotUICount + tempSlotSplitUI.takeSlotItemCount < targetItemSlotUI.slotUIData.itemMaxCount +1 ) 
                    {
                        targetItemSlotUI.slotUICount += tempSlotSplitUI.takeSlotItemCount;
                        playerInven.itemSlots[targetItemSlotUI.slotUIID].IncreaseSlotItem(tempSlotSplitUI.takeSlotItemCount);    //원본 슬롯에도 데이터와 갯수 전달
                        targetItemSlotUI.SetSlotWithData(targetItemSlotUI.slotUIData, targetItemSlotUI.slotUICount);    //자기 자신의 갯수가 바뀌었으니 해당데이터로 UI표기 최신화

                        splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                        splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                        playerInvenUI.SetAllSlotWithData(); //합치고 난뒤 전부 데이터가 제대로 들어갔는지 확인하기 위해  데이터로 슬롯을 바꿔준다
                    }
                    else    //옮기는 데이터 종류와 다른 슬롯이거나 같더라도 합쳐야하는데 2개의 합이 maxCount보다 많을 경우 => 데이터를 서로 바꾼다
                    {
                        //두 아이템의 자리 옮기기
                        playerInven.itemSlots[this.slotUIID].AssignSlotItem(targetItemSlotUI.slotUIData, targetItemSlotUI.slotUICount);    //원본 슬롯에 데이터와 갯수 전달
                        SetSlotWithData(targetItemSlotUI.slotUIData, targetItemSlotUI.slotUICount);  //temp슬롯으로 데이터를 보내 비어있는 자기 자신에 상대 슬롯의 값 먼저 할당

                        //원본 슬롯에 데이터와 갯수 전달
                        playerInven.itemSlots[targetItemSlotUI.slotUIID].AssignSlotItem(tempSlotSplitUI.takeSlotItemData, tempSlotSplitUI.takeSlotItemCount);    
                        targetItemSlotUI.SetSlotWithData(tempSlotSplitUI.takeSlotItemData, tempSlotSplitUI.takeSlotItemCount);
                        
                        splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                        splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                        playerInvenUI.SetAllSlotWithData(); //합치고 난뒤 전부 데이터가 제대로 들어갔는지 확인하기 위해 데이터로 슬롯을 바꿔준다
                    }
                }

            }
            else    //아이템 슬롯UI가 아니라면
            {
                SetSlotWithData(tempSlotSplitUI.takeSlotItemData, tempSlotSplitUI.takeSlotItemCount);
                playerInven.itemSlots[this.slotUIID].AssignSlotItem(slotUIData, slotUICount);    //원본 슬롯에도 데이터와 갯수 전달
                splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
                splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기

                playerInvenUI.SetAllSlotWithData(); //합치고 난뒤 전부 데이터가 제대로 들어갔는지 확인하기 위해
            }
        }
        else //raycast받는 게임 오브젝트가 아예없을 때 => Inventory밖에 버렸을 때 = 아이템 드롭
        {

            dropUI.splitItemData = tempSlotSplitUI.takeSlotItemData;
            dropUI.splitPossibleCount = tempSlotSplitUI.takeSlotItemCount;   //현재 슬롯이 가지고있는 갯수를 전해줌, splitPossibleCount는 splitUI내에서 실제 적용할 splitCount로 변환
            dropUI.takeID = slotUIID;                                      //현재 해당 슬롯의 ID를 전달해 어떤 슬롯인지 구분
            splitUI.splitTempSlotSplitUI.ClearTempSlot();                   //tempSlot은 역할은 다했으니 초기화
            splitUI.splitTempSlotSplitUI.gameObject.SetActive(false);       //처리 다했으면 tempslotUI끄기
            
            dropUI.SplitUIOpen();
            //여기부터

            
        }




        //Debug.Log($"{slotUIID}");



        isDrag = false;

        /*
         * 1.
         * 만약 tempslotUI의 이미지와 드래그가 끝나는 곳의 이미지가 같다면 아이템 갯수를 확인해서 아이템을 이동시킨다.
         * 만약 이동시킨 아이템 갯수의 합이 최대 갯수보다 많다면 몇개를 이동시킬지 설정하는 창을 만들고 합쳤을때의 최대갯수보다 적게 설정하게 만든다.
         * 이동시킨 아이템갯수의 합이 최대갯수보다 적다면 물어보지 않고 바로 이동한다
         * 아이템 갯수가 꽉차있다면 위치를 바꾼다.
         * 
         * 2.만약 tempslotUI의 이미지와 드래그가 끝나는 곳의 이미지가 다르고 NULL이 아니라면 아이템의 위치를 바꾼다.
         *3.만약 끝나는 곳의 이미지가 NULL이라면 아이템을 해당 위치로 이동한다.
         *4.만약 끝나는 곳이 슬롯이 아니고 인벤토리와 슬롯 테두리라면 아무일도 일어나지 않는다.(기존 슬롯위치로 돌아간다.)
         *5.만약 끝나는 곳이 슬롯이나 인벤토리와 슬롯 테두리가 아니라면(외부라면) 아이템 스플릿 창이 만들어지며 아이템을 드랍한다.
         */
    }

    /// <summary>
    /// onbigindrag와 onenddrag가 작동되기 위해서는 ondrag가 필요하다
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        
    }


}

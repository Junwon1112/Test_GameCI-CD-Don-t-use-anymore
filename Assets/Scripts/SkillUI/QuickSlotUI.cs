using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 각각의 퀵슬롯의 동작에 대한 클래스
/// </summary>
public class QuickSlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    /// <summary>
    /// 퀵슬롯을 확인하는 Id, 퀵슬롯 아이디는 2000번 부터 시작
    /// </summary>
    public int quickSlotID = -1;    
    public SkillData quickSlotSkillData;
    Image skillImage;
    TempSlotSkillUI tempSlotSkillUI;
    public SkillUse skillUse;


    private void Awake()
    {
        skillImage = GetComponent<Image>();
        tempSlotSkillUI = FindObjectOfType<TempSlotSkillUI>();
        skillUse = GetComponentInChildren<SkillUse>();

    }

    private void Start()
    {
        skillImage.color = Color.clear;
        //skillUse = new SkillUse();
    }


    //public void SkillUseInitiate()
    //{
    //    skillUse = new SkillUse();
    //}

    /// <summary>
    /// 퀵슬롯에 스킬을 세팅하는 메서드
    /// </summary>
    /// <param name="skillData">퀵슬롯에 세팅할 데이터</param>
    public void QuickSlotSetData(SkillData skillData = null)    
    {
        if(skillData != null)
        {
            quickSlotSkillData = skillData;
            skillImage.sprite = skillData.skillIcon;

            skillImage.color = Color.white;
        }
        else
        {
            quickSlotSkillData = null;
            skillImage.sprite = null;

            skillImage.color = Color.clear;
        }
        
    }

    /// <summary>
    /// 퀵슬롯간에 스킬데이터 교환시 사용, 드래그 시작시 실행될 메서드
    /// </summary>
    /// <param name="eventData"></param>
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if(quickSlotSkillData != null)
        {
            GameObject.Find("SkillMoveSlotUI").transform.GetChild(0).gameObject.SetActive(true);
            tempSlotSkillUI.SetTempSkillSlotUIData(quickSlotSkillData);
        } 
    }

    /// <summary>
    /// 퀵슬롯간에 스킬데이터 교환시 사용, 드래그 끝낼 시 실행될 메서드
    /// </summary>
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerCurrentRaycast.gameObject;
        QuickSlotUI otherQuickSlotUI = obj.GetComponent<QuickSlotUI>();

        if (otherQuickSlotUI != null && otherQuickSlotUI != this)     //퀵슬롯 안찍었으면 QuickSlotUI컴포넌트가 어차피 없을꺼니까 퀵슬롯을 찍었다면 이라는 뜻
        {
            if(otherQuickSlotUI.quickSlotSkillData == null)     //이동할 슬롯이 비어있다면
            {
                otherQuickSlotUI.QuickSlotSetData(tempSlotSkillUI.tempSkillData);   //이동할 퀵슬롯을 채우고
                QuickSlotSetData();     //현재 슬롯을 비운다.
            }
            else    //이동할 슬롯이 다른 스킬로 되어있다면
            {
                SkillData tempSkillData = new SkillData();  //저장용 임시 데이터

                tempSkillData = otherQuickSlotUI.quickSlotSkillData;    //임시 스킬데이터에 덮어씌울 스킬데이터 저장하고

                otherQuickSlotUI.QuickSlotSetData(tempSlotSkillUI.tempSkillData);   //이동할 퀵슬롯을 현재 슬롯 데이터로 바꾸고
                
                QuickSlotSetData(tempSkillData);     //현재 슬롯을 임시 데이터로 바꾼다.
            }
        }

        tempSlotSkillUI.transform.gameObject.SetActive(false);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
    }





}

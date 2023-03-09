using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬창 UI에 대한 클래스
/// </summary>
public class SkillUI : MonoBehaviour
{
    PlayerInput skillWindowControl;

    public List<SkillData> skillDatas;
    SkillSlotUI[] skillSlotUIs;
    CanvasGroup skillCanvasGroup;
    public bool isSkillWindowOff = true;
    UI_Player_MoveOnOff ui_OnOff;

    Button skillCloseButton;
    

    private void Awake()
    {
        skillSlotUIs = GetComponentsInChildren<SkillSlotUI>();
        skillWindowControl = new PlayerInput();
        skillCanvasGroup = GetComponent<CanvasGroup>();
        ui_OnOff = GetComponentInParent<UI_Player_MoveOnOff>();
        skillCloseButton = transform.Find("CloseButton").GetComponent<Button>();
    }

    private void OnEnable()
    {
        skillWindowControl.Enable();
        skillWindowControl.Skill.SkillWindowOnOff.performed += OnSkillWindowOnOff;
    }


    private void OnDisable()
    {
        skillWindowControl.Skill.SkillWindowOnOff.performed -= OnSkillWindowOnOff;
        skillWindowControl.Disable();
    }

    private void Start()
    {
        skillCloseButton.onClick.AddListener(OnSkillOnOffSetting);

        for(int i = 0; i < skillSlotUIs.Length; i++)
        {
            skillSlotUIs[i].skillData = skillDatas[i];
            skillSlotUIs[i].SetSkillUIInfo();
        }
        
    }

    private void OnSkillWindowOnOff(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSkillOnOffSetting();
        
    }

    private void OnSkillOnOffSetting()
    {
        if (isSkillWindowOff)
        {
            isSkillWindowOff = false;

            skillCanvasGroup.alpha = 1;
            skillCanvasGroup.interactable = true;
            skillCanvasGroup.blocksRaycasts = true;

            ui_OnOff.IsUIOnOff();
        }
        else
        {
            isSkillWindowOff = true;

            skillCanvasGroup.alpha = 0;
            skillCanvasGroup.interactable = false;
            skillCanvasGroup.blocksRaycasts = false;

            ui_OnOff.IsUIOnOff();
        }
    }

}

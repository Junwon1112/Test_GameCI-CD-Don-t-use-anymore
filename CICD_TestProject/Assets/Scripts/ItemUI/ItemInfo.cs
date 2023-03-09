using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ItemInfo창의 동작을 관리하는 클래스
/// </summary>
public class ItemInfo : MonoBehaviour
{
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public TextMeshProUGUI infoName;           
    public TextMeshProUGUI itemInformation; 
    public CanvasGroup infoCanvasGroup;
    public RectTransform infoTransform;
    public TempSlotInfoUI infoTempSlotUI;

    private void Awake()
    {
        infoName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        itemInformation = transform.Find("Information").GetComponent<TextMeshProUGUI>();
        infoCanvasGroup = GetComponent<CanvasGroup>();
        infoTransform = GetComponent<RectTransform>();
        infoTempSlotUI = FindObjectOfType<TempSlotInfoUI>();
    }

    /// <summary>
    /// UI가 열렸을 때 실행될 메서드
    /// </summary>
    public void OpenInfo()
    {
        infoCanvasGroup.alpha = 1.0f;
        infoCanvasGroup.blocksRaycasts = true;
        infoCanvasGroup.interactable = true;
    }

    public void CloseInfo()
    {
        infoCanvasGroup.alpha = 0.0f;
        infoCanvasGroup.blocksRaycasts = false;
        infoCanvasGroup.interactable = false;
    }
}

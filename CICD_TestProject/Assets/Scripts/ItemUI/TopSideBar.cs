using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 위쪽 TopSideBar와 관련된 메서드, 드래그 할 시 마우스를 따라다니는 등
/// </summary>
public class TopSideBar : MonoBehaviour,  IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //Button topSideBarButton;
    RectTransform parentRectTransform;

    private void Awake()
    {
        //topSideBarButton = GetComponent<Button>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 드래그를 시작 할떄 해당 위치를 기준으로 인벤토리가 움직일 수 있게하는 메서드, 클릭시 피봇값을 바꿔 해당 위치 기준으로 움직임 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)  //눌렀을 때 해당 위치를 pivot값으로 설정하는 함수
    {
        float absoluteMinPosition_x = (parentRectTransform.position.x - parentRectTransform.rect.width * (parentRectTransform.pivot.x));
        float absoluteMinPosition_y = (parentRectTransform.position.y - parentRectTransform.rect.height * (parentRectTransform.pivot.y));

        parentRectTransform.pivot = new Vector2((eventData.position.x - absoluteMinPosition_x) / parentRectTransform.rect.width,
                                                    (eventData.position.y - absoluteMinPosition_y) / parentRectTransform.rect.height);

        parentRectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    /// <summary>
    /// 마우스가 움직일 때마다 호출
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)  
    {
        parentRectTransform.position = eventData.position;
    }
}

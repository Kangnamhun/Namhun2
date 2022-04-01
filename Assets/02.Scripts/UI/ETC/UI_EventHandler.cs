using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//각 오브젝트에 붙여서 개인 제어 가능하다
public class UI_EventHandler : MonoBehaviour, /*IBeginDragHandler,*/ IDragHandler,IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler // 이벤트 탐지 스크립트 
{
    public Action<PointerEventData> OnBeginDragHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnPointerHandler = null;
    public Action<PointerEventData> OnPointerExitHandler = null;
    //public void OnBeginDrag(PointerEventData eventData) //드래그가 시작되거나 끝날때
    //{
    //    if (OnBeginDragHandler != null)
    //        OnBeginDragHandler.Invoke(eventData); // OnBeginDragHandler가 들어왔을때 eventData 실행
    //}

    public void OnDrag(PointerEventData eventData) // 드래그 중
    {
    //       transform.position = eventData.position; //드래그해서 움직이기 가능
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData); //  OnDragHandler 들어왔을때 eventData 실행
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerHandler != null)
            OnPointerHandler.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitHandler != null)
            OnPointerExitHandler.Invoke(eventData);
    }
}

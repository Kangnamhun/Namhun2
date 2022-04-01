using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseInputManager
{   
    
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false;
    float _pressedTime = 0;

    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject()) // 유아이를 가르킨다면 리턴 -> 게임오브젝트가 아니라면 리턴
            return;

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0)) //PointerDown ->Press
            {
                if (!_pressed) // 한번도 눌렀던적이 없었는데 눌렀을때 들어온거라면
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown); //델리게이트로 이벤트호출
                    _pressedTime = Time.time; // 누른시간 경과 체크
                    
                }
                
                MouseAction.Invoke(Define.MouseEvent.Press); 
                _pressed = true;
            }
            else if(Input.GetMouseButton(1))
            {
                if (!_pressed) // 한번도 눌렀던적이 없었는데 눌렀을때 들어온거라면
                {
                  
                    MouseAction.Invoke(Define.MouseEvent.PointerRightDown); //델리게이트로 이벤트호출
                    _pressedTime = Time.time; // 누른시간 경과 체크
                }
                
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed) //Click ->PointerUp(좀 오래 눌러있다가 뗐을 떄)
                {
                    if (Time.time < _pressedTime + 0.2f) //0.2초이내에 뗐을 때
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false; // pressed값 초기화
                _pressedTime = 0; //뗀 이후 초기화
            }
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}

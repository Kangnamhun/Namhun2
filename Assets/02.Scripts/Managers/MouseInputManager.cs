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
        if (EventSystem.current.IsPointerOverGameObject()) // �����̸� ����Ų�ٸ� ���� -> ���ӿ�����Ʈ�� �ƴ϶�� ����
            return;

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0)) //PointerDown ->Press
            {
                if (!_pressed) // �ѹ��� ���������� �����µ� �������� ���°Ŷ��
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown); //��������Ʈ�� �̺�Ʈȣ��
                    _pressedTime = Time.time; // �����ð� ��� üũ
                    
                }
                
                MouseAction.Invoke(Define.MouseEvent.Press); 
                _pressed = true;
            }
            else if(Input.GetMouseButton(1))
            {
                if (!_pressed) // �ѹ��� ���������� �����µ� �������� ���°Ŷ��
                {
                  
                    MouseAction.Invoke(Define.MouseEvent.PointerRightDown); //��������Ʈ�� �̺�Ʈȣ��
                    _pressedTime = Time.time; // �����ð� ��� üũ
                }
                
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed) //Click ->PointerUp(�� ���� �����ִٰ� ���� ��)
                {
                    if (Time.time < _pressedTime + 0.2f) //0.2���̳��� ���� ��
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false; // pressed�� �ʱ�ȭ
                _pressedTime = 0; //�� ���� �ʱ�ȭ
            }
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}

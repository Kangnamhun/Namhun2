using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class UI_Base : MonoBehaviour
{

    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>(); // 키값/ 밸류


    public abstract void Init(); // 상속받은 애들이 쓸거라

    private void Start()
    {
        Init();
        AddUIEvent(this.gameObject, (PointerEventData data) => Managers.Sound.Play("EffectSound/Button"));
    }
    protected void Bind<T>(Type type) where T : UnityEngine.Object // 값저장 함수
    {
        string[] names = Enum.GetNames(type);            // 타입의 name들을 리턴해서 저장 
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length]; //names의 길이만큼 오브젝트 배열 크기 할당
        _objects.Add(typeof(T), objects); // 딕셔너리 _objects에 Add(타입(ex Button) , 오브젝트 배열)

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true); // 스크립트가 달린 gameobject, 이름 , 자식까지 찾을건가
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true); // 스크립트가 달린 gameobject, 이름 , 자식까지 찾을건가

            if (objects[i] == null)
            {
#if UNITY_EDITOR
                Debug.Log($"Failed to bind({names[i]})");
#endif

            }
        }
    }
    protected T Get<T>(int idx) where T : UnityEngine.Object //값뽑는 함수
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false) // 딕셔너리의 값을 가져올땐 TryGetValue
            return null; // 값 가져오는걸 실패했을때 return null;

        return objects[idx] as T; // T타입으로 캐스팅

    }

    protected Text GetText(int idx) { return Get<Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }


    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt= Util.GetOrAddComponent<UI_EventHandler>(go);
        
        switch(type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;

            case Define.UIEvent.OnPointer:
                evt.OnPointerHandler -= action;
                evt.OnPointerHandler += action;
                break;
            case Define.UIEvent.OnPointerExit:
                evt.OnPointerHandler -= action;
                evt.OnPointerHandler += action;
                break;
        }
    }

    
}

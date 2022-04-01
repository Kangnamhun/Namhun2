using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Util 
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    //최상위 부모 , 이름 ->비우면 타입만 , recursive -> 자식의 자식도 찾을것인가
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null) // gameobject가 비었다면 null 리턴
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name) // 이름으로 거르기
                {
                    T component = transform.GetComponent<T>(); // 이름이 맞았으니 컴퍼넌트가 있는지 확인하기
                    if (component != null)  // 널이 아니면 들어가니까 -> 리턴
                        return component;
                }
            }
        }
        else
        {
            foreach(T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name) // 이름이 비어있거나 타입이 같다면
                    return component;
            }
        }

        return null;

    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)  // 그냥오브젝트 찾을때
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null) return null;
        return transform.gameObject;
    }
}

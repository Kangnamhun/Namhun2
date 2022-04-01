using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object   // T에 GameObject가 들어갈것이라 where T : Object가 된다.
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null) // parent -> 없으면 null 자동 대입
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");   // $ 

        if(prefab ==null)
        {
#if UNITY_EDITOR
            Debug.Log($"{path}가 없습니다");
#endif
            return null;
        }
        return Object.Instantiate(prefab, parent);  // 오브젝트 안 붙이면 재귀적인 함수 구현된다.
    }

    
    public void Destroy(GameObject go,float time = 0f)
    {
        if (go == null)
            return;
        Object.Destroy(go,time);
    }
}

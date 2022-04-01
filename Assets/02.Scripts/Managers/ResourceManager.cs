using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object   // T�� GameObject�� �����̶� where T : Object�� �ȴ�.
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null) // parent -> ������ null �ڵ� ����
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");   // $ 

        if(prefab ==null)
        {
#if UNITY_EDITOR
            Debug.Log($"{path}�� �����ϴ�");
#endif
            return null;
        }
        return Object.Instantiate(prefab, parent);  // ������Ʈ �� ���̸� ������� �Լ� �����ȴ�.
    }

    
    public void Destroy(GameObject go,float time = 0f)
    {
        if (go == null)
            return;
        Object.Destroy(go,time);
    }
}

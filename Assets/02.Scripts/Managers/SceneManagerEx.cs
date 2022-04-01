//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public string loadSceneName { get; private set; }

    string GetSceneName(SceneState type)
    {
        string name = System.Enum.GetName(typeof(SceneState), type); // C#의 Reflection. Scene enum의 
        return name;
    }

    public void LoadScene(SceneState type)
    {
        Clear(); //불필요한 메모리 클리어

        loadSceneName = GetSceneName(type);
        SceneManager.LoadScene("Loading");
    }
    
    public void Clear()
    {
        CurrentScene.Clear();

    }
}

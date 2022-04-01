using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    protected override void Init()  // ��� ���� Awake() �ȿ��� �����. "LoginScene"�� �ʱ�ȭ
    {
        
        base.Init();
        SceneType = SceneState.Login;
        Managers.UI.ShowSceneUI<UI_LoginCanvas>();

    }


    public override void Clear()
    {
    }

}
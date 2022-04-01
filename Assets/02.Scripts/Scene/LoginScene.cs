using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    protected override void Init()  // 상속 받은 Awake() 안에서 실행됨. "LoginScene"씬 초기화
    {
        
        base.Init();
        SceneType = SceneState.Login;
        Managers.UI.ShowSceneUI<UI_LoginCanvas>();

    }


    public override void Clear()
    {
    }

}
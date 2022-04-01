using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class UI_LoginCanvas : UI_Scene 
{
    #region SetUp
    enum Buttons
    {
        LoginButton, 
        ExitButton
    }


    enum GameObjects
    {
        IDInputField,
        BackGround
    }
   
    Button loginButton; // �� ���� ��ư
    Button exitButton;

    InputField idInput;
    public string nickName { get; private set; }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        Bind<GameObject>(typeof(GameObjects));

  

        loginButton = GetButton((int)Buttons.LoginButton);

        exitButton = GetButton((int)Buttons.ExitButton);

        idInput = Get<GameObject>((int)GameObjects.IDInputField).GetComponent<InputField>();
        idInput.characterLimit = 10; // ���ڼ� ����



        
        loginButton.gameObject.AddUIEvent(ClickLogin); // �α��� Ŭ����
        exitButton.gameObject.AddUIEvent(Exit);
        

    }
#endregion

    public void ClickLogin(PointerEventData data)
    {
        if(Regex.IsMatch(idInput.text, @"[^a-zA-Z0-9��-�R]"))
        {
            UI_Message ui_Message =  Managers.UI.ShowPopupUI<UI_Message>();
            ui_Message.Init();
            ui_Message.ShowMessage("����", "�̸��� Ư�����ڴ� �� �� �����ϴ�");
            ui_Message.okButton.gameObject.AddUIEvent(ui_Message.Cancel);
            return;
        }
        Managers.Game.SetName(idInput.text);
        Managers.Scene.LoadScene(SceneState.Tutorial);
        Destroy(gameObject);
    }


    public void  Exit(PointerEventData data)
    {
        Application.Quit();
    }

}

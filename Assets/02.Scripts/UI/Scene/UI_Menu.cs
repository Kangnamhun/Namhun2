using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Menu : UI_Scene
{
    #region SetUp
    bool bInit;

    enum Buttons
    {
        ResumeButton,
        SoundButton,
        ScreenButton,
        ExitButton
    }
    enum GameObjects
    {
        Body,
        MainMenu,
        SoundMenu,
        ScreenMenu
       

    }

    GameObject body;
    GameObject mainMenu;
    GameObject soundMenu;
    GameObject screenMenu;
    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));


        body = Get<GameObject>((int)GameObjects.Body);
        mainMenu = Get<GameObject>((int)GameObjects.MainMenu);
        soundMenu = Get<GameObject>((int)GameObjects.SoundMenu);
        screenMenu = Get<GameObject>((int)GameObjects.ScreenMenu);
        
        GetButton((int)Buttons.ResumeButton).gameObject.AddUIEvent(ResumeButton);
        GetButton((int)Buttons.SoundButton).gameObject.AddUIEvent(SoundButton);
        GetButton((int)Buttons.ScreenButton).gameObject.AddUIEvent(ScreenButton);
        GetButton((int)Buttons.ExitButton).gameObject.AddUIEvent(ExitButton);

        body.SetActive(false);
    }
    #endregion

    public void OpenMenu()
    {
        if (!body.activeSelf)
        {
            body.SetActive(true);
            soundMenu.SetActive(false);
            screenMenu.SetActive(false);
            Managers.UI.AddLinkedList(body);
        }
        else
        {
            body.SetActive(false);
            Managers.UI.RemoveLinkedList(body);
        }


        
    }

    #region ButtonEvent
    public void ResumeButton(PointerEventData data)
    {
        body.SetActive(false);
    }
    public void SoundButton(PointerEventData data)
    {
      
        soundMenu.SetActive(true);
    }
    public void ScreenButton(PointerEventData data)
    {
        screenMenu.SetActive(true);
    }

    public void ExitButton(PointerEventData data)
    {
        UI_Message ui_Message = Managers.UI.ShowPopupUI<UI_Message>();
        ui_Message.Init();
        ui_Message.ShowMessage("종료", "종료하시겠습니까?");
        ui_Message.okButton.gameObject.AddUIEvent(ui_Message.GameQuit);
    }
    #endregion
}

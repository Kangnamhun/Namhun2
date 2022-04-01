using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Message : UI_Popup
{
    #region SetUp
    bool bInit;
    Text title, content;
    [HideInInspector]
    public Button okButton;
    [HideInInspector]
    public Button cancelButton;
    public Slider countSlider { get; private set; }

    enum Texts
    {
        TitleText,
        ContentText,
    
    }
    enum Buttons
    {
        OKButton,
        CancelButton
    }
    enum GameObjects
    {
        CountSlider
    }

    public override void Init()
    {
        base.Init();
        if (bInit) return;
        bInit = true;

        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        okButton = GetButton((int)Buttons.OKButton);
        cancelButton = GetButton((int)Buttons.CancelButton);
        title = GetText((int)Texts.TitleText);
        content = GetText((int)Texts.ContentText);
        countSlider = Get<GameObject>((int)GameObjects.CountSlider).GetComponent<Slider>();

        cancelButton.gameObject.AddUIEvent(Cancel);

        countSlider.onValueChanged.AddListener(Function_Slider);
        countSlider.gameObject.SetActive(false);
    }
    #endregion
    public void ShowMessage(string _title, string _content)
    {
        title.text = _title; //제목
        content.text = _content;// 본문
    }


    public void Cancel(PointerEventData data)
    {
        Managers.UI.ClosePopupUI(this);
    }
    #region OutEvent
    public void SceneMoveOk(PointerEventData data)
    {
        if (!SceneMove())
        {
            ShowMessage("에러", "이전 퀘스트를 완료해주세요");
            okButton.gameObject.AddUIEvent(Cancel);
            return;
        }

        Managers.Scene.LoadScene((SceneState)Managers.talk.NPC.GetComponent<ObjData>().id);

        Managers.UI.ClosePopupUI(this);
    }
    bool SceneMove()  // 퀘스트 완료 안 하면 못돌아감
    {
        SceneState sceneState = BaseScene.instance.SceneType;
        if (sceneState == SceneState.Tutorial)
        {

            foreach (var quest in Managers.Quest.CompletedQuests)
            {
                foreach (var taskgroup in quest.TaskGroups)
                {
                    foreach (var task in taskgroup.Tasks)
                    {

                        if (task.CodeName == "JUMP" && task.IsComplete)
                        {
                            return true;

                        }
                    }
                }
            }
        }
        else if (sceneState == SceneState.Select)
        {

            foreach (var quest in Managers.Quest.CompletedQuests)
            {
                foreach (var taskgroup in quest.TaskGroups)
                {
                    foreach (var task in taskgroup.Tasks)
                    {

                        if (task.CodeName == "KILL_DUMMY" && task.IsComplete)
                        {
                            return true;

                        }
                    }
                }
            }
        }
        else if (sceneState == SceneState.Town)
        {
            if(Managers.talk.NPC.GetComponent<ObjData>().id == 6004){
                foreach (var quest in Managers.Quest.ActiveQuests)
                {
                    if(quest.CodeName == "BOSS"){
                        return true;
                    }
                    else{
                        return false;
                    }
                }
            }
            
            if(Managers.talk.NPC.GetComponent<ObjData>().id != 6004){
                foreach(var quest in Managers.Quest.ActiveQuests){
                    if(quest.CodeName == "TOWN"){
                        return true;
                    }
                    else{
                        return false;
                    }
                }
                
            }
        }
        else if (sceneState == SceneState.Map1)
        {

            foreach (var quest in Managers.Quest.CompletedQuests)
            {
                foreach (var taskgroup in quest.TaskGroups)
                {
                    foreach (var task in taskgroup.Tasks)
                    {

                        if (task.CodeName == "KILL_MAP1BOSS" && task.IsComplete)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        else if (sceneState == SceneState.Map2)
        {

            foreach (var quest in Managers.Quest.CompletedQuests)
            {
                foreach (var taskgroup in quest.TaskGroups)
                {
                    foreach (var task in taskgroup.Tasks)
                    {

                        if (task.CodeName == "KILL_EARTHSLIME" && task.IsComplete)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void ReSpawn(PointerEventData data) //부활 
    {
        RecoverSet();
        Managers.UI.ClosePopupUI(this);
    }

    public void TownSpawn(PointerEventData data)
    {
        Managers.Scene.LoadScene(SceneState.Town);
        RecoverSet();
        Managers.UI.ClosePopupUI(this);
      
    }
    public void GameQuit(PointerEventData data)
    {
        Application.Quit();
    }
    public void RecoverSet()
    {
        PlayerStatus playerStatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
        playerStatus.Hp = playerStatus.MAX_HP;
        playerStatus.GetComponent<Animator>().SetTrigger("Recover");
        playerStatus.GetComponent<Animator>().SetBool("PlayerDie", false);
        playerStatus.BDeath = false;
        Managers.UI.ui_PlayerData.DisplayHP(playerStatus.Hp, playerStatus.MAX_HP);
    }
    #endregion



    private void Function_Slider(float _value) // value에 따른 개수 변경
    {
        ShowMessage("구매", (int)_value + "개 구매");
       
    }



}

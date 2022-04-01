using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Quest : UI_Scene
{
    #region SetUp
    bool bInit;

    enum GameObjects
    {
       QuestView,
       CompletedQuestListView
    }



    public GameObject questView;
    GameObject completedQuestListView;
    public override void Init()
    {
        if (bInit) return;
        base.Init();
        bInit = true;

        
        Bind<GameObject>(typeof(GameObjects));
        questView = Get<GameObject>((int)GameObjects.QuestView);
        completedQuestListView = Get<GameObject>((int)GameObjects.CompletedQuestListView);

        questView.SetActive(false);
        completedQuestListView.SetActive(false);
    }
    #endregion

    public void OpenQuest()
     {
        Debug.Log(questView.activeSelf);
        if (!questView.activeSelf)
        {
            
            questView.SetActive(true);
            Managers.UI.AddLinkedList(questView);
        }
        else
        {
            
            questView.SetActive(false);
            Managers.UI.RemoveLinkedList(questView);
        }

    }
}

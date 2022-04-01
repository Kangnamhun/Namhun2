using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Panel : UI_Popup
{
    
    enum Buttons
    {
        JobButton,
        ExitButton
    }

    enum Texts
    {
        TalkText,
        NPCNameText,
    }

    enum Images
    {
        JobBackGroundImage
    }

    bool bInit;
    ObjData objData;
    QuestReporter questReporter;

    [HideInInspector]
    public Button okButton;

    Text npcNameText;
    Text talkText;

    
    public override void Init()
    {
        base.Init();
        if (bInit) return;
        bInit = true;


        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        okButton = GetButton((int)Buttons.JobButton);
        GetButton((int)Buttons.ExitButton).gameObject.AddUIEvent(JobExitButton);

        objData = Managers.talk.NPC.GetComponent<ObjData>();

        npcNameText = GetText((int)Texts.NPCNameText);
        talkText = GetText((int)Texts.TalkText);


        talkText.text = Managers.talk.GetTalk(objData.id,talkIndex);
        npcNameText.text = objData.gameObject.name;

        questReporter = objData.GetComponent<QuestReporter>();
    }


    public void JobExitButton(PointerEventData data) // ��ҹ�ư ������
    {
        talkIndex = 0;
        Managers.UI.ClosePopupUI();

    }
    public void OnJobChoiceButton(PointerEventData data)
    {
        JobController jobController = Managers.Game.GetPlayer().GetOrAddComponent<JobController>();
       
       
        if (jobController.jobstring != null)  //������ �̹� ������
        {
            if (Managers.talk.GetTalk(talkIndex) == null)
            {
                talkIndex = 0;
                Managers.UI.ClosePopupUI(this);
            }
            talkText.text = Managers.talk.GetTalk(talkIndex);
            talkIndex++;
        }
        else // �ƴϸ� �� ��ȭ ����
        {
            talkIndex++;
            talkText.text = Managers.talk.GetTalk(objData.id, talkIndex);
        }
        if (Managers.talk.GetTalk(objData.id, talkIndex) == null) //���� �� ������
        {
            talkIndex = 0;
            jobController.JobChoice(objData.id); // ����Ȯ��
            questReporter.Report();
            Managers.UI.ClosePopupUI(this);
        }
            

    }

    public void QuestGive(PointerEventData data)
    {
        talkIndex++;
        talkText.text = Managers.talk.GetTalk(objData.id, talkIndex);

        if (Managers.talk.GetTalk(objData.id, talkIndex) == null) //���� �� ������
        {
            talkIndex = 0;
            objData.GetComponent<QuestGiver>().QuestSet();
            Managers.UI.ClosePopupUI(this);

        }

    }
}

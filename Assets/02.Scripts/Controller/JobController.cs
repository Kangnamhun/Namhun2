using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JobInfo { COMMON, SWORD, MAGIC , BOW, STORE }

public class JobController : MonoBehaviour
{
    public string jobstring { get; private set; }
    ItemData itemData;

    void Start()
    {
        DontDestroyOnLoad(this);
        jobstring = null;
    }

    public void JobChoice(int jobstate)  // 직업 활성화
    { 
        if (jobstate == (int)JobInfo.SWORD) // 전사
        {
            jobstring = "Sword";
            itemData = new ItemData((int)Define.Itemcode.Sword1);
        }
        else if (jobstate == (int)JobInfo.MAGIC) // 법사 전직
        {
            jobstring = "Magic";
            itemData = new ItemData((int)Define.Itemcode.Wand1);
        }

        else if (jobstate == (int)JobInfo.BOW) // 궁수 전직
        {
            jobstring = "Bow";
            itemData = new ItemData((int)Define.Itemcode.Bow1);
        }

        Managers.UI.ui_Inventory.AddItemData(itemData); //직업에 맞는 템 부여
        gameObject.AddComponent(System.Type.GetType(jobstring)); // 직업에 맞는 스크립트 부여

    }

}

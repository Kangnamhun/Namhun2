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

    public void JobChoice(int jobstate)  // ���� Ȱ��ȭ
    { 
        if (jobstate == (int)JobInfo.SWORD) // ����
        {
            jobstring = "Sword";
            itemData = new ItemData((int)Define.Itemcode.Sword1);
        }
        else if (jobstate == (int)JobInfo.MAGIC) // ���� ����
        {
            jobstring = "Magic";
            itemData = new ItemData((int)Define.Itemcode.Wand1);
        }

        else if (jobstate == (int)JobInfo.BOW) // �ü� ����
        {
            jobstring = "Bow";
            itemData = new ItemData((int)Define.Itemcode.Bow1);
        }

        Managers.UI.ui_Inventory.AddItemData(itemData); //������ �´� �� �ο�
        gameObject.AddComponent(System.Type.GetType(jobstring)); // ������ �´� ��ũ��Ʈ �ο�

    }

}

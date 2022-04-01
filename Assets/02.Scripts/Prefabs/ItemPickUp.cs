using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ʵ忡 ������ ������ Ŭ����
//1.ó�� �����Ҷ� �����ۿ� �ڵ带 �־���(itemcode)
//2.itemcode -> dic -> �ش������ Ŭ������ �����ͼ� ����������
//20001�� �־��ָ� ��հ��� ������ �ȴ�.
public class ItemPickUp : MonoBehaviour
{
    Transform tr;

    //������ ������ �ڵ� ��ȣ
    public int itemcode;

    //������ ����
    public int count = 1;

    //itemcode�� ���� ������ ������
    //itemData�� ����������(��: ��ȭȽ��,���� ����)�� �Һ������Ͱ� ����
    public ItemData itemData { get; private set; }
    public eItemType eitemType;

    private void Start()
    {
        InitData(itemcode, count);
        tr = GetComponent<Transform>();
    }

    public void ClearDestroy()
    {
        Destroy(tr.gameObject);
    }

    //Start���� ������ �ʱ�ȭ ���ش�
    //1.Start�� ���ؼ� ���� �ν����Ϳ� �ִ� �����͸� �������� �ʱ�ȭ�ؼ� ����
    public void InitData(int _itemcode = -1, int _count = 1)
    {
        if(_itemcode == -1)
        {
            _itemcode = itemcode;
            _count = count;
        }
        else
        {
            itemcode = _itemcode;
            count = _count;
        }    
        itemData = new ItemData(_itemcode, count);
    }


    //������ ȸ����Ű�� �Լ���
    public float TurnSpeed = 90f;
    public Vector3 axis = Vector3.up;
    private void Update() 
    {
        tr.Rotate(axis * TurnSpeed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//필드에 떨어진 아이템 클래스
//1.처음 생성할때 아이템에 코드를 넣어줌(itemcode)
//2.itemcode -> dic -> 해당아이템 클래스를 가져와서 세팅을해줌
//20001을 넣어주면 양손검이 세팅이 된다.
public class ItemPickUp : MonoBehaviour
{
    Transform tr;

    //세팅할 아이템 코드 번호
    public int itemcode;

    //아이템 수량
    public int count = 1;

    //itemcode로 얻어온 아이템 데이터
    //itemData는 가변데이터(예: 강화횟수,물약 갯수)과 불변데이터가 공존
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

    //Start에서 강제로 초기화 해준다
    //1.Start를 통해서 오면 인스펙터에 있는 데이터를 기준으로 초기화해서 셋팅
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


    //아이템 회전시키는 함수들
    public float TurnSpeed = 90f;
    public Vector3 axis = Vector3.up;
    private void Update() 
    {
        tr.Rotate(axis * TurnSpeed * Time.deltaTime);
    }
}

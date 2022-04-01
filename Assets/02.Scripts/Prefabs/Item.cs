using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject{ 
    
    public int itemID;  //아이템의 고유 ID값. 중복 불가능
    public string itemName; //아이템의 이름. 중복 불가능.(고대유물, 고대유물)
    public string itemDescription;  //아이템 설명
    public int itemCount; //아이템 소지 개수.
    public Sprite itemIcon; //아이템의 아이콘
    public GameObject itemPrefab; // 아이템의 프리팹.
    public eItemType itemType;   // 아이템 타입(장비,소모품,기타)


    public Item(int _itemID, string _itemName, string _itemDes, eItemType _itemType, int _itemCount = 1)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemType = _itemType;
        itemCount = _itemCount;
        itemIcon = Resources.Load("ItemIcon/" + _itemID.ToString(), typeof(Sprite))as Sprite;
    }

    
}


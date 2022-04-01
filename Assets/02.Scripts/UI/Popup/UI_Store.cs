using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[System.Serializable]
public class StoreItemInfo
{
    public int itemcode;
    public int itemCount = 1;
}

public class UI_Store : UI_Popup
{

    enum Buttons
    {
        StoreItem,
        CloseButton
    }
    enum GameObjects
    {
        ContentGroup,
    }


    GameObject content;
    StoreItem storeItem; 
    Button closeButton;


    List<StoreItemInfo> listInfo = new List<StoreItemInfo>();
    List<StoreItem> list = new List<StoreItem>();


    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        storeItem = GetButton((int)Buttons.StoreItem).GetComponent<StoreItem>();
        content = Get<GameObject>((int)GameObjects.ContentGroup);
        closeButton = GetButton((int)Buttons.CloseButton);

        ListInit();
        StoreItem[] _storeItem = new StoreItem[listInfo.Count];
        for (int i = 0; i < _storeItem.Length; i++)
        {
            _storeItem[i] = Managers.Resource.Instantiate("StoreItem", content.transform).GetComponent<StoreItem>();
            _storeItem[i].Init();
            _storeItem[i].SetItem(listInfo[i].itemcode, listInfo[i].itemCount, _storeItem[i].OnClickStoreItem); // itemdata 전달
            AddUIEvent(_storeItem[i].gameObject, _storeItem[i].ItemClick);
            list.Add(_storeItem[i]);
        }
  
        Destroy(storeItem.gameObject);
        closeButton.gameObject.AddUIEvent(OnClose);
        
    }
    public void ListInit()
    {
        listInfo.Add(new StoreItemInfo() { itemcode = 10001, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 10002, itemCount = 1 });

        listInfo.Add(new StoreItemInfo() { itemcode = 21001, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 21002, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 22001, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 22002, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 23001, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 23002, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 20001, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 20002, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 20101, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 20102, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 20201, itemCount = 1 });
        listInfo.Add(new StoreItemInfo() { itemcode = 20202, itemCount = 1 });
        
    } // 상점에 아이템 추가


    
    



    void OnClose(PointerEventData data)
    {

        Managers.UI.ClosePopupUI(this);
    }
}

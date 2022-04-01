using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_Inventory : UI_Scene
{
    #region SetUp
    bool bInit;
   
    List<UI_InventorySlot> slotsEquip = new List<UI_InventorySlot>();  //인벤토리 슬롯들
    List<UI_InventorySlot> slotsUse = new List<UI_InventorySlot>();
    List<UI_InventorySlot> slotsETC = new List<UI_InventorySlot>();

    public Text Description_Text; //아이템에 대한 부연설명
    public string[] tabDescription; //탭 부연설명

    GameObject goEquipTab;
    GameObject goEquipSlot;
    GameObject goUseTab;
    GameObject goUseSlot;
    GameObject goETCTab;
    GameObject goETCSlot;
    GameObject body;
    GameObject inventory;

    int MAX_EQUIP = 20;
    List<ItemData> listEquip = new List<ItemData>();
    List<ItemData> listUse = new List<ItemData>();
    List<ItemData> listETC = new List<ItemData>();


    public RectTransform CanvaRect;
    public Vector2 v;
    
    PlayerStatus playerstatus;

    enum GameObjects
    {
        EquipBody,
        UseBody,
        ETCBody,
        Equip_GridSlot,
        Use_GridSlot,
        ETC_GridSlot,
        EquipPanel,
        UsePanel,
        ETCPanel,
        Body,
        Inventory
    }
    enum Buttons
    {
        Equip_Selected_Tab,
        Use_Selected_Tab,
        ETC_Selected_Tab,
        CloseButton
    }


    public override void Init()
    {
        if (bInit) return;
        bInit = true;
        #region Set
        base.Init();
        
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        goEquipTab = Get<GameObject>((int)GameObjects.EquipPanel);
        goUseTab = Get<GameObject>((int)GameObjects.UsePanel);
        goETCTab = Get<GameObject>((int)GameObjects.ETCPanel);
        goEquipSlot = Get<GameObject>((int)GameObjects.Equip_GridSlot); 
        goUseSlot = Get<GameObject>((int)GameObjects.Use_GridSlot);
        goETCSlot = Get<GameObject>((int)GameObjects.ETC_GridSlot);
        body = Get<GameObject>((int)GameObjects.Body);
        inventory = Get<GameObject>((int)GameObjects.Inventory);
        #endregion
        #region invenslotSet
        GameObject equipbody = Get<GameObject>((int)GameObjects.EquipBody);
        GameObject usebody = Get<GameObject>((int)GameObjects.UseBody);
        GameObject etcbody = Get<GameObject>((int)GameObjects.ETCBody);

        GetButton((int)Buttons.CloseButton).gameObject.AddUIEvent(CloseInventory);
        

        SetSlot(equipbody, slotsEquip, listEquip);
        SetSlot(usebody, slotsUse, listUse);
        SetSlot(etcbody, slotsETC, listETC);

        goUseSlot.SetActive(false); //켜고 소비랑 기타 꺼주기
        goETCSlot.SetActive(false);
        goUseTab.SetActive(false);
        goETCTab.SetActive(false);
        #endregion


        //탭선택 이벤트 부여
        GetButton((int)Buttons.Equip_Selected_Tab).gameObject.AddUIEvent(Invoke_EquipTab);
        GetButton((int)Buttons.Use_Selected_Tab).gameObject.AddUIEvent(Invoke_UseTab);
        GetButton((int)Buttons.ETC_Selected_Tab).gameObject.AddUIEvent(Invoke_ETCTab);

        playerstatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();

        inventory.AddUIEvent((PointerEventData data) => { inventory.transform.position = data.position; }, Define.UIEvent.Drag);
        body.SetActive(false); //생성되면 꺼주기

    }
    
    void SetSlot(GameObject _go, List<UI_InventorySlot> _slotList, List<ItemData>_listData)
    {
        GameObject _itemGO;
        UI_InventorySlot _inventorySlot;
        
        for (int i = 0; i < MAX_EQUIP; i++)
        {
            _itemGO = Managers.Resource.Instantiate("UI/Popup/UI_InventorySlot");
            _inventorySlot = _itemGO.GetComponent<UI_InventorySlot>();
            _slotList.Add(_inventorySlot);
            _inventorySlot.Init();
            _itemGO.transform.SetParent(_go.transform);
            _itemGO.transform.localScale = Vector3.one;

            _listData.Add(null);
        }
    }
    #endregion
    #region tabsetting

    public void Invoke_SetTab()
    {
        goEquipTab.SetActive(false);
        goEquipSlot.SetActive(false);
        goUseTab.SetActive(false);
        goUseSlot.SetActive(false);
        goETCTab.SetActive(false);
        goETCSlot.SetActive(false);
    }
    public void Invoke_EquipTab(PointerEventData data)
    {
        Invoke_SetTab();
        goEquipTab.SetActive(true);
        goEquipSlot.SetActive(true);
        Description_Text.text = tabDescription[0];
    }
    public void Invoke_UseTab(PointerEventData data)
    {
        Invoke_SetTab();
        goUseTab.SetActive(true);
        goUseSlot.SetActive(true);
        Description_Text.text = tabDescription[1];

    }
    public void Invoke_ETCTab(PointerEventData data)
    {
        Invoke_SetTab();
        goETCTab.SetActive(true);
        goETCSlot.SetActive(true);
        Description_Text.text = tabDescription[2];
    }
    #endregion

    public void OnOffInventory()
    {
        if (!body.activeSelf)
        {
            body.SetActive(true);
            Managers.UI.AddLinkedList(body);
        }
        else
        {
            body.SetActive(false);
            Managers.UI.RemoveLinkedList(body);
        }


    }
    public void CloseInventory(PointerEventData data)
    {
        body.SetActive(false);
    }
    ItemData CheckItemData(List <ItemData>_list, ItemData _newItemData, out int _index)//동일한 아이템이 있는지 검사
    {
        _index = -1;
        foreach(ItemData _itemData in _list)
        {
            _index++;
            if(_itemData.itemcode == _newItemData.itemcode)//아이템이 새로들어온게 현재에 있는 아이템과 같을경우
            {
                return _itemData;
            }
        }
        return null;
    }
    #region AddRemoveItem
    //획득한 아이템 클래스를 그대로 받아서 넣어줌
    public bool AddItemData(ItemData _itemDataNew)
    {
        
      //  int _index = -1;
        bool _rtn = false;
        ItemData _itemDataOld = null;
        switch (_itemDataNew.itemType)    //탭 에따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가
        {
            case eItemType.Equip: //장비창, 장비템이면 한칸에 한개씩
                for (int i = 0; i < listEquip.Count; i++)
                {
                    if (listEquip[i] == null || (listEquip[i] != null &&listEquip[i].itemcode ==0)) 
                    {
                        listEquip[i] = _itemDataNew;
                        slotsEquip[i].SetItem(_itemDataNew);

                        //추가 되었다는 플래그를 알려줌
                        _rtn = true;
                        break;
                    }
                }
                break;

            case eItemType.Use: //소비창

                for (int i = 0; i < listUse.Count; i++)
                {
                    if (listUse[i] == null || (listUse[i] != null && listUse[i].itemcode == 0))
                    {
                        //신규 아이템추가
                        listUse[i] = _itemDataNew;
                        slotsUse[i].SetItem(_itemDataNew);

                        //추가 되었다는 플래그를 알려줌
                        _rtn = true;
                        break;
                    }
                    else if (listUse[i].itemcode != 0 && listUse[i].itemcode == _itemDataNew.itemcode)
                    {
                        //동일 아이템에 수량 추가
                        //아이템리스트에서 같은 아이템이 있는지 검색해서 같은 아이템이 있으면 추가기능
                        _itemDataOld = listUse[i];
                        _itemDataOld.itemCount += _itemDataNew.itemCount;
                        slotsUse[i].ReDisplayCount();
                        _rtn = true;
                        break;
                    }
                }
                break;

            case eItemType.ETC: //기타창
                for (int i = 0; i < listETC.Count; i++)
                {
                    if (listETC[i] == null || (listETC[i] != null && listETC[i].itemcode == 0))
                    {
                        //신규 아이템추가
                        listETC[i] = _itemDataNew;
                        slotsETC[i].SetItem(_itemDataNew);

                        //추가 되었다는 플래그를 알려줌
                        _rtn = true;
                        break;
                    }
                    else if (listETC[i].itemcode != 0 && listETC[i].itemcode == _itemDataNew.itemcode)
                    {
                        //동일 아이템에 수량 추가
                        //아이템리스트에서 같은 아이템이 있는지 검색해서 같은 아이템이 있으면 추가기능
                        _itemDataOld = listETC[i];
                        _itemDataOld.itemCount += _itemDataNew.itemCount;
                        slotsETC[i].ReDisplayCount();
                        _rtn = true;
                        break;
                    }
                }
                break;
        }
        return _rtn;
    }


    public bool RemoveItemData(ItemData _itemDataRemove)
    {
        bool _rtn = false;
        switch (_itemDataRemove.itemType)    //탭 에따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가
        {
            case eItemType.Equip: //장비창, 장비템이면 한칸에 한개씩
                for (int i = 0; i < listEquip.Count; i++)
                {
                    if (listEquip[i] == _itemDataRemove)
                    {
                        listEquip[i] = null;
                        _rtn = true;
                        break;
                    }
                }
                break;

            case eItemType.Use: //소비창

                for (int i = 0; i < listUse.Count; i++)
                {
                    if (listUse[i] == _itemDataRemove)
                    {
                        listUse[i] = null;
                        _rtn = true;
                        break;
                    }
                }
                break;

            case eItemType.ETC: //기타창
                for (int i = 0; i < listETC.Count; i++)
                {
                    if (listETC[i] == _itemDataRemove)
                    {
                        listETC[i] = null;
                        _rtn = true;
                        break;
                    }
                }
                break;
        }
        return _rtn;
    }
    #endregion
    public ItemData CheckAndEatHP(int _itemcode) //인벤토리 내에 아이템을 찾고 먹는
    {
        //체크 존재 유뮤
        ItemData _itemData = null;
        
        for (int i = 0; i < listUse.Count; i++)
        {
            if (listUse[i] != null && listUse[i].itemcode == _itemcode && listUse[i].itemCount > 0)//itemcode가 현재들어온itemcode랑 데이터가 일치하면
            {
                //찾는다
               _itemData = listUse[i];

                //해당 아이템 적용(사용)
                ItemInfoUsepart _usePart = ItemInfo.ins.GetItemInfoUsepart(_itemData.itemcode);
                playerstatus.SetHPMP(_usePart.hp, _usePart.mp);
                _itemData.itemCount--;
                if(_itemData.itemCount <= 0)
                {
                    listUse[i] = null;
                    slotsUse[i].RemoveItem();
                }
                else
                {
                    slotsUse[i].SetItem(_itemData);
                }
                break;
            }
        }
        return _itemData;
    }

    

}
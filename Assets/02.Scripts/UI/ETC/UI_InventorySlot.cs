using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InventorySlot : UI_Base
{
    bool bInit;
    public ItemData itemData;
    Image icon;
    Text itemCount;

    PlayerStatus playerStatus;
    enum  Images
    {
        Item_Icon,
        Panel
    }
    enum Texts
    {
        Item_Count_Text
    }
    public override void  Init()
    {
        if (bInit) return;
        bInit = true;

        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        
        icon = GetImage((int)Images.Item_Icon); 
        itemCount = GetText((int)Texts.Item_Count_Text);
        
        if (itemData == null || (itemData != null && itemData.itemCount <= 1))
        {
            itemCount.gameObject.SetActive(false); //아이템 수량 나오는것을 끔
        }
        playerStatus= Managers.Game.GetPlayer().GetComponent<PlayerStatus>();

        AddUIEvent(icon.gameObject, OnPointerClick); // eventhandler 접근하려고 

    }

    public void SetItem(ItemData _itemData)
    {
        itemData = _itemData;
        icon.sprite = _itemData.iconSprite;
        //icon.sprite = ItemInfo.ins.GetSprite(_itemData.icon);
        //icon.sprite = ItemInfo.ins.GetItemInfoSpriteIcon(_itemData.itemcode);
        if (itemData.itemCount <= 1)
        {
            itemCount.gameObject.SetActive(false);
        }
        else
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = "x" + itemData.itemCount;
        }
    }


    public void RemoveItem()
    {
        itemData = null;
        icon.sprite = null;
        itemCount.gameObject.SetActive(false);
    }

    public void ReDisplayCount()
    {
        itemCount.gameObject.SetActive(true);
        itemCount.text = "x" + itemData.itemCount;
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (clickCount == 2)//두번 클릭시
            OnDoubleClick();
    }


    void OnDoubleClick() //더블클릭시
    {
        if (itemData == null || itemData.itemcode ==0) return;//아이템데이터가 null이면 리턴
        switch(itemData.itemType)
        {
            case eItemType.Equip:   //장비창에서 더블클릭시
#if UNITY_EDITOR
                Debug.Log("더블클릭 >>장비교체");
#endif

                if (WeaponAniChange()) return;

                bool _bEquip = Managers.UI.ui_Equipment.Equip(itemData); //장비 장착해주는거
                if(_bEquip)
                {
                    Managers.UI.ui_Inventory.RemoveItemData(itemData);
                    RemoveItem();
                }
                break;

            case eItemType.Use:   //소비창에서 더블클릭시

                //물약을 먹음
                ItemInfoUsepart _usePart = ItemInfo.ins.GetItemInfoUsepart(itemData.itemcode);

                playerStatus.SetHPMP(_usePart.hp, _usePart.mp);
                Managers.Sound.Play("EffectSound/Potion");
                itemData.itemCount--; //아이템에서 수량을 한개빼줌
                if(itemData.itemCount <= 0)//아이템 수량이 0개가되면
                {
                    Managers.UI.ui_Inventory.RemoveItemData(itemData);
                    RemoveItem();
                }
                else
                {
                    SetItem(itemData);
                }
#if UNITY_EDITOR
                Debug.Log("더블클릭 >>물약먹기 HP:" + _usePart.hp + " MP: " + _usePart.mp);
#endif
                break;
            case eItemType.ETC:   //기타창에서 더블클릭시
#if UNITY_EDITOR
                Debug.Log("더블클릭 >>작동안함");
#endif
                break;
        }
    }


    bool WeaponAniChange()
    {
        int weaponcode = itemData.itemcode % 20000;
        if (weaponcode < 1000) // 직업다른데 착용 시도 했을때
        {
            //0,1 전사 , 101,102 법사 201,202 궁수
            if (weaponcode < 100)
            {
                return JobAniSet("Sword");

            }
            else if (weaponcode < 200)
            {
                return JobAniSet("Magic");
            }
            else if (weaponcode < 300)
            {
                return JobAniSet("Bow");
            }

        }
        return false;
    }


    bool JobAniSet(string jobstring)
    {
        JobController jobController = Managers.Game.GetPlayer().GetComponent<JobController>();
        if (jobController.jobstring != jobstring)
        {
            Managers.UI.ui_ErrorText.SetErrorText(Define.Error.OtherWeapon);
            return true;
        }
        else
        {
            jobController.GetComponent<Animator>().runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>($"Animator/{jobController.jobstring}");
            return false;
        }
    }
}


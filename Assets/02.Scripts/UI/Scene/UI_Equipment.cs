using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum eEquipmentSlot { Head, Armor, Weapon, Boots, NotSlot };
[System.Serializable]
public class EquipmentSlot
{
    public string name; //보기용
    public ItemData itemData;
    public UI_EquipmentSlot equipSlot;
    public SkinnedMeshRenderer skin;
}


public class UI_Equipment : UI_Scene
{
    bool bInit;


    enum GameObjects
    {
        Body,
        HelmetBG,
        ArmorBG,
        BootsBG,
        WeponBG,
        ExitButton,
        Equipment
    }
    enum Texts
    {
        Attack_val,
        Hp_val,
        Mp_val,
        DEF_val
    }
    
    public EquipmentSlot[] defaultSlots = new EquipmentSlot[4];
    public EquipmentSlot[] slots = new EquipmentSlot[4]; //4개의 배열생성

    GameObject body;
    GameObject helmetBG, armorBG, bootsBG, weaponBG;
    GameObject equipment;

    PlayerStatus playerstatus;
    Text attText, defText, hpText, mpText;


    public override void Init()
    {
        if (bInit) return;
        bInit = true;

        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        attText = GetText((int)Texts.Attack_val);
        hpText = GetText((int)Texts.Hp_val);
        mpText = GetText((int)Texts.Mp_val);
        defText = GetText((int)Texts.DEF_val);

        body = Get<GameObject>((int)GameObjects.Body);

        helmetBG = Get<GameObject>((int)GameObjects.HelmetBG);
        armorBG = Get<GameObject>((int)GameObjects.ArmorBG); 
        bootsBG = Get<GameObject>((int)GameObjects.BootsBG);
        weaponBG = Get<GameObject>((int)GameObjects.WeponBG);
        equipment = Get<GameObject>((int)GameObjects.Equipment);

        Get<GameObject>((int)GameObjects.ExitButton).AddUIEvent(Invoke_CloseEquipment);

        slots[0].equipSlot = helmetBG.GetComponent<UI_EquipmentSlot>();
        slots[1].equipSlot = armorBG.GetComponent<UI_EquipmentSlot>();
        slots[2].equipSlot = weaponBG.GetComponent<UI_EquipmentSlot>();
        slots[3].equipSlot = bootsBG.GetComponent<UI_EquipmentSlot>();
        for(int i =0; i < slots.Length; i++)
        {
            slots[i].equipSlot.Init();
        }

        equipment.AddUIEvent((PointerEventData data) => { equipment.transform.position = data.position; }, Define.UIEvent.Drag);


        playerstatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
        DisplayAttack(playerstatus.attack);
        DisplayDEF(playerstatus.defense);
        DisplayHP(playerstatus.Hp);
        DisplayMP(playerstatus.mp);

        body.SetActive(false);

    }

    public void OnOffEquipment() //장비창 키는함수(새로운방식)
    {
        if(!body.activeSelf)
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
    


        public void Invoke_CloseEquipment(PointerEventData data) //장비창 끄는함수(기존에 알던 방식)
    {
        body.SetActive(false);
    }


    public bool Equip(ItemData _itemData) //장착
    {
        bool _rtn = false;
        eEquipmentSlot _slot = _itemData.equipmentSlot;
        int _index = (int)_slot;
        if (_index < slots.Length)
        {
            
            if (slots[_index].itemData != null && slots[_index].itemData.itemcode != 0)
            {
                Debug.Log("@@@ E장착해제");

                ItemData _oldItemData = slots[_index].itemData; //기존에 있던 장비 탈착
                Managers.UI.ui_Inventory.AddItemData(slots[_index].itemData);//장비를 해제하면 UI_Inventory 에다 넣어줌
                slots[_index].itemData = null; //장비창 슬롯을 비워준다
                slots[_index].equipSlot.SetItem(null); //slots에있는 SetItem의 아이콘을 지움

                //장착 Mesh - 능력치
                playerstatus.UnEquip(_index, _oldItemData);// 해당 아이템 데이터를 탈착
            }

            //새것
#if UNITY_EDITOR
            Debug.Log("@@@ UI장착중");
#endif

            slots[_index].itemData = _itemData; //slots에 아이템 데이터를 넣음
            slots[_index].equipSlot.SetItem( _itemData); //_itemData에서 아이템 데이터를 세팅

            //장착 Mesh + 능력치
            playerstatus.Equip(_index, _itemData);  // 해당 아이템 데이터를 장착
            _rtn = true;
        }
        Managers.Sound.Play("EffectSound/Equip");
        return _rtn;
    }

    public void UnEquip(ItemData _itemData)//탈착
    {
        eEquipmentSlot _slot = _itemData.equipmentSlot;
        int _index = (int)_slot;
        if (_index < slots.Length)
        {

            if (slots[_index].itemData != null)
            {
#if UNITY_EDITOR
                Debug.Log("@@@ E장착해제");
#endif
                slots[_index].itemData = null;
                slots[_index].equipSlot.SetItem(null);

                Managers.UI.ui_Inventory.AddItemData(_itemData);
            }
            playerstatus.UnEquip((int)_slot, _itemData);
        }
        if (_slot == eEquipmentSlot.Weapon)
        {
            Managers.Game.GetPlayer().GetComponent<JobController>().GetComponent<Animator>()
                           .runtimeAnimatorController = Managers.Resource.Load<RuntimeAnimatorController>("Animator/None");
        }
        Managers.Sound.Play("EffectSound/Equip");
    }

    public void DisplayAttack(float _att)
    {
        attText.text = ((int)_att).ToString();
    }

    public void DisplayDEF(float _def)
    {
        defText.text = ((int)_def).ToString();
    }

    public void DisplayHP(float _max)
    {
        hpText.text = ((int)_max).ToString();

    }

    public void DisplayMP(float _max)
    {
        mpText.text = ((int)_max).ToString();
    }
}
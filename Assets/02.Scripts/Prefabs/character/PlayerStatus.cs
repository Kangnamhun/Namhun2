using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PartInfo
{
    //list �� ���� ���� -> �ش��̸��� on
    public string partName;
    public GameObject partDefault;
    public List<GameObject> partList = new List<GameObject>();
    public ItemData itemData;



    public void SetItemData(ItemData _itemData)
    {
        itemData = _itemData;
    }

    public void Equip(string _partName, eEquipmentSlot _equipmentSlot)//����  ��Ų ����
    {
        for (int i = 0, imax = partList.Count; i < imax; i++)
        {
            if(partList[i].name == _partName) //�̸��� ������ Ų��
            {
                partList[i].SetActive(true);
                if (_equipmentSlot == eEquipmentSlot.Weapon)
                {
                    Managers.Game.GetPlayer().GetComponent<PlayerAttack>().HasWeapon = true;
                }
            }
        }
        
    }

    public void UnEquip(string _partName, eEquipmentSlot _equipmentSlot) //��� Ż��
    {
        for (int i = 0, imax = partList.Count; i < imax; i++) //partList���� ������ �̸��̶� ���� ��� ã�´�
        {
            if (partList[i].name == _partName) //�̸��� ������ ����
            {
                partList[i].SetActive(false);
                if (_equipmentSlot == eEquipmentSlot.Weapon)
                {
                    Managers.Game.GetPlayer().GetComponent<PlayerAttack>().HasWeapon = false;
                   
                }
            }
        }
    }
}

public class PlayerStatus : Status
{
    #region PartInfo ����
    // 0     1       2      3   
    //Head, Armor, Weapon, Boots
    public List<PartInfo> listPartInfos = new List<PartInfo>();

    public void Equip(int _index, ItemData _itemData)
    {
        //Mesh(����)��ü
        PartInfo _partInfo = listPartInfos[_index];
        _partInfo.Equip(_itemData.skin, _itemData.equipmentSlot);//Ż���� ��Ų�� �Ҵ�
        _partInfo.Equip(_itemData.skin2, _itemData.equipmentSlot);//Ż���� ��Ų�� �Ҵ�
        _partInfo.SetItemData(_itemData);
        //default off ����Ʈ�� ���ش�
        if (_partInfo.partDefault != null)
        {
            _partInfo.partDefault.SetActive(false);
        }
        SetEquip(_partInfo, _itemData);
    }

    public void UnEquip(int _index, ItemData _itemData)
    {
        PartInfo _partInfo = listPartInfos[_index];
        _partInfo.UnEquip(_itemData.skin, _itemData.equipmentSlot);//Ż���� ��Ų�� ����
        _partInfo.UnEquip(_itemData.skin2, _itemData.equipmentSlot);//Ż���� ��Ų�� ����
        _partInfo.SetItemData(null);//��� �����Ҷ��� null�� �־��ش�
        //default off ����Ʈ�� ���ش�
        if (_partInfo.partDefault != null) //partDefault null�̾ƴϸ�
        {
            _partInfo.partDefault.SetActive(true);
        }
        SetEquip(_partInfo, _itemData, -1);
    }
    public void SetEquip(PartInfo _partInfo, ItemData _itemData, float Un = 1f)
    {
        //�������� �����ϸ� status��ü(����)
        wearAttack += _itemData.plusatt * Un;
        wearDefense += _itemData.plusdef * Un;
        wearHP += _itemData.plushp * Un;
        wearMP += _itemData.plusmp * Un;
        Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP); //ü�� ������ �̹��� ������
        Managers.UI.ui_PlayerData.DisplayMP(mp, MAX_MP);

        //Debug.Log("@@@UI_Equipment�Ʒ�����");
        Managers.UI.ui_Equipment.DisplayAttack(attack);
        Managers.UI.ui_Equipment.DisplayDEF(defense);
        Managers.UI.ui_Equipment.DisplayHP(MAX_HP);
        Managers.UI.ui_Equipment.DisplayMP(MAX_MP);

    }

    #endregion
    #region SetUp
    enum eAbiltyKind { LevelHP, LevelMP, LevelAttack, LevelDefense, LevelExp };
    public ParticleSystem psLevelUp;

    float gold1, gold2;
    public float gold
    {
        //���ȼ� ������ ����2���� ����
        //��) �ʵ忡�� 1000��带 �����ҽ� gold1�� 500�� gold2�� 500�� �޾ƿͼ� ���� ��ħ
        get { return gold1 + gold2; }
        set
        {
            float _plus = value - (gold1 + gold2); //value =gold1 + gold2 + _pick.count
            int _g1 = (int)_plus / 2;
            int _g2 = (int)_plus - _g1;

            gold1 += _g1;
            gold2 += _g2;

            Managers.UI.ui_Money.DisplayCoin(gold);
        }
    }
    public float level;

    float totalExp;
    float[] expArray;
    public float exp
    {
        get { return totalExp; }
        set
        {
      
            float _plus = value - totalExp;
            totalExp += _plus;
            float _levelOld = level;

            level = GetLevel(totalExp);
            levelHP = GetAbility(eAbiltyKind.LevelHP);
            levelMP = GetAbility(eAbiltyKind.LevelMP);
            levelAttack = GetAbility(eAbiltyKind.LevelAttack);
            levelDefense = GetAbility(eAbiltyKind.LevelDefense);
            levelExp = GetAbility(eAbiltyKind.LevelExp);

            if (level != _levelOld) //������ �ҽ�
            {
                if (level >= 2) //������2�̻� 
                {
                    //StartCoroutine(Co_ShowLevelUp(2f));
                    psLevelUp.gameObject.SetActive(true);//������ ��ƼŬ ����
                    psLevelUp.Stop();
                    psLevelUp.Play();
                    Managers.Sound.Play("EffectSound/LevelUp");
                    // Managers �Ҹ��ֱ�
                }

                Hp = MAX_HP; //�������� hp�� ���� ȸ��
                mp = MAX_MP;
                Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP);
                Managers.UI.ui_PlayerData.DisplayMP(Hp, MAX_MP);
            }
            float _needExp = GetNeedExp(level); //- GetNeedExp(level - 1); //���緹�� - ������
            float _curExp = totalExp - GetNeedExp(level - 1); //���������� ���緹������
            Managers.UI.ui_PlayerData.DisplayEXP(_curExp, _needExp);
            Managers.UI.ui_PlayerData.DisplayLevelText(level);
        }

    }


    #endregion
    protected override void Init()
    {
        base.Init();
        psLevelUp.gameObject.SetActive(false); //���۽� ������ ��ƼŬ ����
        //���۽�hp ,mp�� ó�� baseHP,baseMP ������ ������ ����
        
        expArray = new float[10 + 1];
        for (int i = 1; i < expArray.Length; i++)
        {
            expArray[i] = GetNeedExp(i) + GetNeedExp(i-1);
        }

    }



  

    float GetNeedExp(float _level) //����ġ ����ϴ� �Լ�
    {
        return _level <= 0 ? 0 : (_level * 30); //������ �ʿ����ġ
    }


    float GetLevel(float _totalExp)
    {
        float _level = 1;
        bool _bFind = false;
        for (int i = 0; i < expArray.Length -1; i++)
        {
            if(_totalExp >= expArray[i] && _totalExp < expArray[i + 1])
            {
                _level = i + 1;
                _bFind = true;
            }
        }

        //���� ���̺��� �������� ã�Ҵµ� ��ã����� �ְ���(����)
        if(!_bFind)
        {
            _level = expArray.Length;
        }
        return _level;
    }

    float GetAbility(eAbiltyKind _kind)
    {
        float _rtn = 0;
        float _level = level - 1;
        switch (_kind)
        {
            case eAbiltyKind.LevelHP:  _rtn = _level * 20; break;   //�������� �����ϴ�HP
            case eAbiltyKind.LevelMP: _rtn = _level * 20; break;    //�������� �����ϴ�MP
            case eAbiltyKind.LevelAttack: _rtn = _level * 5.0f; break;//�������� �����ϴ� Attack
            case eAbiltyKind.LevelDefense: _rtn = _level * 0.5f; break;//�������� �����ϴ�DEF
            case eAbiltyKind.LevelExp: _rtn = _level * 10f; break;
        }
        Managers.UI.ui_Equipment.DisplayAttack(attack);
        Managers.UI.ui_Equipment.DisplayDEF(defense);
        Managers.UI.ui_Equipment.DisplayHP(MAX_HP);
        Managers.UI.ui_Equipment.DisplayMP(MAX_MP);
        return _rtn;
    }


    public void SetHPMP(float _hp, float _mp)
    {
        //HP MP�� Plus
        Hp += _hp; //������ ������
        Hp = Hp > MAX_HP ? MAX_HP : Hp; //hp�� MAX���� �ʰ��ϸ� ���̻� ȸ�������ʴ´�
        Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP); //ü�� ������ �̹��� ������
        //HP MP
        mp += _mp; //������ ������
        mp = mp > MAX_MP ? MAX_MP : mp;//mp�� MAX���� �ʰ��ϸ� ���̻� ȸ�������ʴ´�
        Managers.UI.ui_PlayerData.DisplayMP(mp, MAX_MP);
    }


    public bool Skill(float _useMP) //PlayerStatusTest ��ũ��Ʈ �����(����)
    {
        //mp : ĳ���� Ŭ���� + ��� + ������ ��� ����� mp
        if (mp - _useMP < 0) 
        {
            Managers.UI.ui_ErrorText.SetErrorText(Define.Error.NoneMp);
            return false; //���ǰ� �����ϸ� false

        }
        else
        {
            mp -= _useMP;
            Managers.UI.ui_PlayerData.DisplayMP(mp, MAX_MP);
            return true;

        }
    }
    public override void TakeDamage(Status attacker, float ratio)
    {
        base.TakeDamage(attacker, ratio);
        Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP); //ü�� ������ �̹��� ������
    }
    protected override void Die()
    {
        base.Die();
        animator.SetBool("PlayerDie", BDeath);
        UI_Message ui_Message = Managers.UI.ShowPopupUI<UI_Message>();
        ui_Message.Init();
        ui_Message.ShowMessage("����", "��Ȱ�Ͻðڽ��ϱ�?");
        ui_Message.okButton.gameObject.AddUIEvent(ui_Message.ReSpawn);
        ui_Message.cancelButton.gameObject.AddUIEvent(ui_Message.TownSpawn);
    }


}

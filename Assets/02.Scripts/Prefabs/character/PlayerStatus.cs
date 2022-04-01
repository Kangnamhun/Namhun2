using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PartInfo
{
    //list 를 전부 끄고 -> 해당이름만 on
    public string partName;
    public GameObject partDefault;
    public List<GameObject> partList = new List<GameObject>();
    public ItemData itemData;



    public void SetItemData(ItemData _itemData)
    {
        itemData = _itemData;
    }

    public void Equip(string _partName, eEquipmentSlot _equipmentSlot)//장착  스킨 켜짐
    {
        for (int i = 0, imax = partList.Count; i < imax; i++)
        {
            if(partList[i].name == _partName) //이름이 같으면 킨다
            {
                partList[i].SetActive(true);
                if (_equipmentSlot == eEquipmentSlot.Weapon)
                {
                    Managers.Game.GetPlayer().GetComponent<PlayerAttack>().HasWeapon = true;
                }
            }
        }
        
    }

    public void UnEquip(string _partName, eEquipmentSlot _equipmentSlot) //장비 탈착
    {
        for (int i = 0, imax = partList.Count; i < imax; i++) //partList에서 던져준 이름이랑 같은 장비를 찾는다
        {
            if (partList[i].name == _partName) //이름이 같으면 끈다
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
    #region PartInfo 정보
    // 0     1       2      3   
    //Head, Armor, Weapon, Boots
    public List<PartInfo> listPartInfos = new List<PartInfo>();

    public void Equip(int _index, ItemData _itemData)
    {
        //Mesh(외형)교체
        PartInfo _partInfo = listPartInfos[_index];
        _partInfo.Equip(_itemData.skin, _itemData.equipmentSlot);//탈착한 스킨을 켠다
        _partInfo.Equip(_itemData.skin2, _itemData.equipmentSlot);//탈착한 스킨을 켠다
        _partInfo.SetItemData(_itemData);
        //default off 디폴트를 꺼준다
        if (_partInfo.partDefault != null)
        {
            _partInfo.partDefault.SetActive(false);
        }
        SetEquip(_partInfo, _itemData);
    }

    public void UnEquip(int _index, ItemData _itemData)
    {
        PartInfo _partInfo = listPartInfos[_index];
        _partInfo.UnEquip(_itemData.skin, _itemData.equipmentSlot);//탈착한 스킨을 끈다
        _partInfo.UnEquip(_itemData.skin2, _itemData.equipmentSlot);//탈착한 스킨을 끈다
        _partInfo.SetItemData(null);//장비를 해제할때는 null을 넣어준다
        //default off 디폴트를 꺼준다
        if (_partInfo.partDefault != null) //partDefault null이아니면
        {
            _partInfo.partDefault.SetActive(true);
        }
        SetEquip(_partInfo, _itemData, -1);
    }
    public void SetEquip(PartInfo _partInfo, ItemData _itemData, float Un = 1f)
    {
        //아이템을 장착하면 status교체(감소)
        wearAttack += _itemData.plusatt * Un;
        wearDefense += _itemData.plusdef * Un;
        wearHP += _itemData.plushp * Un;
        wearMP += _itemData.plusmp * Un;
        Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP); //체력 게이지 이미지 움직임
        Managers.UI.ui_PlayerData.DisplayMP(mp, MAX_MP);

        //Debug.Log("@@@UI_Equipment아래스텟");
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
        //보안성 때문에 변수2개로 지정
        //예) 필드에서 1000골드를 습득할시 gold1에 500원 gold2에 500원 받아와서 둘이 합침
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

            if (level != _levelOld) //레벨업 할시
            {
                if (level >= 2) //레벨이2이상 
                {
                    //StartCoroutine(Co_ShowLevelUp(2f));
                    psLevelUp.gameObject.SetActive(true);//레벨업 파티클 실행
                    psLevelUp.Stop();
                    psLevelUp.Play();
                    Managers.Sound.Play("EffectSound/LevelUp");
                    // Managers 소리넣기
                }

                Hp = MAX_HP; //레벨업시 hp를 전부 회복
                mp = MAX_MP;
                Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP);
                Managers.UI.ui_PlayerData.DisplayMP(Hp, MAX_MP);
            }
            float _needExp = GetNeedExp(level); //- GetNeedExp(level - 1); //현재레벨 - 전레벨
            float _curExp = totalExp - GetNeedExp(level - 1); //전레벨에서 현재레벨빼기
            Managers.UI.ui_PlayerData.DisplayEXP(_curExp, _needExp);
            Managers.UI.ui_PlayerData.DisplayLevelText(level);
        }

    }


    #endregion
    protected override void Init()
    {
        base.Init();
        psLevelUp.gameObject.SetActive(false); //시작시 레벨업 파티클 끄기
        //시작시hp ,mp를 처음 baseHP,baseMP 선언한 값으로 시작
        
        expArray = new float[10 + 1];
        for (int i = 1; i < expArray.Length; i++)
        {
            expArray[i] = GetNeedExp(i) + GetNeedExp(i-1);
        }

    }



  

    float GetNeedExp(float _level) //경험치 계산하는 함수
    {
        return _level <= 0 ? 0 : (_level * 30); //레벨당 필요경험치
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

        //레벨 테이블에서 레벨값을 찾았는데 못찾을경우 최고레벨(만렙)
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
            case eAbiltyKind.LevelHP:  _rtn = _level * 20; break;   //레벨업시 증가하는HP
            case eAbiltyKind.LevelMP: _rtn = _level * 20; break;    //레벨업시 증가하는MP
            case eAbiltyKind.LevelAttack: _rtn = _level * 5.0f; break;//레벨업시 증가하는 Attack
            case eAbiltyKind.LevelDefense: _rtn = _level * 0.5f; break;//레벨업시 증가하는DEF
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
        //HP MP를 Plus
        Hp += _hp; //물약을 먹을시
        Hp = Hp > MAX_HP ? MAX_HP : Hp; //hp가 MAX양을 초과하면 더이상 회복하지않는다
        Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP); //체력 게이지 이미지 움직임
        //HP MP
        mp += _mp; //물약을 먹을시
        mp = mp > MAX_MP ? MAX_MP : mp;//mp가 MAX양을 초과하면 더이상 회복하지않는다
        Managers.UI.ui_PlayerData.DisplayMP(mp, MAX_MP);
    }


    public bool Skill(float _useMP) //PlayerStatusTest 스크립트 실험용(엠피)
    {
        //mp : 캐릭터 클래스 + 장비 + 레벨이 모두 적용된 mp
        if (mp - _useMP < 0) 
        {
            Managers.UI.ui_ErrorText.SetErrorText(Define.Error.NoneMp);
            return false; //엠피가 부족하면 false

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
        Managers.UI.ui_PlayerData.DisplayHP(Hp, MAX_HP); //체력 게이지 이미지 움직임
    }
    protected override void Die()
    {
        base.Die();
        animator.SetBool("PlayerDie", BDeath);
        UI_Message ui_Message = Managers.UI.ShowPopupUI<UI_Message>();
        ui_Message.Init();
        ui_Message.ShowMessage("죽음", "부활하시겠습니까?");
        ui_Message.okButton.gameObject.AddUIEvent(ui_Message.ReSpawn);
        ui_Message.cancelButton.gameObject.AddUIEvent(ui_Message.TownSpawn);
    }


}

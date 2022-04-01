using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Define
{
    public enum UIEvent
    {
        Click,
        Drag,
        OnPointer,
        OnPointerExit
    }
    public enum AttackType
    {
        NormalAttack,
        SkillAttack
    }
    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerRightDown,
        PointerUp,
        Click,
    }
    public enum Sound
    {
        BGM,
        Effect,
        LoopEffect,
        Moving,
        MaxCount
    }
    public enum Error
    {
        NoneWeapon,
        OtherWeapon,
        NoneJob,
        CoolTime,
        NoneMp,
        NonePotion,
        NoneGold,
        NoneTarget,
        MaxInv
    }

    public enum Items
    {
        HpPotion,
        MpPotion,
        Bow1,
        Bow2,
        Sword1,
        Sword2,
        Wand1,
        Wand2,
        Coin
    }

    public enum Itemcode
    {
        Sword1  = 20001,
        Sword2 = 20002,
        Wand1 = 20101,
        Wand2 = 20102,
        Bow1 = 20201,
        Bow2 = 20202
    }
}


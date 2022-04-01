using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerData : UI_Scene
{
    #region SetUp
    Text levelText;
    Image hpbar, mpbar, expbar;
    Text hpText, mpText, expText;


    enum Texts
    {
        HPText,
        MPText,
        EXPText,
        LevelText
    }

    enum Images
    {
        HPBar,
        MPBar,
        EXPBar
    }
    PlayerStatus playerStatus;
    public override void Init()
    {
        base.Init();
        

        
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        hpText = GetText((int)Texts.HPText);
        mpText = GetText((int)Texts.MPText);
        expText = GetText((int)Texts.EXPText);
        levelText = GetText((int)Texts.LevelText);

        hpbar = GetImage((int)Images.HPBar);
        mpbar = GetImage((int)Images.MPBar);
        expbar = GetImage((int)Images.EXPBar);
        playerStatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
        playerStatus.SetHPMP(playerStatus.MAX_HP, playerStatus.MAX_MP);
    }
    #endregion

    #region DisPlayUpdate
    public void DisplayHP(float _hp, float _max)
    {
        float _v = _hp / _max;
        hpbar.fillAmount = _v;
        hpText.text = _hp.ToString();

    }
    
    public void DisplayMP(float _mp, float _max)
    {
        float _v = _mp / _max;
        mpbar.fillAmount = _v;
        mpText.text = _mp.ToString();
    }

    public void DisplayEXP(float _exp, float _max)
    {
        float _v = _exp / _max;
        expbar.fillAmount = _v;
        expText.text = string.Format("{0:0.0}", (_v * 100f)) + "%"; //소수점 한자리까지만 출력하는 함수
    }

    public void DisplayLevelText(float _level)
    {
        
        levelText.text = string.Format("{0:0}",_level); 
    }
    #endregion
}

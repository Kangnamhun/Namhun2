using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Money : UI_Scene
{
    #region SetUp
    bool bInit;



    enum Texts
    {
        MoneyCountText
    }
    Text moneyCountText;


    public override void Init()
    {
        if(bInit)return;
        bInit = true;

        Bind<Text>(typeof(Texts));
        moneyCountText = GetText((int)Texts.MoneyCountText);
        DisplayCoin(0); //첫시작시 0으로 시작

    }
    #endregion
    public void DisplayCoin(float _coin)
    {

        moneyCountText.text = string.Format("{0:0}", _coin); 

    }
}

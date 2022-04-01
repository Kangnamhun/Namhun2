using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ErrorText : UI_Scene
{
    enum Texts
    {
        ErrorText
    }

    Text errorText;
    Color alpha;
    float alphaSpeed;
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));

        errorText = GetText((int)Texts.ErrorText);

        alpha.a = 0f;
        alphaSpeed = 12f;
        
    }

    private void Update()
    {

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); //텍스트 알파값 조정
        errorText.color = alpha;
    }

    public void SetErrorText(Define.Error type)
    {
        alpha.a = 255f;
        
        switch (type)
        {
            case Define.Error.NoneJob:
                errorText.text = "직업이 없습니다.";
                break;
            case Define.Error.NoneWeapon:
                errorText.text = "장착한 무기가 없습니다.";          
                break;
            case Define.Error.OtherWeapon:
                errorText.text = "직업과 다른 무기 입니다.";
                break;
            case Define.Error.CoolTime:
                errorText.text = "쿨타임 입니다.";
                break;
            case Define.Error.NoneMp:
                errorText.text = "마나가 부족합니다.";
                break;
            case Define.Error.NonePotion:
                errorText.text = "물약이 없습니다.";
                break;
            case Define.Error.NoneGold:
                errorText.text = "보유 골드가 부족합니다";
                break;
            case Define.Error.NoneTarget:
                errorText.text = "몬스터를 지정해주세요";
                break;
            case Define.Error.MaxInv:
                errorText.text = "인벤토리가 가득 찼습니다.";
                break;
        }

    }

}
